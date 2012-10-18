using System;
using System.Runtime.Serialization;

namespace MonkeyScan
{
	public class ConfAttendee
	{
		public ConfAttendee ()
		{
		}

//		[DataMember (Name = "id")]
		public int Id { get; set; }

		[SQLite.PrimaryKey]
		public int SqlId { get; set; }

		public string Barcode { get; set; }
		public string Name { get; set; }

		public int ScanCount { get; set; }
	}
}

