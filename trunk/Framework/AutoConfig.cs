using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Windows.Forms;

using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.Framework
{

    public struct PluginInfo
    {
        public string _domain;

        public string _testBrowser;
        public string _testBrowserDLL;
        public string _testObjectPool;
        public string _testObjectPoolDLL;
        public string _testAction;
        public string _testActionDLL;
        public string _testVP;
        public string _testVPDLL;
    }

    public sealed class AutoConfig : IDisposable
    {
        #region fields

        private static AutoConfig _autoConfig;

        private const string _defaultFrameworkConfigFile = "Framework.config";
        private string _frameworkConfigFile = null;
        private string _projectConfigFile = null;

        private XmlTextReader _myFrameworkReader = null;
        private XmlTextReader _myProjectReader = null;

        private string _projectName;
        private string _projectDomain;
        private string _projectDriveFile;
        private string _logDir;
        private string _logTemplate;
        private string _screenprintDir;
        private bool _isHighlight;

        private List<PluginInfo> _plugList = new List<PluginInfo>(5);

        #endregion

        #region properties

        public string FrameworkConfigFile
        {
            get { return this._frameworkConfigFile; }
            set
            {
                if (value != null)
                {
                    this._frameworkConfigFile = value;
                }
            }
        }

        public string ProjectConfigFile
        {
            get { return this._projectConfigFile; }
            set
            {
                if (value != null)
                {
                    this._projectConfigFile = value;
                }
            }
        }

        public string ProjectName
        {
            get { return this._projectName; }
        }

        public string ProjectDomain
        {
            get { return this._projectDomain; }
        }

        public string ProjectDriveFile
        {
            get { return this._projectDriveFile; }
        }

        public string LogDir
        {
            get { return this._logDir; }
        }

        public string LogTemplate
        {
            get { return this._logTemplate; }
        }

        public string ScreenPrintDir
        {
            get { return this._screenprintDir; }
        }

        public bool IsHighlight
        {
            get { return this._isHighlight; }
        }

        #endregion


        #region mthods

        #region ctor

        private AutoConfig()
        {
            GetFrameworkConfigFile(null, out _frameworkConfigFile);
        }

        ~AutoConfig()
        {
            Dispose();
        }


        #endregion

        #region public methods

        public static AutoConfig GetInstance()
        {
            if (_autoConfig == null)
            {
                _autoConfig = new AutoConfig();
            }

            return _autoConfig;
        }

        public void ParseConfigFile()
        {
            if (this._frameworkConfigFile == null || this._projectConfigFile == null)
            {
                throw new ConfigFileNotFoundException("Config file can not be null.");
            }

            if (!File.Exists(this._frameworkConfigFile) || !File.Exists(this._projectConfigFile))
            {
                throw new ConfigFileNotFoundException("Can not find config file:" + this._frameworkConfigFile + " and/or " + this._projectConfigFile);
            }

            try
            {
                _myFrameworkReader = new XmlTextReader(this._frameworkConfigFile);
                _myFrameworkReader.WhitespaceHandling = WhitespaceHandling.None;
                _myProjectReader = new XmlTextReader(this._projectConfigFile);
                _myProjectReader.WhitespaceHandling = WhitespaceHandling.None;
            }
            catch
            {
                throw new CanNotOpenConfigFileException();
            }

            try
            {
                ParseFrameworkConfigFile(this._frameworkConfigFile);
                ParseProjectConfigFile(this._projectConfigFile);
            }
            catch
            {
                throw;
            }
            finally
            {
                Close();
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_myFrameworkReader != null)
            {
                _myFrameworkReader.Close();
                _myFrameworkReader = null;
            }

            if (_myProjectReader != null)
            {
                _myProjectReader.Close();
                _myProjectReader = null;
            }

            GC.SuppressFinalize(this);
        }


        public PluginInfo GetTestPluginByDomain(string domain)
        {
            foreach (PluginInfo p in this._plugList)
            {
                if (p._domain.ToUpper() == domain.ToUpper())
                {
                    return p;
                }
            }

            throw new CanNotLoadDllException("Plugin not found.");
        }

        #endregion

        #region private methods

        private void ParseFrameworkConfigFile(string frameworkConfigFile)
        {
            if (String.IsNullOrEmpty(frameworkConfigFile) || !File.Exists(frameworkConfigFile))
            {
                throw new ConfigFileNotFoundException("Can not find framework config file:" + frameworkConfigFile);
            }

            _myFrameworkReader.Read();

            while (!_myFrameworkReader.EOF)
            {
                if (_myFrameworkReader.Name == "AutoTester" && !_myFrameworkReader.IsStartElement())
                {
                    break;
                }

                if (_myFrameworkReader.Name != "TestExtension")
                {
                    _myFrameworkReader.Read();
                }
                else
                {
                    try
                    {
                        PluginInfo tmp = new PluginInfo();

                        _myFrameworkReader.Read();

                        tmp._domain = _myFrameworkReader.ReadElementString("TestDomain");
                        tmp._testBrowser = _myFrameworkReader.GetAttribute("ClassName");
                        tmp._testBrowserDLL = _myFrameworkReader.ReadElementString("TestBrowser");
                        tmp._testObjectPool = _myFrameworkReader.GetAttribute("ClassName");
                        tmp._testObjectPoolDLL = _myFrameworkReader.ReadElementString("TestObjectPool");
                        tmp._testAction = _myFrameworkReader.GetAttribute("ClassName");
                        tmp._testActionDLL = _myFrameworkReader.ReadElementString("TestAction");
                        tmp._testVP = _myFrameworkReader.GetAttribute("ClassName");
                        tmp._testVPDLL = _myFrameworkReader.ReadElementString("TestVP");

                        _plugList.Add(tmp);

                        _myFrameworkReader.Read();

                        if (_myFrameworkReader.Name != "TestExtension")
                        {
                            break;
                        }
                    }
                    catch
                    {
                        throw new BadFormatConfigFileException("Invalid framework config file.");
                    }
                }

                continue;
            }

            _myFrameworkReader.Close();

        }

        private void ParseProjectConfigFile(string projectConfigFile)
        {
            if (String.IsNullOrEmpty(projectConfigFile) || !File.Exists(projectConfigFile))
            {
                throw new ConfigFileNotFoundException("Can not find project config file: " + projectConfigFile);
            }

            _myProjectReader.Read(); //<xml version>
            _myProjectReader.Read();  //<Project>
            _myProjectReader.Read(); //<ProjectName>
            try
            {
                this._projectName = _myProjectReader.ReadElementString("ProjectName");
                this._projectDomain = _myProjectReader.ReadElementString("ProjectDomain");
                this._projectDriveFile = _myProjectReader.ReadElementString("DriveFile");
                this._logDir = _myProjectReader.ReadElementString("Log");
                this._logTemplate = _myProjectReader.ReadElementString("LogTemplate");
                this._screenprintDir = _myProjectReader.ReadElementString("ScreenPrint");

                if (_myProjectReader.ReadElementString("HighLight").ToUpper() == "NO")
                {
                    this._isHighlight = false;
                }
                else
                {
                    this._isHighlight = true;
                }
            }
            catch
            {
                throw new BadFormatConfigFileException("Invalid Project config file.");
            }

            _myProjectReader.Close();

        }

        private bool GetFrameworkConfigFile(string dir, out string configFile)
        {
            configFile = null;

            if (String.IsNullOrEmpty(dir))
            {
                dir = Application.StartupPath;// Environment.CurrentDirectory;// Assembly.GetExecutingAssembly().Location;
                dir = Path.GetDirectoryName(dir);
            }

            string expectedFile = dir + "\\" + _defaultFrameworkConfigFile;

            if (File.Exists(expectedFile))
            {
                configFile = expectedFile;
                return true;
            }
            else
            {
                string[] dirs = Directory.GetDirectories(dir);
                foreach (string d in dirs)
                {
                    if (GetFrameworkConfigFile(d, out configFile))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        #endregion

        #endregion


    }
}
