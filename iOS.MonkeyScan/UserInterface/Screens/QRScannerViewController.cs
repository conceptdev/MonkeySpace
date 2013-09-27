using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;
using MonoTouch.AVFoundation;
using MonoTouch.CoreVideo;
using MonoTouch.CoreMedia;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using MonoTouch.Foundation;

namespace MonkeyScan
{
	// based on https://github.com/xamarin/monotouch-samples/blob/master/AVCaptureFrames/Main.cs
	public class QRScannerViewController : UIViewController
	{
		public TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

		public event Action<string> QrScan;

		AVCaptureSession session;
		UILabel qrResult1, qrResult2;
		UIButton scanButton;

		public override void LoadView ()
		{
			base.LoadView ();

			if (!SetupCaptureSession ()) {
				Console.WriteLine ("Scan function requires a camera - doesn't work in the iOS Simulator");
				throw new NotSupportedException ("Unable to setup camera for QR scan");
			}
			scanButton = UIButton.FromType (UIButtonType.System);
			scanButton.BackgroundColor = UIColor.White;
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

			QrScan += (result) => {
				//Console.WriteLine ("Scan : " + result.BarcodeFormat + " " + result.Text );
				InvokeOnMainThread (async () => {

					session.StopRunning();

					qrResult2.Text = result; //.Text;

					var scan = new ConfScan () { Barcode=result, ScannedAt=DateTime.Now };

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
					// Adds to local database
					AppDelegate.UserData.AddScan (scan);


					//HACK: commented out Azure for now ~ no account 
					// and still need to test the new async-style calls
//					try {
//						// REMOTE DATA OPERATIONS
//						// Massive hack here, need to revisit how we deal with these Azure updates
//						await AzureManager.scanTable.InsertAsync (scan);
//						Console.WriteLine ("Updated scan in cloud, Id:" + scan.Id);
//						
//						// we need to find out of the Attendee.Barcode is already in Azure...
//						if (validAttendee != null) {
//							if (validAttendee.Id > 0) {
//								await AzureManager.attendeeTable.UpdateAsync (validAttendee);
//								Console.WriteLine ("Updated attendee in cloud, Id:" + validAttendee.Id);
//							} else {
//								await AzureManager.attendeeTable.InsertAsync (validAttendee);
//								Console.WriteLine ("Inserted attendee in cloud, Id:" + validAttendee.Id);
//								AppDelegate.UserData.Update (validAttendee); // with Azure~Id
//								Console.WriteLine ("Inserted attendee in local, Id:" + validAttendee.Id);
//							}
//						}
//					} catch (AggregateException ae) {
//						Console.WriteLine ("AZURE error " + ae);
//					} catch (Exception e) {
//						Console.WriteLine ("AZURE error " + e);
//					}

					if (valid && !reentry) {
						View.BackgroundColor = UIColor.Green;
						Speak ("Please enter");
					} else if (valid && reentry) {
						View.BackgroundColor = UIColor.Orange;
						Speak ("Welcome back");
					} else {
						View.BackgroundColor = UIColor.Red;
						Speak ("Denied!");
					}
					View.AddSubview (scanButton);
				});
			};

			//View.AddSubview (scanButton); // gets added/removed dynamically
			View.AddSubview (qrResult1);
			View.AddSubview (qrResult2);
		}

		bool SetupCaptureSession ()
		{
			session = new AVCaptureSession();

			AVCaptureDevice device = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);

			if (device == null) {
				Console.WriteLine("No video camera (in simulator?)");
				return false; // simulator?
			}

			NSError error = null;

			AVCaptureDeviceInput input = AVCaptureDeviceInput.FromDevice(device, out error);

			if (input == null)
				Console.WriteLine("Error: " + error);
			else
				session.AddInput(input);

			AVCaptureMetadataOutput output = new AVCaptureMetadataOutput();

			var dg = new CaptureDelegate(this);
			output.SetDelegate(dg,	MonoTouch.CoreFoundation.DispatchQueue.MainQueue);
			session.AddOutput(output);

