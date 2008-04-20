/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: CoreEngine.cs
*
* Description: CoreEngine contorl the framework running status, coordinate
*              other engines to work together.
*              It get config from AutoConfig, get test steps from parser,
*              call ObjectEngine to get test object, then pass test object
*              and actions/data to ActionEngine or VPEngine to perform actions
*              or check point, also, call LogEngine to write log, Exception
*              Engine to handle exceptions. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.Framework
{
    //save the sub status, before call other sub, we need to save current test steps to stack.
    internal struct TestStepStatus
    {
        internal int _index;
        internal List<TestStep> _stepList;
    };

    public sealed class CoreEngine
    {
        #region Fields


        //event to deliver message to other program.
        //eg: deliver the message to console, or to GUI.
        public delegate void _frameworkInfoDelegate(string message);
        public event _frameworkInfoDelegate OnNewMessage;


        private AutoConfig _autoConfig;
        private Parser _parser;

        //special interface, browser and desktop application.
        private ITestBrowser _testBrowser;
        private ITestApp _testApp;


        //other engine to perform each action.
        //data engine is managed by SubEngine.
        private ObjectEngine _objEngine;
        private ActionEngine _actEngine;
        private VPEngine _vpEngine;
        private LogEngine _logEngine;
        private SubEngine _subEngine;
        private ExceptionEngine _exEngine;

        //flag for start or not. If false, will not perform any action.
        private bool _started = false;

        //flag for end, if true, stop testing.
        private bool _end = false;

        //flag for highlight a test object, if true, will highlight a test object.
        private bool _isHighlight = false;

        //currently used test steps list. 
        private int _index = 0;  //actually, the line number start from 2 in the excel file.
        private List<TestStep> _currentTestSteps;

        //stack to store different test steps, then we can switch between main steps and subs.
        private Stack<TestStepStatus> _testStepStack = new Stack<TestStepStatus>(5);

        #endregion

        #region properties

        public AutoConfig AutoConfig
        {
            get { return _autoConfig; }
            set
            {
                if (value != null)
                {
                    _autoConfig = value;
                }
            }
        }

        public Parser KeywordParser
        {
            get { return _parser; }
            set
            {
                if (value != null)
                {
                    _parser = value;
                }
            }
        }

        public bool IsHighlight
        {
            get { return _isHighlight; }
            set { _isHighlight = value; }
        }

        #endregion

        #region methods

        #region ctor

        public CoreEngine()
        {
            this.OnNewMessage += new _frameworkInfoDelegate(WriteMsgToConsole);
        }

        public CoreEngine(AutoConfig config, Parser parser)
        {
            if (config == null || parser == null)
            {
                throw new ArgumentNullException("AutoConfig and/or KeywordParser can not be null.");
            }

            this._autoConfig = config;
            this._parser = parser;

            this.OnNewMessage += new _frameworkInfoDelegate(WriteMsgToConsole);
        }
        #endregion

        #region public methods

        /* void Start()
         * Start testing.
         */
        public void Start()
        {
            //print version information
            PrintHeaders();

            //init jobs.
            InitEngines();
            _index = 0;
            _currentTestSteps = _parser.GetAllMainTestSteps();

            while (true)
            {
                try
                {
                    //perform action of each step.
                    PerformEachStep();

                    if (_end)
                    {
                        break;
                    }
                }
                catch (TestException e)
                {
                    OnNewMessage(e.ToString());

                    //use ExceptionEngine to handle exceptions
                    //can not go to next step.
                    if (!_exEngine.HandleException(e))
                    {
                        //main test step, stop testing.
                        if (_testStepStack.Count == 0)
                        {
                            break;
                        }
                        else
                        {
                            // return to parent test step.
                            TestStepStatus tmp = _testStepStack.Pop();
                            _index = tmp._index;
                            _currentTestSteps = tmp._stepList;
                        }
                    }
                    else
                    {
                        //we can handle it...
                    }
                }
                catch (Exception ex)
                {
                    OnNewMessage(ex.ToString());
                    break;
                }
            }

            End();

        }

        /* void End()
         * Stop testing.
         */
        public void End()
        {

            string endMsg = "\n------------------------------------------------------------------\n"
            + "Test End";

            this._logEngine.Close();

            OnNewMessage(endMsg);

        }

        #endregion

        #region private methods

        /* void WriteMsgToConsole(string message)
         * Always write log to console, for debugging or test.
         */
        private void WriteMsgToConsole(string message)
        {
            message = message.TrimEnd();
            Console.WriteLine(message);
        }

        /* void PrintHeaders()
         * Print some version information
         */
        private void PrintHeaders()
        {
            string headers =
            "\n----------------------------------------\n"
            + "UAT Automation Framework v0.8 C# Edition\n"
            + "        Shrinerain@hotmail.com          \n"
            + "----------------------------------------\n\n"
            + "Start to test...\n"
            + "------------------------------------------------------------------\n"
            + "Command\tItem\tProperty\tAction\tData\tExpectResult\tActualResult\n";

            OnNewMessage(headers);

        }

        /* void InitEngines()
         * Init other engines.
         */
        private void InitEngines()
        {
            try
            {

                this._objEngine = new ObjectEngine();
                this._subEngine = new SubEngine();
                this._actEngine = new ActionEngine();
                this._vpEngine = new VPEngine();
                this._logEngine = new LogEngine();
                this._exEngine = new ExceptionEngine();

                this._isHighlight = this._autoConfig.IsHighlight;

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TestFrameworkException("Fatal Error: Can not load engines: " + ex.Message);
            }
        }

        /* void PerformEachStep()
         * Execute each step, means each line in the EXCEL driver file.
         */
        private void PerformEachStep()
        {
            //get the test step.
            TestStep currentStep = GetNextStep();

            //clear last step information.
            this._logEngine.Clear();
            this._logEngine.TestStepInfo = currentStep.ToString();

            //if the first column is empty, stop testing.
            if (String.IsNullOrEmpty(currentStep._testCommand))
            {
                PerformEnd();
            }
            else
            {
                OnNewMessage(currentStep.ToString());

                string command = currentStep._testCommand.ToUpper();

                //check the command, if "Begin" or "Start" , then start.
                //if not started yet, we won't execute any actions.
                if (!this._started)
                {
                    if (command == "BEGIN" || command == "START")
                    {
                        PerformBegin();
                    }
                }
                else
                {
                    //perform other actions
                    if (command == "COMMENTS" || command == "COMMENT")
                    {
                        PerformComments();
                    }
                    else if (command == "SKIP")
                    {
                        PerformSkip();
                    }
                    else if (command == "GO")
                    {
                        PerformGo(currentStep);
                    }
                    else if (command == "VP")
                    {
                        PerformVP(currentStep);
                    }
                    else if (command == "IF")
                    {
                        PerformIf(currentStep);
                    }
                    else if (command == "JUMP")
                    {
                        PerformJump(currentStep);
                    }
                    else if (command == "END")
                    {
                        PerformEnd();
                    }
                    else if (command == "EXIT" || command == "RETURN")
                    {
                        PerformExit();
                    }
                    else if (command == "CALL")
                    {
                        PerformCall(currentStep);
                    }
                    else
                    {
                        throw new UnSupportedKeywordException("Command: " + command + " is not supported.");
                    }
                }
            }
        }

        /* TestStep GetNextStep()
         * Return the next test step
         */
        private TestStep GetNextStep()
        {
            if (_index < _currentTestSteps.Count)
            {
                return _currentTestSteps[_index++];
            }
            else
            {
                //if the index if out of test steps range, return an empty one.
                return new TestStep();
            }
        }

        #region other actions

        /* void PerformBegin()
         * set the _started flag true, then we can start testing.
         */
        private void PerformBegin()
        {
            _started = true;

            this._logEngine.WriteLog("Test Begin.");
        }

        /* void PerformGo(TestStep step)
         * Perform normal actions.
         * If the first column in EXCEL driver file is "GO", we will call this method.
         */
        private void PerformGo(TestStep step)
        {

            string item = step._testControl;

            if (item.ToUpper() == "BROWSER")
            {
                if (_testBrowser == null)
                {
                    _testBrowser = (TestBrowser)_objEngine.GetTestBrowser();
                }

                string action = step._testAction.ToUpper();
                if (action == "START")
                {
                    _testBrowser.Start();
                    string url = step._testData;
                    if (!String.IsNullOrEmpty(url) && (url.ToUpper().StartsWith("HTTP://")) || url.ToUpper().StartsWith("HTTPS://"))
                    {

                        _testBrowser.Load(url);
                    }
                }
                else if (action == "LOAD")
                {
                    _testBrowser.Load(step._testData);
                }
                else if (action == "FIND")
                {
                    _testBrowser.Find(step._testData);
                }
                else if (action == "WAIT")
                {
                    string data = step._testData.ToUpper().Replace(" ", "");

                    int seconds;
                    if (int.TryParse(data, out seconds))
                    {
                        _testBrowser.Wait(seconds);
                    }
                    else
                    {
                        if (data == "NEXTPAGE")
                        {
                            _testBrowser.WaitForNextPage();
                        }
                        else if (data == "POPWINDOW")
                        {
                            _testBrowser.WaitForPopWindow();
                        }
                        else if (data == "NEWWINDOW")
                        {
                            _testBrowser.WaitForNewWindow();
                        }
                        else if (data == "NEWTAB")
                        {
                            _testBrowser.WaitForNewTab();
                        }
                        else
                        {
                            throw new CannotPerformActionException("Error: Bad data for Wait action: " + data);
                        }
                    }
                }
                else if (action == "MAXSIZE")
                {
                    _testBrowser.MaxSize();
                }
                else if (action == "CLOSE")
                {
                    _testBrowser.Close();
                }
                else if (action == "REFRESH")
                {
                    _testBrowser.Refresh();
                }
                else if (action == "FORWARD")
                {
                    _testBrowser.Forward();
                }
                else if (action == "BACK")
                {
                    _testBrowser.Back();
                }
                else
                {
                    throw new CannotPerformActionException("Unsupported action: " + step._testAction);
                }

                //sleep 1 seconds after browser action, make it looks like human actions
                Thread.Sleep(1000 * 1);

            }
            else if (item.ToUpper() == "APP")
            {
                if (_testApp == null)
                {
                    _testApp = (ITestApp)_objEngine.GetTestApp();
                }
            }
            else
            {
                TestObject obj = _objEngine.GetTestObject(step);

                _testBrowser.Active();

                if (this._isHighlight)
                {
                    ((IVisible)obj).HighLight();
                }

                _actEngine.PerformAction(obj, step._testAction, step._testData);

                //sleep for 1 second, make it looks like human actions
                Thread.Sleep(1000 * 1);
            }

            this._logEngine.WriteLog();


        }

        /* void PerformVP(TestStep step)
         * perform check point
         */
        private void PerformVP(TestStep step)
        {
            try
            {

                TestObject obj = _objEngine.GetTestObject(step);

                ((IVisible)obj).HighLight();
                this._logEngine.SaveScreenPrint();

                object actualReslut;
                string message = null;

                if (_vpEngine.PerformVPCheck(obj, step._testAction, step._testVPProperty, step._testExpectResult, out actualReslut))
                {
                    message = "*** PASS: " + step.ToString() + "\t" + actualReslut.ToString();
                }
                else
                {
                    message = "*** FAIL: " + step.ToString() + "\t" + actualReslut.ToString();
                }

                OnNewMessage(message);
                this._logEngine.TestResultInfo = message;

                this._logEngine.WriteLog();

            }
            catch (Exception ex)
            {
                throw new VerifyPointException("Can not perform VP with step [" + step.ToString() + "]: " + ex.Message);
            }
        }

        /*  void PerformCall(TestStep step)
         *  call subs.
         */
        private void PerformCall(TestStep step)
        {

            string subName = step._testControl;
            List<TestStep> subSteps;
            subSteps = _subEngine.BuildTestStepBySubName(subName);

            if (subSteps == null || subSteps.Count < 1)
            {
                throw new CannotLoadSubException("Sub steps must contains step.");
            }

            TestStepStatus tmp = new TestStepStatus();
            tmp._index = _index;
            tmp._stepList = _currentTestSteps;
            _testStepStack.Push(tmp);

            _currentTestSteps = subSteps;
            _index = 0;

            this._logEngine.WriteLog();

        }

        /* void PerformEnd()
         * Stop testing.
         */
        private void PerformEnd()
        {
            this._end = true;
            this._logEngine.WriteLog("Test End.");
        }

        /* void PerformExit()
         * Exit from a sub, if not in a sub, then stop testing.
         */
        private void PerformExit()
        {
            TestStepStatus tmp;
            if (_testStepStack.Count > 0)
            {
                tmp = _testStepStack.Pop();

                _index = tmp._index;
                _currentTestSteps = tmp._stepList;
            }
            else
            {
                PerformEnd();
            }

            this._logEngine.WriteLog("Exit sub.");
        }

        /* void PerformIf(TestStep step)
         * check status, decide what to do next step.
         */
        private void PerformIf(TestStep step)
        {

        }

        /* void PerformJump(TestStep step)
         * Jump to other test step.
         */
        private void PerformJump(TestStep step)
        {

            int newIndex;

            if (int.TryParse(step._testData, out newIndex))
            {
                if (newIndex > 2)
                {
                    _index = newIndex - 2;
                }
            }


            this._logEngine.WriteLog("Jump to " + _index);
        }

        /* void PerformSkip()
         * skip current step.
         */
        private static void PerformSkip()
        {
        }

        /* void PerformComments()
         * skip current step, it is comments.
         */
        private static void PerformComments()
        {
        }

        #endregion

        #endregion

        #endregion
    }
}
