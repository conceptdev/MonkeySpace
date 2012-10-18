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

namespace MonkeyScan
{
	// based on https://github.com/xamarin/monotouch-samples/blob/master/AVCaptureFrames/Main.cs
	public class QRScannerViewController : UIViewController
	{
		public TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

		public event Action<Result> QrScan;

		AVCaptureSession session;
		AVCaptureVideoPreviewLayer previewLayer;

		DispatchQueue queue;
		QrScanner qrScanner;

		UILabel qrResult1, qrResult2;
		UIButton scanButton;

		public override void LoadView ()
		{
			base.LoadView ();

			if (!SetupCaptureSession ())
				throw new NotSupportedException ("Unable to setup camera for QR scan");

			scanButton = UIButton.FromType (UIButtonType.RoundedRect);
			scanButton.Frame = new RectangleF(50,60,220,80);
			scanButton.SetTitle ("Scan Again", UIControlState.Normal);
			scanButton.TouchUpInside += (sender, e) => {
				View.BackgroundColor = UIColor.Black;
				session.StartRunning ();
				qrResult1.Text = "";
				qrResult2.Text = "";
				scanButton.RemoveFromSuperview ();
			};

			qrResult1 = new UILabel(new RectangleF (10, 300, 300, 40));
			qrResult1.TextColor = UIColor.White;
			qrResult1.BackgroundColor = UIColor.Clear;
			qrResult1.Font = UIFont.BoldSystemFontOfSize (32f);
			qrResult1.AdjustsFontSizeToFitWidth = true;

			qrResult2 = new UILabel(new RectangleF (10, 350, 300, 30));
			qrResult2.TextColor = UIColor.White;
			qrResult2.BackgroundColor = UIColor.Clear;
			qrResult2.Font = UIFont.BoldSystemFontOfSize (18f);


			previewLayer.Frame = new System.Drawing.RectangleF (0, 0, 320, 290);
			View.Layer.AddSublayer (previewLayer);

			QrScan += (result) => {
				Console.WriteLine ("Scan : " + result.BarcodeFormat + " " + result.Text );
				InvokeOnMainThread (() => {
					qrResult2.Text = result.Text;

					var scan = new ConfScan () { Barcode=result.Text, ScannedAt=DateTime.Now };

					bool valid = false, reentry = false;

					// LOCAL DATA OPERATIONS
					// check for attendee
					var validAttendee = AppDelegate.UserData.CheckBarcode (scan);

					if (validAttendee != null) {
						valid = true;
						scan.IsValid = true;
						qrResult1.Text = validAttendee.Name;
						if (validAttendee.ScanCount > 0) {
							reentry = true;
							validAttendee.ScanCount += 1;
							qrResult2.Text += " (re-entry)";
							AppDelegate.UserData.Update (validAttendee);
						} else {
							// first entry
							validAttendee.ScanCount = 1;
							AppDelegate.UserData.Update (validAttendee);
						}
					} else {
						qrResult1.Text = "BARCODE NOT FOUND";
						scan.AttendeeName = "NOT FOUND";
						scan.IsValid = false;
					}
					AppDelegate.UserData.AddScan (scan);

					// REMOTE DATA OPERATIONS
					// Massive hack here, need to revisit how we deal with PKs in Azure
					AzureManager.scanTable.InsertAsync (scan)
						.ContinueWith (t => {
							Console.WriteLine ("Updated scan in cloud " + t.Status + " " + t.Id);
						}, scheduler);
					// we need to find out of the Attendee.Barcode is already in Azure...
					if (validAttendee != null) {
						if (validAttendee.Id > 0)
							AzureManager.attendeeTable.UpdateAsync (validAttendee)
								.ContinueWith (u => {
									Console.WriteLine ("Updated attendee in cloud " + u.Status + " update " + u.Id);
								}, scheduler);
						else {
							AzureManager.attendeeTable.InsertAsync (validAttendee)
								.ContinueWith (v => {
									Console.WriteLine ("Inserted attendee in cloud " + v.Status + " " + v.Id + " ~ " + validAttendee.Id);
									AppDelegate.UserData.Update (validAttendee); // with Azure~Id
								}, scheduler);
						}
					}

					if (valid && !reentry)
						View.BackgroundColor = UIColor.Green;
					else if (valid && reentry)
						View.BackgroundColor = UIColor.Orange;
					else
						View.BackgroundColor = UIColor.Red;

					session.StopRunning ();
					View.AddSubview (scanButton);
				});
			};

			//View.AddSubview (scanButton); // gets added/removed dynamically
			View.AddSubview (qrResult1);
			View.AddSubview (qrResult2);
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

			// create a VideoDataOutput and add it to the session
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
			QRScannerViewController parent;
			MultiFormatReader reader;
			byte [] bytes;

			public QrScanner (QRScannerViewController parent)
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