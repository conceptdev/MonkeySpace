using System;

namespace QrSample.iOS
{
	public class ConfScan
	{
		public ConfScan ()
		{
		}

		public int Id { get; set; }

		public string Barcode { get; set; }
		public DateTime ScannedAt { get; set; }

//		public string AttendeeName { get; set; } // HACK: ??
	}
}