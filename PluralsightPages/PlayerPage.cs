using System;
using System.IO;
using System.Linq;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

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

        static IWebElement VolumeButton
        {
            get
            {
                ShowControlPanel();
                return Driver.Instance.FindElement(By.CssSelector("button.icon-volume"));
            }
        }

        private static void ShowControlPanel()
        {
            Actions action = new Actions(Driver.Instance);
            IWebElement progressBar = Driver.Instance.FindElement(By.Id("progress-bar"));
            action.MoveToElement(progressBar).Perform();
        }

        static bool IsMuted
        {
            get
            {
                ShowControlPanel();
                return !VolumeButton.Displayed;
            }
        }

        public static void DownloadCourse()
        {
            var courseDir = Initialize();

            var sectionElementList = Driver.Instance.FindElements(By.CssSelector("div.modules section"));
            Console.WriteLine("Number of sections: {0}", sectionElementList.Count);

            for (var sectionIndex = 0; sectionIndex < sectionElementList.Count; sectionIndex++)
            {
                var sectionElement = sectionElementList[sectionIndex];
                DownloadSection(sectionElement, sectionIndex + 1, courseDir);
            }
        }

        private static string Initialize()
        {
            // Disable volume
            if (!IsMuted)
                VolumeButton.Click();

            ExpandAllSections();

            var currentCourseTitle = Driver.Instance.FindElement(By.TagName("h1")).Text;
            Console.WriteLine("Downloading course: {0}", currentCourseTitle);

            // Create a dir for the current course
            var courseDir = CourseDownloader.CreateDir(PluralsaverSettings.Path, currentCourseTitle);
            Console.WriteLine("Into {0}", courseDir);

            return courseDir;
        }

        static void ExpandAllSections()
        {
            var sectionExpanders = Driver.Instance.FindElements(By.CssSelector("div.icon-row-expand"));
            foreach (var sectionExpander in sectionExpanders)
            {
                sectionExpander.Click();
            }
        }

        private static void DownloadSection(IWebElement sectionElement, int sectionIndex, string courseDir)
        {
            var sectionTitle = sectionElement.FindElement(By.TagName("h2")).Text;

            // Create a dir for the current section
            var sectionTitleWithIndex = String.Format("{0:D2}. {1}", sectionIndex, sectionTitle);
            var sectionFullPath = CourseDownloader.CreateDir(courseDir, sectionTitleWithIndex);
            Console.WriteLine("    Downloading section {0}: {1}", sectionTitleWithIndex, sectionFullPath);

            var clipElements = sectionElement.FindElements(By.CssSelector("ul li"));
            for (var i = 0; i < clipElements.Count; i++)
            {
                DownloadClip(clipElements[i], i + 1, sectionFullPath);
            }
        }

        private static void DownloadClip(IWebElement clipElement, int clipIndex, string sectionFullPath)
        {
            var clipLinkElement = clipElement.FindElement(By.TagName("h3"));
            var clipTitle = string.Format("{0:D2}. {1}", clipIndex, clipLinkElement.Text);
            Console.Write("        Downloading clip: {0}", clipTitle);

            var clipLocalPath = string.Format(@"{0}\{1}", sectionFullPath,
                CourseDownloader.RemoveInvalidCharacters(clipTitle));

            // If the clip file exists locally, skip to the next one
            // NB: we may have some clips downloaded, e.g. previous download failed for some reason
            // NB #2: at this point we don't know the video extension!
            if (CheckIfAlreadyDownloaded(sectionFullPath, CourseDownloader.RemoveInvalidCharacters(clipTitle)))
            {
                Console.WriteLine("\n        Clip already exists: {0}", clipLocalPath);
            }
            else
            {
                // Click on the clip and switch to the new window
                //clipLinkElement.Click();
                //Driver.Instance.SwitchTo().Window(Driver.Instance.WindowHandles.Last());

                //var clipUrl = PlayerPage.GetCurrentClipUrl();
                //var clipFullPath = string.Format(@"{0}\{1}{2}", sectionFullPath,
                //    CourseDownloader.RemoveInvalidCharacters(clipTitle),
                //    Path.GetExtension(clipUrl.AbsolutePath));

                //using (var webClient = new WebClient())
                //{
                //    webClient.DownloadFile(clipUrl, clipFullPath);
                //}

                //Driver.Instance.Close();
                //Driver.Instance.SwitchTo().Window(Driver.Instance.WindowHandles.FirstOrDefault());

                //// Again, we want to mimic a human so give it a little timeout after the section
                //Driver.WaitSeconds(PluralsaverSettings.AfterClipTimeout);
            }
        }

        private static bool CheckIfAlreadyDownloaded(string path, string extensionlessFileName)
        {
            return Directory.GetFiles(path, extensionlessFileName + "*").Length > 0;
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
            Driver.WaitSeconds(1);
            Console.WriteLine(" ({0})", ClipDurationSpan.Text);

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