			// This could be any list of supported barcode types
			output.MetadataObjectTypes = new NSString[] {AVMetadataObject.TypeQRCode, AVMetadataObject.TypeAztecCode};
			// OR you could just accept "all" with the following line;
//			output.MetadataObjectTypes = output.AvailableMetadataObjectTypes;  // empty
			// DEBUG: use this if you're curious about the available types
//			foreach (var t in output.AvailableMetadataObjectTypes)
//				Console.WriteLine(t);


			AVCaptureVideoPreviewLayer previewLayer = new AVCaptureVideoPreviewLayer(session);
			//previewLayer.Frame = new RectangleF(0,0, View.Frame.Size.Width, View.Frame.Size.Height);
			previewLayer.Frame = new RectangleF(0, 0, 320, 290);
			previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill.ToString();
			View.Layer.AddSublayer (previewLayer);

			session.StartRunning();

			Console.WriteLine("StartRunning");
			return true;
		}
	
		class CaptureDelegate : AVCaptureMetadataOutputObjectsDelegate 
		{
			QRScannerViewController parent;
			public CaptureDelegate (QRScannerViewController parent)
			{
				this.parent = parent;
			}
			public override void DidOutputMetadataObjects (AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
			{
				string code = "";
				foreach (var metadata in metadataObjects)
				{
					if (metadata.Type == AVMetadataObject.TypeQRCode) {
						code = ((AVMetadataMachineReadableCodeObject)metadata).StringValue;
						Console.WriteLine ("qrcode: " + code);
					} else {
						Console.WriteLine ("type: " + metadata.Type);
						code = ((AVMetadataMachineReadableCodeObject)metadata).StringValue;
						Console.WriteLine ("----: " + code);
					}
				}

				if (parent.QrScan != null)
						parent.QrScan (code);
			}
		}


		void Speak (string text) {
			var speechSynthesizer = new AVSpeechSynthesizer ();

			var speechUtterance = new AVSpeechUtterance (text) {
				Rate = AVSpeechUtterance.MaximumSpeechRate/4,
				Voice = AVSpeechSynthesisVoice.FromLanguage ("en-AU"),
				Volume = 0.5f,
				PitchMultiplier = 1.0f
			};

			speechSynthesizer.SpeakUtterance (speechUtterance);
		}

//		class QrScanner : AVCaptureVideoDataOutputSampleBufferDelegate
//		{
//			QRScannerViewController parent;
//			MultiFormatReader reader;
//			byte [] bytes;
//
//			public QrScanner (QRScannerViewController parent)
//			{
//				this.parent = parent;
//				this.reader = new MultiFormatReader {
//					Hints = new Hashtable {
//						//BarcodeFormat.QR_CODE, PDF417
//						{ DecodeHintType.POSSIBLE_FORMATS, new ArrayList {  BarcodeFormat.QR_CODE } }
//					}
//				};
//			}
//
//			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
//			{
//				try {
//					LuminanceSource luminance;
//
//					using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
//	
//						if (bytes == null)
//							bytes = new byte [pixelBuffer.Height * pixelBuffer.BytesPerRow];
//	
//						pixelBuffer.Lock (0);
//						Marshal.Copy (pixelBuffer.BaseAddress, bytes, 0, bytes.Length);
//	
//						luminance = new RGBLuminanceSource (bytes, pixelBuffer.Width, pixelBuffer.Height);
//						pixelBuffer.Unlock (0);
//					}
//
//					var binarized = new BinaryBitmap (new HybridBinarizer (luminance));
//					var result = reader.decodeWithState (binarized);
//
//					//parent.session.StopRunning ();
//
//					if (parent.QrScan != null)
//						parent.QrScan (result);
//
//				} catch (ReaderException) {
//
//					// ignore this exception; it happens every time there is a failed scan
//
//				} catch (Exception e) {
//
//					// TODO: this one is unexpected.. log or otherwise handle it
//
//					throw;
//
//				} finally {
//					try {
//						// lamest thing, but seems that this throws :(
//						sampleBuffer.Dispose ();
//					} catch { }
//				}
//			}
//		}
	}
}