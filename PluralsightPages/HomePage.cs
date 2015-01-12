using System;
using OpenQA.Selenium;

namespace Pluralsaver.PluralsightPages
{
    public class HomePage
    {
        public static bool IsLogged
        {
            get
            {
                var accountElement = Driver.Instance.FindElement(By.ClassName("ui-account"));
                return accountElement.Displayed;
            }
        }
        public static IWebElement SignInButton
        {
            get { return Driver.Instance.FindElement(By.Id("signInLink")); }
        }

        public static void GoTo()
        {
            Driver.Instance.Navigate().GoToUrl("http://www.pluralsight.com");
            Driver.WaitUntilVisible(By.ClassName("paths"));
        }

        public static void VerifyLoggedState()
        {
            if (!IsLogged)
                throw new Exception("You expected to be logged in at this point");
        }
    }
}
