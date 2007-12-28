/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestLog.cs
*
* Description: TestLog provide log to end users.
*              TestLog recieve a template file. The template is divided 
*              to 3 parts. Head, Body, Tail. 
*              At runtime, Testlog will replace the keywords with actual
*              value, generate the log.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Drawing;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core
{
    public class TestLog : IDisposable
    {

        #region fields

        //log writer
        protected TextWriter _logWriter;
        protected string _logFile;

        //buffer to save the template 
        protected StringBuilder _templateHeadBuf;
        protected StringBuilder _templateBodyBuf;
        protected StringBuilder _templateTailBuf;
        protected StringBuilder _templateBodyOriginBuf;

        //type for reflecting to get the property value.
        protected Type _type;
        protected object _testLogObject;

        //this reg is used to extract the keyword from template string, eg: <%PROJECTNAME%>, extract "projectname"
        protected static Regex _keywordReg;

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
        protected string _userData; //information added by user. like Loginfo in Function Tester

        #endregion

        #region properties

        public string LogFile
        {
            get { return _logFile; }
            set { _logFile = value; }
        }


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
        public string UserData
        {
            get { return _userData; }
            set { _userData = value; }
        }

        #endregion

        #region methods

        #region ctor

        public TestLog()
        {
            Init();
        }

        public TestLog(string template)
        {
            if (!string.IsNullOrEmpty(template))
            {
                this._testlogTemplate = template;
            }

            Init();
        }

        ~TestLog()
        {
            Dispose();
        }


        #endregion

        #region public methods

        /* void Dispose()
         * when GC, close the StreamWriter.
         */
        public virtual void Dispose()
        {
            if (this._logWriter != null)
            {
                WriteTail();

                this._logWriter.Close();
                this._logWriter = null;
            }

            GC.SuppressFinalize(this);
        }

        public virtual void Close()
        {
            Dispose();
        }

        /* void WriteLog()
         * Flush buffer string to file.
         */
        public void WriteLog()
        {
            if (this._logWriter == null)
            {
                try
                {
                    //load log template file to buffer.
                    LoadTemplate(this._testlogTemplate);

                    if (File.Exists(this._logFile))
                    {
                        File.Delete(this._logFile);
                    }

                    this._logWriter = File.CreateText(this._logFile);

                    // write template head
                    WriteHead();
                }
                catch (Exception e)
                {
                    throw new CannotWriteLogException(e.Message);
                }
            }

            //write template body
            WriteBody();

        }

        /* void WriteInfo(string info)
         * These 3 methods is for customers, like XDE Tester.
         */
        public void WriteInfo(string info)
        {

        }

        public void WriteError(string error)
        {

        }

        public void WriteWarning(string warning)
        {

        }

        #region save screen print

        /* void SaveScreenPrint(string filePath, string fileName)
         * Save current screen print to a local jpg file.
         */
        public void SaveScreenPrint(string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
            {
                throw new CannotSaveScreenPrintException("File path and file name can not be empty.");
            }

            if (!Directory.Exists(filePath))
            {
                throw new CannotSaveScreenPrintException("Folder not found: " + filePath);
            }

            string fullName = filePath + fileName;
            fileName.Replace(@"\\\", @"\\");

            SaveScreenPrint(fullName);

        }

        public void SaveScreenPrint(string fileFullName)
        {
            try
            {
                Image img = CaptureScreen();
                img.Save(fileFullName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (CannotSaveScreenPrintException)
            {
                throw;
            }
            catch (IOException e)
            {
                throw new CannotSaveScreenPrintException("Can not save screen print file: " + fileFullName + "," + e.Message);
            }
        }

        #endregion

        #endregion

        #region private methods

        protected virtual void Init()
        {
            _keywordReg = new Regex("(?<=<%)[a-zA-Z]+(?=%>)", RegexOptions.Compiled);

            _type = this.GetType();

        }


        #region write log
        /* void WriteHead()
         * I divide the test log to three parts, head, body and tail.
         * the three part are surround by <%HEAD%> , <%BODY%> and <%TAIL%>
         * Head and tail are the information for whole project, they just be writen once in a log.
         * body means the repeated information about the testing, for example: test step may change rapidly, 
         * we may have lots of steps to log. they are repeated, we need to write the log with the same format but different
         * data.
         */
        protected void WriteHead()
        {
            MatchCollection headKeywords = _keywordReg.Matches(this._templateHeadBuf.ToString());

            foreach (Match m in headKeywords)
            {
                SetTemplateValueByName(this._templateHeadBuf, m.Value);
            }

            try
            {
                ClearLeftKeywords(_templateHeadBuf);

                this._logWriter.WriteLine(this._templateHeadBuf.ToString());
                this._logWriter.Flush();
            }
            catch (Exception e)
            {
                throw new CannotWriteLogException(e.Message);
            }

        }

        protected void WriteBody()
        {
            this._templateBodyBuf = new StringBuilder(_templateBodyOriginBuf.ToString());

            MatchCollection bodyKeywords = _keywordReg.Matches(this._templateBodyBuf.ToString());

            foreach (Match m in bodyKeywords)
            {
                SetTemplateValueByName(this._templateBodyBuf, m.Value);
            }

            try
            {
                ClearLeftKeywords(_templateBodyBuf);

                this._logWriter.WriteLine(this._templateBodyBuf.ToString());
                this._logWriter.Flush();
            }
            catch (Exception e)
            {
                throw new CannotWriteLogException(e.Message);
            }
        }

        protected void WriteTail()
        {
            MatchCollection tailKeywords = _keywordReg.Matches(this._templateTailBuf.ToString());

            foreach (Match m in tailKeywords)
            {
                SetTemplateValueByName(this._templateTailBuf, m.Value);
            }

            try
            {
                ClearLeftKeywords(_templateTailBuf);

                this._logWriter.WriteLine(this._templateTailBuf.ToString());
                this._logWriter.Flush();
            }
            catch (Exception e)
            {
                throw new CannotWriteLogException(e.Message);
            }
        }

        #endregion


        #region parse template

        /* bool IsValidTemplate(string fileName)
         * return true if the input file is a valid template.
         * 
         */
        protected bool IsValidTemplate(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            //check if the extension is ".template"
            if (!fileName.ToUpper().EndsWith(".TEMPLATE"))
            {
                return false;
            }

            return true;
        }

        /* void LoadTemplate(string fileName)
         * Load the file content to buffer.
         * we have 3 buffers, HEAD, BODY, and TAIL
         */
        protected void LoadTemplate(string fileName)
        {
            if (IsValidTemplate(fileName))
            {
                string tmpStr = File.ReadAllText(fileName, Encoding.Default);

                if (!String.IsNullOrEmpty(tmpStr))
                {
                    _templateHeadBuf = GetTemplatePart(tmpStr, "HEAD");

                    _templateBodyOriginBuf = GetTemplatePart(tmpStr, "BODY");

                    _templateTailBuf = GetTemplatePart(tmpStr, "TAIL");

                }
                else
                {
                    throw new CannotOpenTemplateException("Can not read template: " + fileName);
                }

            }
        }

        /* StringBuilder GetTemplatePart(string templateBuf, string flag)
         * Load template to buffer
         */
        protected StringBuilder GetTemplatePart(string templateBuf, string flag)
        {
            string startFlag = "<%" + flag.ToUpper() + "%>";
            string endFlag = "<%/" + flag.ToUpper() + "%>";

            if (templateBuf.IndexOf(startFlag) < 0 || templateBuf.IndexOf(endFlag) < 0)
            {
                throw new InvalidTemplateException("Can not locate " + flag + " in " + templateBuf);
            }
            else
            {
                if (templateBuf.IndexOf(endFlag) < templateBuf.IndexOf(startFlag))
                {
                    throw new InvalidTemplateException(endFlag + " should be later than " + startFlag);
                }
            }

            int pos1 = templateBuf.IndexOf(startFlag);
            int pos2 = templateBuf.IndexOf(endFlag);

            return new StringBuilder(templateBuf.Substring(pos1 + 8, pos2 - pos1 - 8));
        }

        #endregion

        /*  void SetTemplateValueByName(StringBuilder templateBuf, string propertyName)
         *  Replace the keyword in template file with actual value.
         */
        protected void SetTemplateValueByName(StringBuilder templateBuf, string propertyName)
        {
            string value = GetFieldValueByName(propertyName);

            if (String.IsNullOrEmpty(value))
            {
                value = "&nbsp;";
                // throw new CannotWriteLogException("Can not find property: " + propertyName);
            }

            try
            {
                templateBuf.Replace("<%" + propertyName + "%>", value);
            }
            catch (Exception e)
            {
                throw new CannotWriteLogException("Can not write property+ " + propertyName + " : " + e.Message);
            }

        }

        /* string GetFieldValueByName(string propertyName)
         * Get property value, use reflecting.
         */
        protected string GetFieldValueByName(string propertyName)
        {

            try
            {
                return (string)_type.InvokeMember(propertyName, BindingFlags.GetProperty, null, this, null);
            }
            catch
            {
                return null;
            }

        }

        /* void ClearLeftKeywords(StringBuilder templateBuf)
         * replace unused keyword to blank "&nbsp;" 
         */
        protected void ClearLeftKeywords(StringBuilder templateBuf)
        {
            if (templateBuf == null)
            {
                return;
            }

            MatchCollection leftKeywords = _keywordReg.Matches(templateBuf.ToString());

            foreach (Match m in leftKeywords)
            {
                templateBuf.Replace("<%" + m.Value + "%>", "&nbsp;");
            }

        }

        #region save screen print

        /* Image CaptureScreen()
         * return an image of screen.
         */
        protected Image CaptureScreen()
        {
            return CaptureWindow(Win32API.GetDesktopWindow());
        }

        /* Image CaptureWindow(IntPtr handle)
         * return an image of expected handle.
         */
        protected Image CaptureWindow(IntPtr handle)
        {
            try
            {
                // get te hDC of the target window
                IntPtr hdcSrc = Win32API.GetWindowDC(handle);
                // get the size
                Win32API.Rect windowRect = new Win32API.Rect();
                Win32API.GetWindowRect(handle, ref windowRect);
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;
                // create a device context we can copy to
                IntPtr hdcDest = Win32API.CreateCompatibleDC(hdcSrc);
                // create a bitmap we can copy it to,
                // using GetDeviceCaps to get the width/height
                IntPtr hBitmap = Win32API.CreateCompatibleBitmap(hdcSrc, width, height);
                // select the bitmap object
                IntPtr hOld = Win32API.SelectObject(hdcDest, hBitmap);
                // bitblt over
                Win32API.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, Win32API.SRCCOPY);
                // restore selection
                Win32API.SelectObject(hdcDest, hOld);
                // clean up 
                Win32API.DeleteDC(hdcDest);
                Win32API.ReleaseDC(handle, hdcSrc);

                // get a .NET image object for it
                Image img = Image.FromHbitmap(hBitmap);
                // free up the Bitmap object
                Win32API.DeleteObject(hBitmap);

                return img;

            }
            catch (Exception e)
            {
                throw new CannotSaveScreenPrintException(e.Message);
            }

        }
        #endregion

        #endregion

        #endregion
    }
}
