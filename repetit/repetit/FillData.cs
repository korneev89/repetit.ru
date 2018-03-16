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
			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
			//Debug.WriteLine(options);
		}

		[Test]
		public void LoginOnly()
		{
			Login();
			// 15 minutes
			Thread.Sleep(15*60*1000);
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

			var pupils = Tools.CreatePupils();

			var defaultCount = 0;

			for (var i = 0; i < pupilsCount; i++)
			{
				var pSel = $"div.pupil-card.schedule-required:nth-child({i + 1})";

				driver.ScrollElementIntoView(driver.FindElement(By.CssSelector(pSel)));
				var pName = driver.FindElement(By.CssSelector(pSel)).FindElement(By.CssSelector(".pupil-name")).Text;

				var pCurrent = Pupil.FindByName(pupils, pName);

				if (pCurrent.Name == "default") { defaultCount++; }

				driver.FindElement(By.CssSelector(pSel))
					.FindElement(By.CssSelector("span.add-lesson span")).Click();
				Thread.Sleep(300);

				var currentMonth = DateTime.Today.Month;
				var pDay = Tools.NextMonday().AddDays(pCurrent.DayOfWeek-1);

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

				var info = $"Ученик {pCurrent.Name} был добавлен на {pDay.Day} число в {pCurrent.TimeStart} часов";
				Console.WriteLine(info);
				Thread.Sleep(300);
			}

			Console.WriteLine("");
			Console.WriteLine($"Добавлены занятия для {pupilsCount} учеников на следующую неделю");

			if (defaultCount>0)
			{
				var mess = $"Для {defaultCount} ученик(а)ов нет информации о времени занятий, он(и) был(и) добавлен(ы) на понедельник на 19:00";
				Assert.Warn(mess);
				Console.WriteLine("ВНИМАНИЕ !!!");
				Console.WriteLine(mess);
			}
		}

		//[Test, Retry(3)]
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
			wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".timetable-body")));
			Thread.Sleep(1000);

			for (var i = 0; i < week; i++)
			{
				wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("span.next")));
				var timetable = driver.FindElement(By.CssSelector(".timetable-body"));

				driver.FindElement(By.CssSelector("span.next")).Click();
				wait.Until(ExpectedConditions.StalenessOf(timetable));
			}

			try
			{
				wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("div.lesson")));
				DeleteAllLessons();
			}
			catch (WebDriverTimeoutException)
			{
				Console.WriteLine("Занятий на следующей неделе не было (но это не точно)");
				return;
			}
		}

		private void DeleteAllLessons()
		{
			var lessonsCount = driver.FindElements(By.CssSelector("div.lesson")).Count;
			for (var i = 0; i < lessonsCount; i++)
			{
				var currentLesson = driver.FindElement(By.CssSelector("div.lesson"));

				driver.FindElement(By.CssSelector("div.lesson")).Click();
				wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("li.edit-lesson")));

				var lesson = driver.FindElement(By.CssSelector(".schedule-popover .name")).Text;
				var time = driver.FindElement(By.CssSelector(".schedule-popover .time")).Text;

				driver.FindElement(By.CssSelector("li.edit-lesson")).Click();
				wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("button.delete")));

				driver.FindElement(By.CssSelector("button.delete")).Click();
				wait.Until(ExpectedConditions.StalenessOf(currentLesson));

				Console.WriteLine($"{lesson} - занятие {time} удалено");
			}

			Console.WriteLine("");
			Console.WriteLine($"Все занятия({lessonsCount}) на следующей неделе удалены");
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
