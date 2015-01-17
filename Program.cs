using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluralsaver
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var courseDownloader = new CourseDownloader();

                courseDownloader.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nA critical error occured in the application: " + ex.InnerException);                
            }
        }
    }
}
