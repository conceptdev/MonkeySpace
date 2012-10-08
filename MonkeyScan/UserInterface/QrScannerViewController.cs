using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;
using MonoTouch.AVFoundation;
using MonoTouch.CoreVideo;
using MonoTouch.CoreMedia;
using com.google.zxing;
using com.google.zxing.common;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace QrSample.iOS
{
	// based on https://github.com/xamarin/monotouch-samples/blob/master/AVCaptureFrames/Main.cs
	public class QrScannerViewController : UIViewController
	{
		// azure
		private static readonly MobileServiceClient MobileService = new MobileServiceClient (Constants.AzureUrl, Constants.AzureKey);
		private readonly IMobileServiceTable<ConfScan> scanTable = MobileService.GetTable<ConfScan>();
		private TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		// --

		public event Action<Result> QrScan;

		AVCaptureSession session;
		AVCaptureVideoPreviewLayer previewLayer;

		DispatchQueue queue;
		QrScanner qrScanner;

		UILabel qrResult;
		UIButton startButton;

		public override void LoadView ()
		{
			base.LoadView ();

			if (!SetupCaptureSession ())
				throw new NotSupportedException ("Unable to setup camera for QR scan");

			previewLayer.Frame = new System.Drawing.RectangleF (0, 40, 320, 320); //HACK: UIScreen.MainScreen.Bounds;
			View.Layer.AddSublayer (previewLayer);

			qrResult = new UILabel(new RectangleF (0, 380, 320, 30));
			View.AddSubview (qrResult);

			QrScan += (result) => {
				Console.WriteLine ("Scan : " + result.BarcodeFormat + " " + result.Text );
				InvokeOnMainThread (() => {
					//new UIAlertView ("Scan", result.Text, null, "OK").Show ()
					qrResult.Text += result.Text + "\n";

					var scan = new ConfScan () {Barcode=result.Text, ScannedAt=DateTime.Now};
					AppDelegate.UserData.AddScan (scan);

					scanTable.InsertAsync (scan)
						.ContinueWith (t => {
							Console.WriteLine (t.Status);
							Console.WriteLine ("Updated scan in cloud " + t);

						}, scheduler);

					session.StopRunning ();
				});
			};

			startButton = UIButton.FromType (UIButtonType.RoundedRect);
			startButton.Frame = new RectangleF(0,0,50,30);
			startButton.SetTitle ("Scan", UIControlState.Normal);
			startButton.TouchUpInside += (sender, e) => {
				session.StartRunning ();
			};
			View.AddSubview(startButton);
		}

		bool SetupCaptureSession ()
		{
			// configure the capture session for low resolution, change this if your code
			// can cope with more data or volume
			session = new AVCaptureSession () {
				SessionPreset = AVCaptureSession.PresetMedium
			};

			// create a device input and attach it to the session
			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			if (captureDevice == null) {
				// No input device
				return false;
			}
			var input = AVCaptureDeviceInput.FromDevice (captureDevice);
			if (input == null){
				// No input device
				return false;
			}
			session.AddInput (input);

			// create a VideoDataOutput and add it to the sesion
			var output = new AVCaptureVideoDataOutput () {
				VideoSettings = new AVVideoSettings (CVPixelFormatType.CV32BGRA)
			};

			// configure the output
			queue = new DispatchQueue ("myQueue");
			qrScanner = new QrScanner (this);
			output.SetSampleBufferDelegateAndQueue (qrScanner, queue);
			session.AddOutput (output);

			previewLayer = new AVCaptureVideoPreviewLayer (session);
			previewLayer.Orientation = AVCaptureVideoOrientation.Portrait;
			previewLayer.VideoGravity = "AVLayerVideoGravityResizeAspectFill";

			session.StartRunning ();
			return true;
		}
	
		class QrScanner : AVCaptureVideoDataOutputSampleBufferDelegate
		{
			QrScannerViewController parent;
			MultiFormatReader reader;
			byte [] bytes;

			public QrScanner (QrScannerViewController parent)
			{
				this.parent = parent;
				this.reader = new MultiFormatReader {
					Hints = new Hashtable {
						//BarcodeFormat.QR_CODE, PDF417
						{ DecodeHintType.POSSIBLE_FORMATS, new ArrayList {  BarcodeFormat.QR_CODE } }
					}
				};
			}

			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{
				try {
					LuminanceSource luminance;

					using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
	
						if (bytes == null)
							bytes = new byte [pixelBuffer.Height * pixelBuffer.BytesPerRow];
	
						pixelBuffer.Lock (0);
						Marshal.Copy (pixelBuffer.BaseAddress, bytes, 0, bytes.Length);
	
						luminance = new RGBLuminanceSource (bytes, pixelBuffer.Width, pixelBuffer.Height);
						pixelBuffer.Unlock (0);
					}

					var binarized = new BinaryBitmap (new HybridBinarizer (luminance));
					var result = reader.decodeWithState (binarized);

					//parent.session.StopRunning ();

					if (parent.QrScan != null)
						parent.QrScan (result);

				} catch (ReaderException) {

					// ignore this exception; it happens every time there is a failed scan

				} catch (Exception e) {

					// TODO: this one is unexpected.. log or otherwise handle it

					throw;

				} finally {
					try {
						// lamest thing, but seems that this throws :(
						sampleBuffer.Dispose ();
					} catch { }
				}
			}
		}
	}
}