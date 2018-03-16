using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace repetit
{
	[TestFixture]
	public class DkorExampleTest
	{
		private IWebDriver driver;
		private WebDriverWait wait;

		[SetUp]
		public void Start()
		{
			//ChromeOptions options = new ChromeOptions();
			driver = new ChromeDriver();
			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			//Debug.WriteLine(options);
		}

		[Test]
		public void AddLessonsForNextWeek()
		{
			Login();
			Thread.Sleep(500);

			driver.Url = "https://repetit.ru/teacher/schedule.aspx";

			Thread.Sleep(500);
			driver.Manage().Window.Maximize();

			driver.FindElement(By.CssSelector("input.pupils-list-filter")).Click();
			var pupilsCount = driver.FindElements(By.CssSelector("div.pupil-card.schedule-required")).Count;

			var pupils = CreatePupils();

			for (var i = 0; i < pupilsCount; i++)
			{
				var pSel = $"div.pupil-card.schedule-required:nth-child({i + 1})";

				driver.ScrollElementIntoView(driver.FindElement(By.CssSelector(pSel)));
				var pName = driver.FindElement(By.CssSelector(pSel)).FindElement(By.CssSelector(".pupil-name")).Text;

				var pCurrent = Pupil.FindByName(pupils, pName);

				driver.FindElement(By.CssSelector(pSel))
					.FindElement(By.CssSelector("span.add-lesson span")).Click();
				Thread.Sleep(300);

				var currentMonth = DateTime.Today.Month;
				var pDay = NextMonday().AddDays(pCurrent.DayOfWeek-1);

				var pastCount = driver.FindElements(By.CssSelector(".date.past")).Count;
				var pDaySelector = String.Concat(".date",$":nth-child({pDay.Day + pastCount})");

				if (currentMonth != pDay.Month)
				{
					// go to first day of next month
					driver.FindElement(By.CssSelector("div.date.future")).Click();
					Thread.Sleep(300);
				}

				driver.FindElement(By.CssSelector(pDaySelector)).Click();

				driver.FindElement(By.CssSelector("input[data-bind=lessonTimeFrom]"))
					.SendKeys($"{pCurrent.TimeStart}:00");

				driver.FindElement(By.CssSelector("input[data-bind=lessonTimeTo]"))
					.SendKeys($"{pCurrent.TimeStart + 1}:00");

				driver.FindElement(By.CssSelector("button.save"))
					.Click();
				Thread.Sleep(300);
			}
		}

		private static DateTime NextMonday()
		{
			DateTime today = DateTime.Today;
			int daysUntilTuesday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
			return today.AddDays(daysUntilTuesday);
		}


		private List<Pupil> CreatePupils()
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

		[Test]
		public void DeleteAllLessonsForNextWeek()
		{
			Login();
			Thread.Sleep(500);

			driver.Url = "https://repetit.ru/teacher/schedule.aspx";
			driver.Manage().Window.Maximize();

			DeleteAllLessonsForSelectedWeek(1);
		}

		private void DeleteAllLessonsForSelectedWeek(int weekIndex)
		{
			var week = weekIndex;

			// scroll to target week
			wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(".timetable-body")));
			for (var i = 0; i < week; i++)
			{
				wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("span.next")));
				var timetable = driver.FindElement(By.CssSelector(".timetable-body"));

				driver.FindElement(By.CssSelector("span.next")).Click();
				wait.Until(ExpectedConditions.StalenessOf(timetable));
			}

			Thread.Sleep(5000);
			DeleteAllLessons();
		}

		private void DeleteAllLessons()
		{
			var lessonsCount = driver.FindElements(By.CssSelector("div.lesson")).Count;
			Assert.Greater(lessonsCount, 0, $"Количество занятий на выбранной неделе равно {lessonsCount}");

			for (var i = 0; i < lessonsCount; i++)
			{
				wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("div.lesson")));
				var currentLesson = driver.FindElement(By.CssSelector("div.lesson"));

				driver.FindElement(By.CssSelector("div.lesson")).Click();
				wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("li.edit-lesson")));

				driver.FindElement(By.CssSelector("li.edit-lesson")).Click();
				wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("button.delete")));

				driver.FindElement(By.CssSelector("button.delete")).Click();
				wait.Until(ExpectedConditions.StalenessOf(currentLesson));
			}
		}

		private void Login()
		{
			var login = ConfigurationManager.AppSettings["login"];
			var pass = ConfigurationManager.AppSettings["password"];

			if (login == "" || pass == "") { throw new System.ArgumentException("Please provide correct login data"); }

			driver.Url = "https://repetit.ru";
			driver.FindElement(By.CssSelector("div.cabinet")).Click();
			driver.FindElement(By.CssSelector("div.cabinet button")).Click();
			driver.FindElement(By.CssSelector("input.login")).SendKeys(login);
			driver.FindElement(By.CssSelector("input.password")).SendKeys(pass);
			driver.FindElement(By.CssSelector("button.login-btn")).Click();
		}

		[TearDown]
		public void Stop()
		{
			driver.Quit();
			driver = null;
		}
	}
}
