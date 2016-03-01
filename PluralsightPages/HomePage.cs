using System;
using OpenQA.Selenium;

namespace Pluralsaver.PluralsightPages
{
    public class HomePage
    {
        public static IWebElement MainSearchInput
        {
            get
            {
                var searchInput = Driver.Instance.FindElement(By.Id("searchInput"));
                searchInput.Click();

                var mainSearchInput = Driver.Instance.FindElement(By.Id("search_input"));
                return mainSearchInput;
            }
        }

        public static bool IsLogged
        {
            get
            {
                var accountElement = Driver.Instance.FindElement(By.ClassName("header-bar__account-details"));
                return accountElement.Displayed;
            }
        }
        public static IWebElement SignInLink
        {
            get { return Driver.Instance.FindElement(By.LinkText("Sign in")); }
        }
        //public static IWebElement MenuButton
        //{
        //    get { return Driver.Instance.FindElement(By.ClassName("fa-bars")); }
        //}

        public static void GoTo()
        {
            Driver.Instance.Navigate().GoToUrl("http://www.pluralsight.com");
            Driver.WaitUntilVisible(By.ClassName("logo"));
        }

        public static void VerifyLoggedState()
        {
            if (!IsLogged)
                throw new Exception("You expected to be logged in at this point");
        }

        public static void OpenCourse(string courseTitle)
        {
            MainSearchInput.SendKeys(courseTitle + Keys.Enter);

            SearchResultsPage.NavigateToCourse(courseTitle);
        }
    }
}
