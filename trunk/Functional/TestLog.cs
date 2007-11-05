using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Shrinerain.AutoTester.Function
{
    public class TestLog
    {

        #region fields

        //buffer to save the template 
        protected StringBuilder _templateBuf;

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
        public string Programmer
        {
            get { return _programmer; }
            set { _programmer = value; }
        }
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }
        public string CompanyInfo
        {
            get { return _companyInfo; }
            set { _companyInfo = value; }
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
        public string SupporterInfo
        {
            get { return _supporterInfo; }
            set { _supporterInfo = value; }
        }
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; }
        }
        public string ProjectDes
        {
            get { return _projectDes; }
            set { _projectDes = value; }
        }
        public string ProjectURL
        {
            get { return _projectURL; }
            set { _projectURL = value; }
        }
        public string ProjectVersion
        {
            get { return _projectVersion; }
            set { _projectVersion = value; }
        }
        public string ProjectDriveFile
        {
            get { return _projectDriveFile; }
            set { _projectDriveFile = value; }
        }
        public string ProjectConfigFile
        {
            get { return _projectConfigFile; }
            set { _projectConfigFile = value; }
        }
        public string Enviroment
        {
            get { return _enviroment; }
            set { _enviroment = value; }
        }
        public string HardwareInfo
        {
            get { return _hardwareInfo; }
            set { _hardwareInfo = value; }
        }
        public string SoftwareInfo
        {
            get { return _softwareInfo; }
            set { _softwareInfo = value; }
        }
        public string EnvAdmin
        {
            get { return _envAdmin; }
            set { _envAdmin = value; }
        }
        public string TestPhases
        {
            get { return _testPhases; }
            set { _testPhases = value; }
        }
        public string TestRequriment
        {
            get { return _testRequriment; }
            set { _testRequriment = value; }
        }
        public string TestCaseDoc
        {
            get { return _testCaseDoc; }
            set { _testCaseDoc = value; }
        }
        public string TestlogTemplate
        {
            get { return _testlogTemplate; }
            set { _testlogTemplate = value; }
        }
        public string RequrimentDoc
        {
            get { return _requrimentDoc; }
            set { _requrimentDoc = value; }
        }
        public string DesignDoc
        {
            get { return _designDoc; }
            set { _designDoc = value; }
        }
        public string FunctionSpecificationDoc
        {
            get { return _functionSpecificationDoc; }
            set { _functionSpecificationDoc = value; }
        }
        public string MaintenanceDoc
        {
            get { return _maintenanceDoc; }
            set { _maintenanceDoc = value; }
        }
        public string SignOffDoc
        {
            get { return _signOffDoc; }
            set { _signOffDoc = value; }
        }
        public string TestStep
        {
            get { return _testStep; }
            set { _testStep = value; }
        }
        public string Control
        {
            get { return _control; }
            set { _control = value; }
        }
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }
        public string Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }
        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set { _currentTime = value; }
        }
        public string VpProperty
        {
            get { return _vpProperty; }
            set { _vpProperty = value; }
        }
        public string ExpectResult
        {
            get { return _expectResult; }
            set { _expectResult = value; }
        }
        public string ActualResult
        {
            get { return _actualResult; }
            set { _actualResult = value; }
        }
        public bool IsPass
        {
            get { return _isPass; }
            set { _isPass = value; }
        }
        public string TestResultInfo
        {
            get { return _testResultInfo; }
            set { _testResultInfo = value; }
        }
        public string ImgPath
        {
            get { return _imgPath; }
            set { _imgPath = value; }
        }
        public string ImgName
        {
            get { return _imgName; }
            set { _imgName = value; }
        }
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        public string Exception
        {
            get { return _exception; }
            set { _exception = value; }
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

        public void WriteLog()
        {

        }

        public void WriteInfo(string info)
        {

        }

        public void WriteError(string error)
        {

        }

        public void WriteWarning(string warning)
        {

        }

        #endregion

        #region private methods

        protected void LoadTemplate(string fileName)
        {
            if (IsValidTemplate(fileName))
            {
                string tmpStr = GetTemplateString(fileName);
                if (!String.IsNullOrEmpty(tmpStr))
                {
                    this._templateBuf = new StringBuilder(tmpStr.Length * 2);
                    this._templateBuf.Append(tmpStr);
                }

            }
        }

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
