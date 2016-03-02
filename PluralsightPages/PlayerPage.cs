using System;
using OpenQA.Selenium;

namespace Pluralsaver.PluralsightPages
{
    public class PlayerPage
    {
        
        static Uri ClipUrl
        {
            get
            {
                var clipUriString = ((IJavaScriptExecutor)Driver.Instance).ExecuteScript(
                    "return document.getElementsByTagName('video')[0].getAttribute('src');").ToString();
                return new Uri(clipUriString);
            }
        }

        static IWebElement ClipDurationSpan
        {
            get
            {
                var clipDurationSpanSelector = By.CssSelector("span.total-time");
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

        public static Uri GetCurrentClipUrl()
        {
            // To grab a video url we need to make sure that it's loaded
            Driver.WaitUntilVisible(By.ClassName("icon-pause"));

            // We also want to wait a timeout to mimic a human watching the video
            WaitPlayClipTimeout();

            return ClipUrl;
        }

        private static void WaitPlayClipTimeout()
        {
            const int downloadTimeSpan = 15;

            if (ClipDurationInSeconds > downloadTimeSpan)
            {
                // Find out how long we can play clip for allowing some time for downloading
                var maxPossibleClipPlayTime = ClipDurationInSeconds - downloadTimeSpan;

                Driver.WaitSeconds(maxPossibleClipPlayTime < PluralsaverSettings.PlayClipTimeout
                    ? maxPossibleClipPlayTime
                    : PluralsaverSettings.PlayClipTimeout);
            }
        }
    }
}
