using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repetit
{
	public class Pupil
	{
		public string Name { get; internal set; }
		public int DayOfWeek { get; internal set; }
		public int TimeStart { get; internal set; }

		public static Pupil FindByName(List<Pupil> pupils, string name)
		{
			foreach (var pupil in pupils)
			{
				if (pupil.Name == name) { return pupil; }
			}
			return new Pupil
			{
				Name = "default",
				DayOfWeek = 1,
				TimeStart = 19
			}; 
		}
	}
}
