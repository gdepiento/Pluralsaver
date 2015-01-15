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
            var courseDownloader = new CourseDownloader();

            courseDownloader.Initialize();
        }
    }
}
