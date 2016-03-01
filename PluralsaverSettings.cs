using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Pluralsaver
{
    public class PluralsaverSettings
    {
        private const string InitializingErrorText = "An error occured while parsing PluralsightSettings.config file: ";
        private static XElement _settings;
        private static XElement _configuration;
        private static XElement _pluralsightAccout;
        private static XElement _download;
        private static XElement _downloadDelay;
        private static XElement _coursesToDownload;

        public static string Login
        {
            get
            {
                string login;
                try
                {
                    login = _pluralsightAccout.Attribute("Login").Value;
                }
                catch
                {
                    throw new Exception(InitializingErrorText + "Login attribute in PluralsightAccount is missing!");
                }

                if (login.Length == 0)
                    throw new Exception(InitializingErrorText + "Login attribute is empty!");
                return login;
            }
        }

        public static string Password
        {
            get
            {
                string password;
                try
                {
                    password = _pluralsightAccout.Attribute("Password").Value;
                }
                catch
                {
                    throw new Exception(InitializingErrorText + "Password attribute in PluralsightAccount is missing!");
                }

                if (password.Length == 0)
                    throw new Exception(InitializingErrorText + "Password attribute is empty!");
                return password;
            }
        }

        public static string Browser
        {
            get
            {
                string browserName;
                try
                {
                    browserName = _download.Attribute("Browser").Value;
                }
                catch
                {
                    throw new Exception(InitializingErrorText + "Browser attribute in Download element is missing!");
                }

                if ((browserName == "Chrome") || (browserName == "Firefox"))
                    return browserName;
                else throw new Exception(InitializingErrorText + "Browser attribute in Download element is incorrect. Supported values are: Chrome, Firefox");
            }
        }

        public static string Path
        {
            get
            {
                DirectoryInfo path;
                try
                {
                    path = new DirectoryInfo(_download.Attribute("Path").Value);
                }
                catch
                {
                    throw new Exception(InitializingErrorText + "Path attribute in Download element is missing!");
                }

                if (path.Exists)
                    return path.ToString();
                else throw new Exception("Download Path does not exist");
            }
        }

        public static int PlayClipTimeout
        {
            get
            {
                int playClipTimeoutSeconds;

                try
                {
                    var playClipTimeout = _downloadDelay.Attribute("PlayClipTimeout").Value;
                    playClipTimeoutSeconds = int.Parse(playClipTimeout);
                }
                catch (NullReferenceException)
                {
                    throw new Exception(InitializingErrorText +
                                        "PlayClipTimeout attribute in DownloadDelay element is missing!\n");
                }
                catch (FormatException)
                {
                    throw new Exception(InitializingErrorText + "PlayClipTimeout attribute contains a non-numeric value");
                }
                return playClipTimeoutSeconds;
            }
        }

        public static int AfterClipTimeout
        {
            get
            {
                int afterClipTimeoutSeconds;

                try
                {
                    var playClipTimeout = _downloadDelay.Attribute("AfterClipTimeout").Value;
                    afterClipTimeoutSeconds = int.Parse(playClipTimeout);
                }
                catch (NullReferenceException)
                {
                    throw new Exception(InitializingErrorText +
                                        "AfterClipTimeout attribute in DownloadDelay element is missing!\n");
                }
                catch (FormatException)
                {
                    throw new Exception(InitializingErrorText + "AfterClipTimeout attribute contains a non-numeric value");
                }
                return afterClipTimeoutSeconds;
            }
        }

        public static List<string> CoursesToDownload
        {
            get
            {
                return _coursesToDownload.XPathSelectElements("Course").Select(c => c.Value).ToList();
            }
        }

        public static void InitializeSettings()
        {
            Console.WriteLine("Initializing settings...");
            _settings = XElement.Load("PluralsaverSettings.config");
            
            _configuration = _settings.XPathSelectElement("//Configuration");
            _pluralsightAccout = _configuration.XPathSelectElement("PluralsightAccount");
            _download = _configuration.XPathSelectElement("Download");
            _downloadDelay = _configuration.XPathSelectElement("DownloadDelay");
            _coursesToDownload = _settings.XPathSelectElement("CoursesToDownload");

            Console.WriteLine("* Pluralsight Account Login    : {0}", Login);
            Console.WriteLine("* Pluralsight Account Password : {0}", new String('*', Password.Length));
            Console.WriteLine("* Download Path                : {0}", Path);
            Console.WriteLine("* Play Clip Timeout            : {0}", PlayClipTimeout);
            Console.WriteLine("* After Clip Timeout           : {0}", AfterClipTimeout);
            Console.WriteLine("* Browser to Use               : {0}", Browser);

            CourseDownloader.ShowCourseList();
        }
    }
}