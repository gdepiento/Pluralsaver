using OpenQA.Selenium;

namespace Pluralsaver.PluralsightPages
{
    public class PlayerPage
    {
        
        static string ClipUrl
        {
            get
            {
                return ((IJavaScriptExecutor)Driver.Instance).ExecuteScript(
                    "return document.getElementById('video').getAttribute('src');").ToString();

            }
        }

        static IWebElement ClipDurationSpan
        {
            get
            {
                var clipDurationSpanSelector = By.CssSelector("span#clipDurationText");
                Driver.WaitUntilNotEmptyText(clipDurationSpanSelector);

                return Driver.Instance.FindElement(clipDurationSpanSelector);
            }
        }

        static int ClipDurationInSeconds
        {
            get
            {
                // Spent a while trying to get TimeSpan.TryParseExact working, but failed
                // So wrote it myself, quick and dirty
                var duration = ClipDurationSpan.Text.Split(new[] {':'});
                return int.Parse(duration[0])*60 + int.Parse(duration[1]);
            }
        }

        public static string GetCurrentClipUrl()
        {
            // To grab a video url we need to make sure that it's loaded
            Driver.WaitUntilHidden(By.Id("clipLoadingDiv"));

            // We also want to wait a timeout to mimic a human watching the video
            WaitPlayClipTimeout();

            return ClipUrl;
        }

        private static void WaitPlayClipTimeout()
        {
            if (ClipDurationInSeconds < PluralsaverSettings.PlayClipTimeout)
                Driver.WaitSeconds(ClipDurationInSeconds - 10);
            else
                Driver.WaitSeconds(PluralsaverSettings.PlayClipTimeout);
        }
    }
}
