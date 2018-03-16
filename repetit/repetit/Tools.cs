﻿using System;
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
			int daysUntilTuesday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
			return today.AddDays(daysUntilTuesday);
		}

		public static List<Pupil> CreatePupils()
		{
			var pupils = new List<Pupil>();
			var p1 = new Pupil
			{
				Name = "Захар (Парк Победы)",
				DayOfWeek = 3,
				TimeStart = 20
			};

			var p2 = new Pupil
			{
				Name = "Егор (Павелецкая)",
				DayOfWeek = 5,
				TimeStart = 20
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

			var p7 = new Pupil
			{
				Name = "Данил (Немчиновка)",
				DayOfWeek = 6,
				TimeStart = 16
			};

			pupils.Add(p1);
			pupils.Add(p2);
			pupils.Add(p3);
			pupils.Add(p4);
			pupils.Add(p5);
			pupils.Add(p6);
			pupils.Add(p7);

			return pupils;
		}
	}
}
