using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.Recorder
{
    public class Recorder
    {
        #region fields

        private string _scriptFile;

        private ITestApp _app;
        private ITestEventDispatcher _dispatcher;

        public string ScriptFile
        {
            get { return _scriptFile; }
            set { _scriptFile = value; }
        }

        public ITestApp AppUnderTest
        {
            get { return _app; }
            set { _app = value; }
        }

        #endregion

        #region methods

        #region ctor
        Recorder()
        {
        }
        #endregion

        public void Start()
        {
            if (_app != null)
            {
                _dispatcher = _app.GetEventDispatcher();
                if (_dispatcher != null)
                {
                    _dispatcher.OnClick += new TestObjectEventHandler(_dispatcher_OnClick);
                }
            }
        }

        void _dispatcher_OnClick(TestObject sender, TestEventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
