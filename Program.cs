using System;

namespace Pluralsaver
{
    class Program
    {
        static void Main(string[] args)
        {
            //try
            //{
            PluralsaverSettings.InitializeSettings();

            var courseDownloader = new CourseDownloader();
            // If there is only one course in the config, don't ask user anything - just download
            if (PluralsaverSettings.CoursesToDownload.Count == 1)
                courseDownloader.DownloadCourse(1);
            else courseDownloader.Initialize();

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("\n\nA critical error occurred in the application: {0}\n" +
            //        "Inner exception: {1}", ex.Message, ex.InnerException);                
            //}
        }
    }
}
