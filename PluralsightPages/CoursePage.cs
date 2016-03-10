using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

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

        public static void Download()
        {
            Initialize();
            CloseHeader();

            var currentCourseTitle = Driver.Instance.FindElement(By.CssSelector("h1.course-hero__title")).Text;

            Console.WriteLine("Downloading course: {0}", currentCourseTitle);
            
            // Create a dir for the current course
            var courseDir = CourseDownloader.CreateDir(PluralsaverSettings.Path, currentCourseTitle);
            Console.WriteLine("Into {0}", courseDir);


            var sectionElementList = Driver.Instance.FindElements(By.CssSelector("li.accordian__section"));
            Console.WriteLine("Number of sections: {0}", sectionElementList.Count);

            for (var sectionIndex = 0; sectionIndex < sectionElementList.Count; sectionIndex++)
            {
                var sectionElement = sectionElementList[sectionIndex];
                DownloadSection(sectionElement, sectionIndex + 1, courseDir);
            }
        }

        private static void DownloadSection(IWebElement sectionElement, int sectionIndex, string courseDir)
        {
            var sectionTitle = sectionElement.FindElement(By.CssSelector("h3.table-of-contents__title a")).Text;

            // Create a dir for the current section
            var sectionTitleWithIndex = String.Format("{0:D2}. {1}", sectionIndex, sectionTitle);
            var sectionFullPath = CourseDownloader.CreateDir(courseDir, sectionTitleWithIndex);
            Console.WriteLine("    Downloading section {0}: {1}", sectionTitleWithIndex, sectionFullPath);

            var clipElements = sectionElement.FindElements(By.CssSelector("div.accordian__content ul li"));
            for (var i = 0; i < clipElements.Count; i++)
            {
                DownloadClip(clipElements[i], i + 1, sectionFullPath);
            }
        }

        private static void DownloadClip(IWebElement clipElement, int clipIndex, string sectionFullPath)
        {
            var clipLinkElement = clipElement.FindElement(By.TagName("a"));
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
                clipLinkElement.Click();
                Driver.Instance.SwitchTo().Window(Driver.Instance.WindowHandles.Last());

                var clipUrl = PlayerPage.GetCurrentClipUrl();
                var clipFullPath = string.Format(@"{0}\{1}{2}", sectionFullPath,
                    CourseDownloader.RemoveInvalidCharacters(clipTitle),
                    Path.GetExtension(clipUrl.AbsolutePath));

                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(clipUrl, clipFullPath);
                }

                Driver.Instance.Close();
                Driver.Instance.SwitchTo().Window(Driver.Instance.WindowHandles.FirstOrDefault());

                // Again, we want to mimic a human so give it a little timeout after the section
                Driver.WaitSeconds(PluralsaverSettings.AfterClipTimeout);
            }
        }

        private static bool CheckIfAlreadyDownloaded(string path, string extensionlessFileName)
        {
            return Directory.GetFiles(path, extensionlessFileName + "*").Length > 0;
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
