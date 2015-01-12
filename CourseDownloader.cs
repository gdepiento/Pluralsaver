using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int userCourse;
            // Parse user input to figure out what course to download
            if (int.TryParse(userInput, out userCourse))
            {
                if (userCourse == 0)
                {
                    // Download all courses
                    Console.WriteLine("You have opted for downloading all {0} courses", _settings.CoursesToDownload.Count);
                    DownloadAllCourses();
                }
                else if ((userCourse > 0) && (userCourse <= _settings.CoursesToDownload.Count))
                {
                    // Download a specific course
                    DownloadCourse(userCourse);
                }
            }

            // Exit the program
            Environment.Exit(0);
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

        private void DownloadCourse(int userCourse)
        {
            Console.WriteLine("\n-------------------------------");
            // We need to subtract 1 from user input number to get the correct index as it null-based 
            var courseTitle = _settings.CoursesToDownload[userCourse - 1];
            Console.WriteLine("Downloading course #{0}: {1}", userCourse, courseTitle);
        }
    }
}
