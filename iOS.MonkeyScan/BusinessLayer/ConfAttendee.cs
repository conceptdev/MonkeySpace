using System;
using System.Runtime.Serialization;


namespace MonkeyScan
{
	public class ConfAttendee
	{
		public ConfAttendee ()
		{
		}

		public int Id { get; set; }

		public string Barcode { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }

		public int ScanCount { get; set; }
	}
}

