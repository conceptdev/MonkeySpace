using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.Text;
using Ionic.Zip;

namespace signpassnet
{
	class MainClass
	{
		static string yourCertFile = @"C:\somedir\your-cert-exported-from-Mac.p12";
		static string yourCertFilePassword = @"whateveryouchose";

		static string basePath = @"C:\Projects\signpassnet-win\";
		static string rawDirectory = @"CouponBanana.raw";
		static string outputZipFilename = @"CouponBanana.pkpass";

		public static void Main (string[] args)
		{
			Console.WriteLine ("PassKit Generator!");
            Console.WriteLine ("------------------");

			Console.WriteLine ("Step 1 : generate hashes for manifest.json");
            var rawPath = Path.Combine(basePath, rawDirectory);

			var sb = new StringBuilder();
			string fileName = "", json = ""; 

            if (File.Exists(Path.Combine(rawPath, "manifest.json")))
                File.Delete(Path.Combine(rawPath, "manifest.json"));
            if (File.Exists(Path.Combine(rawPath, "signature")))
                File.Delete(Path.Combine(rawPath, "signature"));

			foreach (var f in Directory.GetFiles (rawPath)) {
				if (f.ToLower () == "manifest.json") {}
				else if (f.ToLower () == "signature") {}
				else {
					fileName = f.Substring (f.LastIndexOf ('\\')+1); // '/'
					json = "\r\n  \"" + fileName + "\" : \"" + CalculateSHA1 (Path.Combine (basePath, f)) + "\"";
					Console.WriteLine (json);
					sb.Append(json + ",");
				}
			}

            json = "{" + sb.ToString().Trim(',') + "\r\n}";

			File.WriteAllText (Path.Combine (rawPath, "manifest.json"), json);


			Console.WriteLine ("Step 2 : generate signature based on manifest.json");
			Sign (rawPath);

            Console.WriteLine("Step 3 : zip it");
            var outputFileNameAndPath = Path.Combine (basePath, outputZipFilename);
            ZipFile(rawPath, outputFileNameAndPath);

			Console.WriteLine ("Done! Press <enter> to close.");
			Console.ReadLine();
		}


		//http://stackoverflow.com/questions/11526572/openssl-smime-in-c-sharp
		public static void Sign(string basePath)
		{
            var cert = new X509Certificate2(yourCertFile, yourCertFilePassword);
			
			var buffer = File.ReadAllBytes(Path.Combine(basePath, "manifest.json"));
			
			ContentInfo cont = new ContentInfo(buffer);
			var cms = new SignedCms(cont, true);
			var signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert);
			
			signer.IncludeOption = X509IncludeOption.ExcludeRoot;

            // TODO: You MUST visit this URL and download/install Apple's certificates
            // http://www.apple.com/certificateauthority/
            // Apple Inc. Root Certificate
            // Apple Computer, Inc. Root Certificate
            // Worldwide Developer Relations
			cms.ComputeSignature(signer);
			
			var myCmsMessage = cms.Encode();

			File.WriteAllBytes(Path.Combine(basePath, "signature"), myCmsMessage);
		}

		public static string CalculateSHA1 (string filename)
		{
			var t = File.ReadAllBytes (filename);
			return CalculateSHA1 (t, Encoding.UTF8);
		}

		//http://stackoverflow.com/questions/1756188/how-to-use-sha1-or-md5-in-cwhich-one-is-better-in-performance-and-security-fo
		static string CalculateSHA1(byte[] buffer, Encoding enc)
		{
			SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
			return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLower ();
        }
       
        static void ZipFile(string basePath, string outFilePathAndName)
        {
            using (ZipFile zip = new ZipFile()) {
                foreach (var f in Directory.GetFiles(basePath)) {
                    zip.AddFile(f, "");
                }
                zip.Save(outFilePathAndName);
            }
        }
    }
}
