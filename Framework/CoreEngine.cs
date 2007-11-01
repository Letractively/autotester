using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.Framework
{

    internal struct TestStepStatus
    {
        internal int _index;
        internal List<TestStep> _stepList;
    };

    public sealed class CoreEngine
    {
        #region Fields


        //event to deliver message to other program.
        public delegate void _frameworkInfoDelegate(string message);
        public event _frameworkInfoDelegate OnNewMessage;


        private AutoConfig _autoConfig;
        private Parser _parser;
        private TestBrowser _browser;


        //other engine to perform each action.
        private ObjectEngine _objEngine;
        private ActionEngine _actEngine;
        private VPEngine _vpEngine;
        private LogEngine _logEngine;
        //private DataEngine _dataEngine;
        private SubEngine _subEngine;
        private ExceptionEngine _exEngine;

        //flag for start or not. If false, will not perform any action.
        private bool _started = false;
        private bool _isHighligh = false;

        //currently used test steps list. 
        private int _index = 0;  //actually, the line number start from 2 in the excel file.
        private List<TestStep> _currentTestSteps = null;

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

        public bool IsHighligh
        {
            get { return _isHighligh; }
            set { _isHighligh = value; }
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
        }
        #endregion

        #region public methods

        public void Start()
        {
            PrintHeaders();

            InitEngines();
            _index = 0;
            _currentTestSteps = _parser.GetAllMainTestSteps();

            while (true)
            {
                try
                {
                    PerformEachStep();
                }
                catch (TestException e)
                {
                    //can not go to next step.
                    if (!_exEngine.HandleException(e))
                    {
                        //main test step, break
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
                        //go to next step
                        _index++;
                    }
                }
                catch (Exception e)
                {
                    OnNewMessage(e.ToString());
                    break;
                }
            }

        }

        public void End()
        {

            string endMsg = "\n------------------------------------------------------------------\n"
            + "End.\n";

            OnNewMessage(endMsg);

            //after testing finished. collect all resources.
            GC.Collect();

        }

        #endregion

        #region private methods

        private void WriteMsgToConsole(string message)
        {
            Console.WriteLine(message);
        }

        private void PrintHeaders()
        {
            string headers =
            "\n----------------------------------------\n"
            + "UAT Automation Framework v0.7 C# Edition\n"
            + "        Shrinerain@hotmail.com          \n"
            + "----------------------------------------\n\n"
            + "Start to testing...\n"
            + "------------------------------------------------------------------\n"
            + "Command\tItem\tProperty\tAction\tData\tExpectResult\tActualResult\n";

            OnNewMessage(headers);

        }

        private void InitEngines()
        {
            try
            {
                ////parser config file.
                //this._autoConfig.ParseConfigFile();
                //this._autoConfig.Close();

                ////parser drive file.
                //this._parser.ParseDriveFile();
                //this._parser.Close();

                this._objEngine = new ObjectEngine();
                this._subEngine = new SubEngine();
                this._actEngine = new ActionEngine();
                this._vpEngine = new VPEngine();
                this._logEngine = new LogEngine();
                this._exEngine = new ExceptionEngine();
                // this._dataEngine = new DataEngine();

                this._isHighligh = this._autoConfig.IsHighlight;

            }
            catch
            {
                throw new TestEngineException("Fatal Error: Can not load engines.");
            }
        }

        private void PerformEachStep()
        {
            TestStep currentStep = GetNextStep();
            if (String.IsNullOrEmpty(currentStep._testCommand))
            {
                PerformEnd();
            }

            OnNewMessage(currentStep.ToString());

            string command = currentStep._testCommand.ToUpper();

            if (command == "COMMENTS" || command == "COMMENT")
            {
                PerformComments();
            }
            else if (command == "SKIP")
            {
                PerformSkip();
            }
            else if (command == "BEGIN" || command == "START")
            {
                PerformBegin();
            }
            else if (command == "GO")
            {
                PerformGo(currentStep);
            }
            else if (command == "VP")
            {
                PerformVP(currentStep);
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
                throw new UnSupportedKeywordException("Command:" + command + " is not supported.");
            }

        }

        private TestStep GetNextStep()
        {
            if (_index < _currentTestSteps.Count)
            {
                return _currentTestSteps[_index++];
            }
            else
            {
                return new TestStep();
            }
        }

        private void PerformBegin()
        {
            _started = true;
        }

        private void PerformGo(TestStep step)
        {
            string item = step._testItem;

            if (item.ToUpper() == "BROWSER")
            {
                string action = step._testAction.ToUpper();
                if (_browser == null)
                {
                    _browser = (TestBrowser)_objEngine.GetTestBrowser();
                }
                if (action == "START")
                {
                    _browser.Start();
                    string url = step._testData;
                    if (!String.IsNullOrEmpty(url) && (url.ToUpper().StartsWith("HTTP://")) || url.ToUpper().StartsWith("HTTPS://"))
                    {

                        _browser.Load(url);
                    }
                }
                else if (action == "LOAD")
                {
                    _browser.Load(step._testData);
                }
                else if (action == "WAIT")
                {
                    string data = step._testData.ToUpper().Replace(" ", "");

                    int seconds;
                    if (int.TryParse(data, out seconds))
                    {
                        _browser.Wait(seconds);
                    }
                    else
                    {
                        if (data == "NEXTPAGE")
                        {
                            _browser.WaitForNextPage();
                        }
                        else if (data == "POPWINDOW")
                        {
                            _browser.WaitForPopWindow();
                        }
                        else if (data == "NEWWINDOW")
                        {
                            _browser.WaitForNewWindow();
                        }
                    }
                }
                else if (action == "MAXSIZE")
                {
                    _browser.MaxSize();
                }
                else if (action == "CLOSE")
                {
                    _browser.Close();
                }
                else if (action == "REFRESH")
                {
                    _browser.Refresh();
                }
                else if (action == "FORWARD")
                {
                    _browser.Forward();
                }
                else if (action == "BACK")
                {
                    _browser.Back();
                }
                else
                {
                    throw new CanNotPerformActionException("Unsupported action: " + step._testAction);
                }

                //sleep 1 seconds after browser action, or it will too fast.
                Thread.Sleep(1000 * 1);

            }
            else
            {
                TestObject obj = _objEngine.GetTestObject(step);

                if (this._isHighligh)
                {
                    obj.HightLight();
                }

                _actEngine.PerformAction(obj, step._testAction, step._testData);
            }


        }

        private void PerformVP(TestStep step)
        {
            try
            {
                TestObject obj = _objEngine.GetTestObject(step);
                obj.HightLight();

                if (_vpEngine.PerformVPCheck(obj, step._testAction, step._testVPProperty, step._testExpectResult))
                {
                    OnNewMessage("*** Pass: " + step.ToString());
                }
                else
                {
                    OnNewMessage("*** Failed: " + step.ToString());
                }

            }
            catch
            {
                throw new VerifyPointException("Can not perform VP:" + step.ToString());
            }
        }

        private void PerformCall(TestStep step)
        {
            string subName = step._testItem;
            List<TestStep> subSteps;
            subSteps = _subEngine.BuildTestStepBySubName(subName);

            if (subSteps == null || subSteps.Count < 1)
            {
                throw new CanNotLoadSubException("Sub steps must contains more than one step.");
            }

            TestStepStatus tmp = new TestStepStatus();
            tmp._index = _index;
            tmp._stepList = _currentTestSteps;
            _testStepStack.Push(tmp);

            _currentTestSteps = subSteps;
            _index = 0;

        }

        private void PerformEnd()
        {
            throw new TestEngineException("End.");
        }

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
        }

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
        }

        private void PerformSkip()
        {
            // _index++;
        }

        private void PerformComments()
        {
            // _index++;
        }



        #endregion

        #endregion
    }
}
