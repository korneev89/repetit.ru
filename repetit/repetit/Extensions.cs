using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace repetit
{
	public static class Extensions
	{
		public static void ScrollElementIntoView(this IWebDriver driver, IWebElement element)
		{
			// scroll element into viewport
			IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
			jse.ExecuteScript("arguments[0].scrollIntoView(true);", element);

			// wait for scroll action
			Thread.Sleep(500);
		}
	}
}
