using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Shrinerain.AutoTester.Function
{
    public class TestLog
    {

        #region fields

        //application information
        protected string _appInfo;
        protected string _programmer;
        protected string _version;

        //tester information
        protected string _companyInfo;
        protected string _projectTeamInfo;
        protected string _projectDepInfo;
        protected string _managerInfo;
        protected string _testerInfo;
        protected string _supporterInfo;



        //project information
        protected string _projectName;
        protected string _projectDes;
        protected string _projectURL;
        protected string _projectVersion;
        protected string _projectDriveFile;
        protected string _projectConfigFile;

        //environment information
        protected string _enviroment;
        protected string _hardwareInfo;
        protected string _softwareInfo;
        protected string _envAdmin;

        //test case information
        protected string _testPhases;
        protected string _testRequriment;
        protected string _testCaseDoc;
        protected string _testlogTemplate;
        protected string _requrimentDoc;
        protected string _designDoc;
        protected string _functionSpecificationDoc;
        protected string _maintenanceDoc;
        protected string _signOffDoc;

        //test step information, these informations should be repeated.
        protected string _testStep;
        protected string _control;
        protected string _url;
        protected string _action;
        protected string _properties;
        protected DateTime _currentTime;
        protected string _vpProperty;
        protected string _expectResult;
        protected string _actualResult;
        protected bool _isPass;
        protected string _testResultInfo;
        protected string _imgPath;
        protected string _imgName;
        protected string _code;
        protected string _exception;

        #endregion

        #region properties

        public string AppInfo
        {
            get { return _appInfo; }
            set { _appInfo = value; }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public string Programmer
        {
            get { return _programmer; }
            set { _programmer = value; }
        }

        public string CompanyInfo
        {
            get { return _companyInfo; }
            set { _companyInfo = value; }
        }

        public string SupporterInfo
        {
            get { return _supporterInfo; }
            set { _supporterInfo = value; }
        }
        public string ProjectTeamInfo
        {
            get { return _projectTeamInfo; }
            set { _projectTeamInfo = value; }
        }

        public string ProjectDepInfo
        {
            get { return _projectDepInfo; }
            set { _projectDepInfo = value; }
        }

        public string ManagerInfo
        {
            get { return _managerInfo; }
            set { _managerInfo = value; }
        }

        public string TesterInfo
        {
            get { return _testerInfo; }
            set { _testerInfo = value; }
        }

        #endregion

        #region methods

        #region ctor

        public TestLog()
        {

        }

        public TestLog(string template)
        {
            if (!string.IsNullOrEmpty(template))
            {
                this._testlogTemplate = template;
            }
        }

        #endregion

        #region public methods

        #endregion

        #region private methods

        protected bool IsValidTemplate(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            if (!fileName.ToUpper().EndsWith(".TEMPLATE"))
            {
                return false;
            }

            return true;
        }

        protected string GetTemplateString(string fileName)
        {
            try
            {
                return File.ReadAllText(fileName, Encoding.Default);
            }
            catch (Exception e)
            {
                throw new InvalidTemplateException("Can not read template: " + e.Message);
            }

        }



        #endregion

        #endregion
    }
}
