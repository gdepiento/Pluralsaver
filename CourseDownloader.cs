using System;
using System.IO;
using System.Linq;
using Pluralsaver.PluralsightPages;

namespace Pluralsaver
{
    public class CourseDownloader
    {
        public void Initialize()
        {
            Console.WriteLine("\nPlease select a course number or type '0' to download all. Entering any other value will exit the program: ");
            
            var userInput = Console.ReadLine();
            int userCourseIndex;
            // Parse user input to figure out what course to download
            if (int.TryParse(userInput, out userCourseIndex))
            {
                if (userCourseIndex == 0)
                {
                    // Download all courses
                    Console.WriteLine("You have opted for downloading all {0} courses", PluralsaverSettings.CoursesToDownload.Count);
                    DownloadAllCourses();
                }
                else if ((userCourseIndex > 0) && (userCourseIndex <= PluralsaverSettings.CoursesToDownload.Count))
                {
                    // Download a specific course
                    DownloadCourse(userCourseIndex);
                }
            }
        }

        public static void ShowCourseList()
        {
            Console.WriteLine("\n* Courses list (from PluralsaverSettings.config file)");

            for (var i = 0; i < PluralsaverSettings.CoursesToDownload.Count; i++)
            {
                Console.WriteLine("    {0}. {1}", i+1, PluralsaverSettings.CoursesToDownload[i]);
            }
        }

        private void DownloadAllCourses()
        {
            // Run DownloadCourse() method for each course index
            for (var i = 1; i <= PluralsaverSettings.CoursesToDownload.Count; i++)
            {
                DownloadCourse(i);
            }
        }

        public void DownloadCourse(int userCourseIndex)
        {
            Console.WriteLine("\n-------------------------------");
            // We need to subtract 1 from user input number to get the correct index as it null-based 
            var courseTitle = PluralsaverSettings.CoursesToDownload[userCourseIndex - 1];
            Console.WriteLine("Downloading course #{0}: {1}", userCourseIndex, courseTitle);

            OpenCourseInBrowser(courseTitle);

            CoursePage.Download();

            Console.WriteLine("Course download has been completed.");

            Driver.Close();
        }

        private void OpenCourseInBrowser(string courseTitle)
        {
            Driver.Initialize();

            HomePage.GoTo();
            HomePage.SignInLink.Click();

            LoginPage.Login(PluralsaverSettings.Login, PluralsaverSettings.Password);

            HomePage.OpenCourse(courseTitle);
        }

        public static string CreateDir(string parentDir, string dirName)
        {
            dirName = RemoveInvalidCharacters(dirName);
            var fullDirPath = String.Format("{0}\\{1}", parentDir, dirName);

            // Remove directory completely if already exitsts
            if (!Directory.Exists(fullDirPath))
                Directory.CreateDirectory(fullDirPath);

            return fullDirPath;
        }

        public static string RemoveInvalidCharacters(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            var invalidCharsRemovedArray = fileName
                .Where(x => !invalidChars.Contains(x))
                .ToArray();

            return new string(invalidCharsRemovedArray);
        }
    }
}
