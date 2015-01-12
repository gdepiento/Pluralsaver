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
        private XElement _settings;
        private XElement _configuration;
        private XElement _pluralsightAccout;
        private XElement _download;
        private XElement _downloadDelay;
        private XElement _coursesToDownload;
        
        public string Login
        {
            get { return _pluralsightAccout.Attribute("Login").Value; }
        }

        public string Password
        {
            get { return _pluralsightAccout.Attribute("Password").Value; }
        }

        public string Path
        {
            get
            {
                var path = new DirectoryInfo(_download.Attribute("Path").Value);
                if (path.Exists)
                    return path.ToString();
                else throw new Exception("Download Path does not exist");
            }
        }

        public int PlayClip
        {
            get
            {
                int playClipSeconds;
                var playClip = _downloadDelay.Attribute("PlayClip").Value;

                try
                {
                    playClipSeconds = int.Parse(playClip);
                }
                catch (FormatException)
                {
                    throw new Exception("Config File Error: PlayClip attribute contains a non-number value");
                }
                return playClipSeconds;
            }
        }

        public int AfterClipTimeout
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
        
        public PluralsaverSettings()
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

        public List<string> CoursesToDownload
        {
            get
            {
                return _coursesToDownload.XPathSelectElements("Course").Select(c => c.Value).ToList();
            }
        }

        private void InitializeSettings()
        {
            Console.WriteLine("Initializing settings...");
            _settings = XElement.Load("PluralsaverSettings.config");
            
            _configuration = _settings.XPathSelectElement("//Configuration");
            _pluralsightAccout = _configuration.XPathSelectElement("PluralsightAccount");
            _download = _configuration.XPathSelectElement("Download");
            _downloadDelay = _configuration.XPathSelectElement("DownloadDelay");
            _coursesToDownload = _settings.XPathSelectElement("CoursesToDownload");

            Console.WriteLine("* Pluralsight Account Login   : " + AfterClipTimeout);
            Console.WriteLine("* Download Path               : " + Path);
        }
    }
}