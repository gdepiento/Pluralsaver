using System.Linq;
using OpenQA.Selenium;

namespace Pluralsaver.PluralsightPages
{
    public class CoursePage
    {
        public static IWebElement ExpandAllLink
        {
            get { return Driver.Instance.FindElement(By.LinkText("Expand all")); }
        }

        public static bool IsExpanded
        {
            get { return !ExpandAllLink.Displayed; }
        }

        private static void Initialize()
        {
            Driver.WaitUntilVisible(By.LinkText("Table of contents"));

            if (!IsExpanded)
                ExpandAllLink.Click();
        }

        public static void OpenPlayer()
        {
            Initialize();
            CloseHeader();

            var sections = Driver.Instance.FindElements(By.CssSelector("li.accordian__section"));
            var clips = sections[0].FindElements(By.CssSelector("div.accordian__content ul li"));

            // Click on the first clip and switch to the player's window
            clips[0].FindElement(By.TagName("a")).Click();
            Driver.Instance.SwitchTo().Window(Driver.Instance.WindowHandles.Last());

            PlayerPage.DownloadCourse();
        }

        // Page header often mistakenly receives click
        private static void CloseHeader()
        {
            ((IJavaScriptExecutor) Driver.Instance).ExecuteScript(
                "var element = document.querySelector('header.header-bar');"
                + " if (element) { element.parentNode.removeChild(element); }");
        }
    }
}
