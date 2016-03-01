using OpenQA.Selenium;

namespace Pluralsaver.PluralsightPages
{
    public class LoginPage
    {
        public static IWebElement UserNameInput
        {
            get { return Driver.Instance.FindElement(By.Id("Username")); }
        }
        public static IWebElement PasswordInput
        {
            get { return Driver.Instance.FindElement(By.Id("Password")); }
        }
        public static IWebElement SigninButton
        {
            get { return Driver.Instance.FindElement(By.Id("login")); }
        }

        static LoginPage()
        {
            Driver.WaitUntilVisible(By.Id("Username"));
        }

        public static void Login(string login, string password)
        {
            UserNameInput.SendKeys(login);
            PasswordInput.SendKeys(password);
            SigninButton.Click();

            HomePage.VerifyLoggedState();
        }
    }
}
