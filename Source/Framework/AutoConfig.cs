/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: AutoConfig.cs
*
* Description: Framework will have two config file: Framework config file
*              and project config file.
*              Framework config file will defines the behavior of 
*              Automation Framework.
*              Project config file will provide the information of current
*              project. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Windows.Forms;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.Framework
{

    //for different technology, we have different library.
    //we load these library at runtime.
    //eg: for HTML, we will load HTMLTestObjectPool.

    public struct PluginInfo
    {
        //what type of technology, eg: HTML/Flex
        public string _domain;

        //actual dll we need to load.

        public string _testApp;
        public string _testAppDLL;
        public string _testBrowser;
        public string _testBrowserDLL;
        public string _testObjectPool;
        public string _testObjectPoolDLL;
        public string _testAction;
        public string _testActionDLL;
        public string _testVP;
        public string _testVPDLL;

        public override bool Equals(object obj)
        {
            if (obj is PluginInfo)
            {
                PluginInfo tmp = (PluginInfo)obj;

                if (tmp._domain == this._domain &&
                    tmp._testAction == this._testAction &&
                    tmp._testActionDLL == this._testActionDLL &&
                    tmp._testApp == this._testApp &&
                    tmp._testAppDLL == this._testAppDLL &&
                    tmp._testBrowser == this._testBrowser &&
                    tmp._testBrowserDLL == this._testBrowserDLL &&
                    tmp._testObjectPool == this._testObjectPool &&
                    tmp._testObjectPoolDLL == this._testObjectPoolDLL &&
                    tmp._testVP == this._testVP &&
                    tmp._testVPDLL == this._testVPDLL)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(PluginInfo p1, PluginInfo p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(PluginInfo p1, PluginInfo p2)
        {
            return !p1.Equals(p2);
        }

        public override int GetHashCode()
        {
            string buf = ToString();
            return buf.GetHashCode();
        }

        public override string ToString()
        {
            string str = _testApp + "\t"
                       + _testAppDLL + "\t"
                       + _testBrowser + "\t"
                       + _testBrowserDLL + "\t"
                       + _testObjectPool + "\t"
                       + _testObjectPoolDLL + "\t"
                       + _testAction + "\t"
                       + _testActionDLL + "\t"
                       + _testVP + "\t"
                       + _testVPDLL;
            return str;
        }
    }

    public sealed class AutoConfig : IDisposable
    {
        #region fields

        private static AutoConfig _autoConfig;

        private const string _defaultFrameworkConfigFile = "Framework.config";
        private string _frameworkConfigFile;
        private string _projectConfigFile;

        //config files are XML file, we use XMLTextReader to read them.
        private XmlTextReader _myFrameworkReader;
        private XmlTextReader _myProjectReader;

        //project information, loaded from project config file.
        private string _projectName;
        private string _projectDomain;
        private string _projectDriveFile;
        private string _logDir;
        private string _logTemplate;
        private string _screenprintDir;
        private bool _isHighlight;

        //List to store dll information
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

        /* void ParseConfigFile()
         * Check the two config file, if user didn't pass a framework config, then search it in current folder.
         * Then call two methods to parse two config file separately.
         */
        public void ParseConfigFile()
        {
            //if user didn't input a framework config file.
            if (String.IsNullOrEmpty(this._frameworkConfigFile))
            {
                // if no framework config file found, throw
                if (!GetFrameworkConfigFile(null, out _frameworkConfigFile))
                {
                    throw new ConfigFileNotFoundException("Can not find framework config file.");
                }
            }

            if (String.IsNullOrEmpty(this._frameworkConfigFile) || String.IsNullOrEmpty(this._projectConfigFile))
            {
                throw new ConfigFileNotFoundException("Config file can not be null.");
            }

            //if two files are not exist, throw.
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
            catch (Exception ex)
            {
                throw new CannotOpenConfigFileException(ex.ToString());
            }

            try
            {
                //parse two config files seprately.
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

        /* void Dispose()
         * GC, close XML reader.
         */
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

        /* PluginInfo GetTestPluginByDomain(string domain)
         * return the dll information by domain(technology).
         */
        public PluginInfo GetTestPluginByDomain(string domain)
        {
            foreach (PluginInfo p in this._plugList)
            {
                if (p._domain.ToUpper() == domain.ToUpper())
                {
                    return p;
                }
            }

            throw new CannotLoadDllException("Plugin not found.");
        }

        #endregion

        #region private methods

        /* void ParseFrameworkConfigFile(string frameworkConfigFile)
         * Parse framework config file.
         * NEED UPDATE!!!
         */
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

                        tmp._testApp = _myFrameworkReader.GetAttribute("ClassName");
                        tmp._testAppDLL = _myFrameworkReader.ReadElementString("TestApp");

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

        /* void ParseProjectConfigFile(string projectConfigFile)
         * Parse project config file.
         */
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

        /* bool GetFrameworkConfigFile(string dir, out string configFile)
         * Search the default framework config file.
         * The default config file is named "Framework.config" and is placed in the subfolder.
         */
        private bool GetFrameworkConfigFile(string dir, out string configFile)
        {
            configFile = null;

            if (String.IsNullOrEmpty(dir))
            {
                dir = Application.StartupPath;
                if (dir.IndexOf("AutoTester") > 0)
                {
                    dir = dir.Substring(0, dir.IndexOf("AutoTester") + 11);
                }

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
