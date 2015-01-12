using OpenQA.Selenium;

namespace Pluralsaver.PluralsightPages
{
    public class LoginPage
    {
        public static IWebElement UserNameInput
        {
            get { return Driver.Instance.FindElement(By.Id("userHandle")); }
        }
        public static IWebElement PasswordInput
        {
            get { return Driver.Instance.FindElement(By.Id("password")); }
        }
        public static IWebElement SubmitButton
        {
            get { return Driver.Instance.FindElement(By.Id("submit")); }
        }

        static LoginPage()
        {
            Driver.WaitUntilVisible(By.Id("userHandle"));
        }

        public static void Login(string login, string password)
        {
            UserNameInput.SendKeys(login);
            PasswordInput.SendKeys(password);
            SubmitButton.Click();

            HomePage.VerifyLoggedState();
        }
    }
}
