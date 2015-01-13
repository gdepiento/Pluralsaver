using System;
using System.IO;
using System.Linq;
using Pluralsaver.PluralsightPages;

namespace Pluralsaver
{
    public class CourseDownloader
    {
        private readonly PluralsaverSettings _settings;
        public CourseDownloader(PluralsaverSettings settings)
        {
            _settings = settings;
        }
        
        public void Initialize()
        {
            ShowCourseList();
            Console.WriteLine("\nPlease select a course number or type '0' to download all. Entering any other value will exit the program: ");
            
            var userInput = Console.ReadLine();
            int userCourseIndex;
            // Parse user input to figure out what course to download
            if (int.TryParse(userInput, out userCourseIndex))
            {
                if (userCourseIndex == 0)
                {
                    // Download all courses
                    Console.WriteLine("You have opted for downloading all {0} courses", _settings.CoursesToDownload.Count);
                    DownloadAllCourses();
                }
                else if ((userCourseIndex > 0) && (userCourseIndex <= _settings.CoursesToDownload.Count))
                {
                    // Download a specific course
                    DownloadCourse(userCourseIndex);
                }
            }

            // Exit the program
            //Environment.Exit(0);
        }

        private void ShowCourseList()
        {
            Console.WriteLine("\nSettings file contains the following course titles available to download:");

            for (var i = 0; i < _settings.CoursesToDownload.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i+1, _settings.CoursesToDownload[i]);
            }
        }

        private void DownloadAllCourses()
        {
            // Run DownloadCourse() method for each course index
            for (int i = 1; i <= _settings.CoursesToDownload.Count; i++)
            {
                DownloadCourse(i);
            }
        }

        private void DownloadCourse(int userCourseIndex)
        {
            Console.WriteLine("\n-------------------------------");
            // We need to subtract 1 from user input number to get the correct index as it null-based 
            var courseTitle = _settings.CoursesToDownload[userCourseIndex - 1];
            Console.WriteLine("Downloading course #{0}: {1}", userCourseIndex, courseTitle);

            OpenCourseInBrowser(courseTitle);

            CoursePage.Download(_settings);

            Console.WriteLine("Course download has been completed.");

            Driver.Close();
        }

        private void OpenCourseInBrowser(string courseTitle)
        {
            Driver.Initialize();

            HomePage.GoTo();
            HomePage.SignInButton.Click();

            LoginPage.Login(_settings.Login, _settings.Password);

            HomePage.OpenCourse(courseTitle);

        }

        public static string CreateDir(string parentDir, string dirName)
        {
            dirName = RemoveFilenameInvalidCharacters(dirName);
            var fullDirPath = String.Format("{0}\\{1}", parentDir, dirName);

            // Remove directory completely if already exitsts
            if (Directory.Exists(fullDirPath))
                Directory.Delete(fullDirPath, true);

            Directory.CreateDirectory(fullDirPath);
            return fullDirPath;
        }

        public static string RemoveFilenameInvalidCharacters(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            var invalidCharsRemovedArray = fileName
                .Where(x => !invalidChars.Contains(x))
                .ToArray();

            return new string(invalidCharsRemovedArray);
        }
    }
}
