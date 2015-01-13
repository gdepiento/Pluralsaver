using System;
using OpenQA.Selenium;

namespace Pluralsaver.PluralsightPages
{
    public class SearchResultsPage
    {
        static SearchResultsPage()
        {
            Driver.WaitUntilVisible(By.XPath("//h1[.='Search']"));
        }

        public static void NavigateToCourse(string courseTitle)
        {
            var linkSelector = By.LinkText(courseTitle);
            var courseLink = Driver.Instance.FindElement(linkSelector);
            courseLink.Click();
        }
    }
}
