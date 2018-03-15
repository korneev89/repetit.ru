using System;
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
		public void JustDoIt()
		{
			Login();
			Thread.Sleep(1000);

			driver.Url = "https://repetit.ru/teacher/schedule.aspx";

			Thread.Sleep(1000);
			driver.Manage().Window.Maximize();

			driver.FindElement(By.CssSelector("input.pupils-list-filter")).Click();
			//wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("div.pupil-card.schedule-required")));
			var pupilsCount = driver.FindElements(By.CssSelector("div.pupil-card.schedule-required")).Count;
			for (var i = 0; i < pupilsCount; i++)
			{
				var pSel = $"div.pupil-card.schedule-required:nth-child({i + 1})";
				
				driver.ScrollElementIntoView(driver.FindElement(By.CssSelector(pSel)));

				driver.FindElement(By.CssSelector(pSel))
					.FindElement(By.CssSelector("span.add-lesson span")).Click();
				Thread.Sleep(300);

				//TODO select date from config or usr pupil data

				// go to first day of next month
				driver.FindElement(By.CssSelector("div.date.future")).Click();
				Thread.Sleep(300);

				// select i+1 day of month (next for current date)
				var dSel = $"div.date:nth-child({i+8})";
				Thread.Sleep(300);

				driver.FindElement(By.CssSelector(dSel)).Click();

				driver.FindElement(By.CssSelector("input[data-bind=lessonTimeFrom]"))
					.SendKeys("19:00");

				driver.FindElement(By.CssSelector("input[data-bind=lessonTimeTo]"))
					.SendKeys("20:00");

				driver.FindElement(By.CssSelector("button.save"))
					.Click();
				Thread.Sleep(300);
			}
		}

		private void Login()
		{
			var login = "pro100dimon";
			var pass = "pro100rep";

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
