using System;

namespace Pluralsaver
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PluralsaverSettings.InitializeSettings();

                var courseDownloader = new CourseDownloader();
                courseDownloader.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nA critical error occurred in the application: {0}\n" +
                    "Inner exception: {1}", ex.Message, ex.InnerException);                
            }
        }
    }
}
