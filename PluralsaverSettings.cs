using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Pluralsaver
{
    public class PluralsaverSettings
    {
        private static XElement _settings;
        private static XElement _configuration;
        private static XElement _pluralsightAccout;
        private static XElement _download;
        private static XElement _downloadDelay;
        private static XElement _coursesToDownload;
        
        public static string Login
        {
            get { return _pluralsightAccout.Attribute("Login").Value; }
        }

        public static string Password
        {
            get { return _pluralsightAccout.Attribute("Password").Value; }
        }

        public static string Path
        {
            get
            {
                var path = new DirectoryInfo(_download.Attribute("Path").Value);
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
                var playClipTimeout = _downloadDelay.Attribute("PlayClipTimeout").Value;

                try
                {
                    playClipTimeoutSeconds = int.Parse(playClipTimeout);
                }
                catch (FormatException)
                {
                    throw new Exception("Config File Error: PlayClipTimeout attribute contains a non-number value");
                }
                return playClipTimeoutSeconds;
            }
        }

        public static int AfterClipTimeout
        {
            get
            {
                int afterClipTimeoutSeconds;
                var afterClipTimeout = _downloadDelay.Attribute("AfterClipTimeout").Value;

                try
                {
                    afterClipTimeoutSeconds = int.Parse(afterClipTimeout);
                }
                catch (FormatException)
                {
                    throw new Exception("Config File Error: AfterClipTimeout attribute contains a non-number value");
                }
                return afterClipTimeoutSeconds;
            }
        }

        static PluralsaverSettings()
        {
            try
            {
                InitializeSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has occurred during initialization: " + ex.Message);
            }
        }

        public static List<string> CoursesToDownload
        {
            get
            {
                return _coursesToDownload.XPathSelectElements("Course").Select(c => c.Value).ToList();
            }
        }

        private static void InitializeSettings()
        {
            Console.WriteLine("Initializing settings...");
            _settings = XElement.Load("PluralsaverSettings.config");
            
            _configuration = _settings.XPathSelectElement("//Configuration");
            _pluralsightAccout = _configuration.XPathSelectElement("PluralsightAccount");
            _download = _configuration.XPathSelectElement("Download");
            _downloadDelay = _configuration.XPathSelectElement("DownloadDelay");
            _coursesToDownload = _settings.XPathSelectElement("CoursesToDownload");

            Console.WriteLine("* Pluralsight Account Login   : {0}", Login);
            Console.WriteLine("* Download Path               : {0}\n", Path);
        }
    }
}