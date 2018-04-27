using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repetit
{
	public class Tools
	{
		public static DateTime NextMonday()
		{
			DateTime today = DateTime.Today;
			if (today.DayOfWeek == DayOfWeek.Monday) { return today.AddDays(7); }
			int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
			return today.AddDays(daysUntilMonday);
		}

		public static DateTime ThisMonday()
		{
			DateTime today = DateTime.Today;
			if (today.DayOfWeek == DayOfWeek.Monday) { return today; }
			int daysAfterMonday = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
			return today.AddDays(-daysAfterMonday);
		}

		public static List<Pupil> CreatePupils()
		{
			var pupils = new List<Pupil>();

			var p1 = new Pupil
			{
				Name = "Глеб (Skype)",
				DayOfWeek = 5,
				TimeStart = 21
			};

			var p2 = new Pupil
			{
				Name = "Егор (Павелецкая)",
				DayOfWeek = 2,
				TimeStart = 19
			};

			var p3 = new Pupil
			{
				Name = "Дарья (Skype)",
				DayOfWeek = 7,
				TimeStart = 12
			};

			var p4 = new Pupil
			{
				Name = "Эмиль (Маяковская)",
				DayOfWeek = 7,
				TimeStart = 17
			};

			var p5 = new Pupil
			{
				Name = "Максим (Немчиновка)",
				DayOfWeek = 6,
				TimeStart = 12
			};

			var p6 = new Pupil
			{
				Name = "Станислав (Немчиновка)",
				DayOfWeek = 6,
				TimeStart = 15
			};

			pupils.Add(p1);
			pupils.Add(p2);
			pupils.Add(p3);
			pupils.Add(p4);
			pupils.Add(p5);
			pupils.Add(p6);


			return pupils;
		}
	}
}
