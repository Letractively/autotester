using System;
using System.Collections.Generic;
using System.Text;
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

        #endregion

        #region methods

        #region ctor

        public CoreEngine()
        {

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

            Console.WriteLine("Start to testing...");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("  \t\tCommand\tItem\tProperty\tAction\tData\tExpectResult\tActualResult");
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
                    Console.WriteLine("Fatal Error: " + e.ToString());
                    break;
                }
            }
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("End.");

        }

        public void End()
        {

        }

        #endregion

        #region private methods

        private static void PrintHeaders()
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("UAT Automation Framework v0.6 C# Edition");
            Console.WriteLine("        Shrinerain@hotmail.com          ");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();
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

            Console.WriteLine("Test Step:\t" + currentStep.ToString());

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
            this._started = true;
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

                }

            }
            else
            {
                TestObject obj = _objEngine.GetTestObject(step);

                _actEngine.PerformAction(obj, step._testAction, step._testData);
            }


        }

        private void PerformVP(TestStep step)
        {

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
