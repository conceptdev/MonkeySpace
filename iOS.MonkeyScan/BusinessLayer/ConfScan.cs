using System;
using SQLite;

namespace MonkeyScan
{
	public class ConfScan
	{
		public ConfScan ()
		{
		}

		public int Id { get; set; }

		[PrimaryKey, AutoIncrement]
		public int SqlId { get; set; }

		public string Barcode { get; set; }
		public DateTime ScannedAt { get; set; }

		public string AttendeeName { get; set; } // HACK: ??
		public bool IsValid { get; set; }
	}
}