/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestException.cs
*
* Description: TestException defines the exception used in AutoTester.
*              The root exception is TestException class.
*              Then we have TestObjectException, TestBrowserException etc.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function
{
    [Serializable]
    public class TestException : Exception
    {
        #region fields

        //domain means the kind of test object. for example, HTML is a domain.
        protected string _domain;

        //if web testing, we may have url
        protected string _url;

        //if desktop application, we may have app name.
        protected string _app;

        //the source object which throw the expcetion.
        protected TestObject _obj;
        #endregion

        #region Properties
        public string Domain
        {
            get { return this._domain; }
            set
            {
                this._domain = value;
            }
        }

        public string Url
        {
            get { return this._url; }
            set
            {
                this._url = value;
            }
        }

        public string App
        {
            get { return _app; }
            set { _app = value; }
        }

        public TestObject SourceObject
        {
            get
            {
                return this._obj;
            }
            set
            {
                this._obj = value;
            }
        }

        #endregion

        #region public methods
        public TestException()
            : base()
        {

        }
        public TestException(string message)
            : base(message)
        {

        }
        public TestException(string domain, string message)
            : base(message)
        {
            this._domain = domain;
        }
        public TestException(string message, Exception e)
            : base(message, e)
        {

        }
        public TestException(string domain, string url, string message)
            : base(message)
        {
            this._domain = domain;
            this._url = url;
        }
        public TestException(TestObject obj, string domain, string url, string message)
            : base(message)
        {
            this._obj = obj;
            this._url = url;
            this._domain = domain;
        }
        #endregion
    }

    #region sub exceptions

    #region browser exceptions

    public class TestBrowserException : TestException
    {
        public TestBrowserException()
            : this("TestBrowse exception.")
        {

        }

        public TestBrowserException(string message)
            : base(message)
        {

        }
    };

    public class TestBrowserNotFoundException : TestBrowserException
    {
        public TestBrowserNotFoundException()
            : this("Can not find Internet Explorer.")
        {

        }
        public TestBrowserNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotStartTestBrowserException : TestBrowserException
    {
        public CannotStartTestBrowserException()
            : this("Can not start Internet Explorer.")
        {
        }
        public CannotStartTestBrowserException(string message)
            : base(message)
        {

        }
    };
    public class CannotAttachTestBrowserException : TestBrowserException
    {
        public CannotAttachTestBrowserException()
            : this("Can not hook Internet Explorer.")
        {
        }
        public CannotAttachTestBrowserException(string message)
            : base(message)
        {

        }
    };
    public class CannotActiveTestBrowserException : TestBrowserException
    {
        public CannotActiveTestBrowserException()
            : this("Can not active Internet Explorer.")
        {

        }
        public CannotActiveTestBrowserException(string message)
            : base(message)
        {

        }
    };
    public class CannotLoadUrlException : TestBrowserException
    {
        public CannotLoadUrlException()
            : this("Can not load the url, please check the url")
        {
        }
        public CannotLoadUrlException(string message)
            : base(message)
        {

        }
    }
    public class CannotNavigateException : TestBrowserException
    {
        public CannotNavigateException()
            : this("Can not navigate.")
        {
        }
        public CannotNavigateException(string message)
            : base(message)
        {

        }
    }
    #endregion

    #region app exceptions

    public class TestAppException : TestException
    {
        public TestAppException(string appName)
            : this(appName, "Test application exception.")
        {
            this._app = appName;
        }

        public TestAppException(string appName, string message)
            : base(message)
        {
            this._app = appName;
        }

    };

    public class CannotStartAppException : TestAppException
    {
        public CannotStartAppException()
            : this("Can not start test application.")
        {
        }
        public CannotStartAppException(string message)
            : base(message)
        {
        }

    }

    public class CannotStopAppException : TestAppException
    {
        public CannotStopAppException()
            : this("Can not stop test application.")
        {
        }

        public CannotStopAppException(string message)
            : base(message)
        {
        }
    }

    public class CannotActiveAppException : TestAppException
    {
        public CannotActiveAppException()
            : this("Can not active test application.")
        {
        }

        public CannotActiveAppException(string message)
            : base(message)
        {
        }
    }

    public class CannotAttachAppException : TestAppException
    {
        public CannotAttachAppException()
            : this("Can not attach test application.")
        {

        }

        public CannotAttachAppException(string message)
            : base(message)
        {
        }
    }

    public class CannotMoveAppException : TestAppException
    {
        public CannotMoveAppException()
            : this("Can not move test application.")
        {
        }

        public CannotMoveAppException(string message)
            : base(message)
        {

        }
    }

    public class CannotResizeAppException : TestAppException
    {
        public CannotResizeAppException()
            : this("Can not resize test application.")
        {
        }

        public CannotResizeAppException(string message)
            : base(message)
        {

        }
    }

    public class CannotGetAppStatusException : TestAppException
    {
        public CannotGetAppStatusException()
            : this("Can not get status of test application.")
        {
        }

        public CannotGetAppStatusException(string message)
            : base(message)
        {
        }
    }

    public class CannotGetAppProcessException : TestAppException
    {
        public CannotGetAppProcessException()
            : this("Can not get process of test application.")
        {
        }

        public CannotGetAppProcessException(string message)
            : base(message)
        {
        }

    }

    public class CannotGetAppNetworkException : TestAppException
    {
        public CannotGetAppNetworkException()
            : this("Can not get netowork information of test application.")
        {
        }

        public CannotGetAppNetworkException(string message)
            : base(message)
        {
        }
    }

    public class CannotGetAppInfoException : TestAppException
    {
        public CannotGetAppInfoException()
            : this("Can not get information of test application.")
        {
        }

        public CannotGetAppInfoException(string message)
            : base(message)
        {
        }
    }

    #endregion

    #region config exceptions

    public class TestConfigException : TestException
    {
        public TestConfigException()
            : this("Config expcetion.")
        {

        }
        public TestConfigException(string message)
            : base(message)
        {

        }
    }

    public class CannotLoadConfigException : TestConfigException
    {
        public CannotLoadConfigException()
            : this("Can not load config file.")
        {

        }

        public CannotLoadConfigException(string message)
            : base(message)
        {

        }
    };

    public class CannotSaveConfigFileException : TestConfigException
    {
        public CannotSaveConfigFileException()
            : this("Can not save project config file.")
        {
        }

        public CannotSaveConfigFileException(string message)
            : base(message)
        {
        }
    }

    public class ConfigFileNotFoundException : TestConfigException
    {
        public ConfigFileNotFoundException()
            : this("Config file not found.")
        {

        }
        public ConfigFileNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotOpenConfigFileException : TestConfigException
    {
        public CannotOpenConfigFileException()
            : this("Can not open config file.")
        {

        }
        public CannotOpenConfigFileException(string message)
            : base(message)
        {

        }
    };
    public class BadFormatConfigFileException : TestConfigException
    {
        public BadFormatConfigFileException()
            : this("Bad format config file.")
        {

        }
        public BadFormatConfigFileException(string message)
            : base(message)
        {

        }

    };
    public class DrivenFileNotFoundException : TestConfigException
    {
        public DrivenFileNotFoundException()
            : this("Can not find driven file.")
        {

        }
        public DrivenFileNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotOpenDrivenFileException : TestConfigException
    {
        public CannotOpenDrivenFileException()
            : this("Can not open driven file.")
        {

        }
        public CannotOpenDrivenFileException(string message)
            : base(message)
        {

        }
    };
    public class BadFormatDrivenFileException : TestConfigException
    {
        public BadFormatDrivenFileException()
            : this("Bad format driven file.")
        {

        }
        public BadFormatDrivenFileException(string message)
            : base(message)
        {

        }
    };

    public class CannotLoadTestStepsException : TestConfigException
    {
        public CannotLoadTestStepsException()
            : this("Can not load test steps.")
        {

        }
        public CannotLoadTestStepsException(string message)
            : base(message)
        {

        }
    };
    public class CannotLoadDataPoolException : TestConfigException
    {
        public CannotLoadDataPoolException()
            : this("Can not load data pool.")
        {

        }
        public CannotLoadDataPoolException(string message)
            : base(message)
        {

        }
    }
    public class CannotLoadSubException : TestConfigException
    {
        public CannotLoadSubException()
            : this("Can not load sub.")
        {

        }
        public CannotLoadSubException(string message)
            : base(message)
        {

        }

    }
    public class FolderNotFoundException : TestConfigException
    {
        public FolderNotFoundException()
            : this("Can not find the folder.")
        {

        }
        public FolderNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class DllNotFoundException : TestConfigException
    {
        public DllNotFoundException()
            : this("Can not find Dll.")
        {

        }
        public DllNotFoundException(string message)
            :
            base(message)
        {

        }
    };
    public class CannotLoadDllException : TestConfigException
    {
        public CannotLoadDllException()
            : this("Can not load dll.")
        {

        }
        public CannotLoadDllException(string message)
            : base(message)
        {

        }

    };
    public class InvalidDllException : TestConfigException
    {
        public InvalidDllException()
            : this("Invalid dll, can not load it.")
        {

        }
        public InvalidDllException(string message)
            : base(message)
        {

        }
    };

    public class TemplateFileNotFoundException : TestConfigException
    {
        public TemplateFileNotFoundException()
            : this("Can not find log template.")
        {
        }
        public TemplateFileNotFoundException(string message)
            : base(message)
        {

        }
    }
    public class InvalidTemplateException : TestConfigException
    {
        public InvalidTemplateException()
            : this("Invalid template file.")
        {
        }
        public InvalidTemplateException(string message)
            : base(message)
        {

        }
    }

    #endregion

    #region object exceptions

    public class TestObjectException : TestException
    {
        public TestObjectException()
            : this("Test object exception.")
        {

        }
        public TestObjectException(string message)
            : base(message)
        {

        }
        public TestObjectException(TestObject obj, string message)
            : base(message)
        {
            this._obj = obj;
        }
    };

    public class ObjectNotFoundException : TestObjectException
    {
        public ObjectNotFoundException()
            : this("Can not found test object.")
        {

        }
        public ObjectNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotBuildObjectException : TestObjectException
    {
        public CannotBuildObjectException()
            : this("Can not build test object.")
        {
        }
        public CannotBuildObjectException(string message)
            : base(message)
        {
        }
    }
    public class CannotGetObjectPositionException : TestObjectException
    {
        public CannotGetObjectPositionException()
            : this("Can not get the position of the object.")
        {
        }
        public CannotGetObjectPositionException(string message)
            : base(message)
        {

        }
    }
    public class ItemNotFoundException : TestObjectException
    {
        public ItemNotFoundException()
            : this("Can not found sub item.")
        {

        }
        public ItemNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class PropertyNotFoundException : TestObjectException
    {
        public PropertyNotFoundException()
            : this("Can not find the property.")
        {

        }
        public PropertyNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotSetPropertyException : TestObjectException
    {
        public CannotSetPropertyException()
            : this("Can not set the property.")
        {
        }
        public CannotSetPropertyException(string message)
            : base(message)
        {

        }
    }
    public class CannotPerformActionException : TestObjectException
    {
        public CannotPerformActionException()
            : this("Can not perform the action.")
        {

        }
        public CannotPerformActionException(string message)
            : base(message)
        {

        }
    };

    public class TimeoutException : TestObjectException
    {
        public TimeoutException()
            : this("Time is out.")
        {

        }
        public TimeoutException(string message)
            : base(message)
        {

        }
    };

    public class VerifyPointException : TestObjectException
    {
        public VerifyPointException()
            : this("Can not perform VP check.")
        {

        }
        public VerifyPointException(string message)
            : base(message)
        {

        }
    };

    public class CannotHighlightObjectException : TestObjectException
    {
        public CannotHighlightObjectException()
            : this("Can not highlight the object.")
        {

        }
        public CannotHighlightObjectException(string message)
            : base(message)
        {

        }
    };
    #endregion

    #region log Exception

    public class TestLogException : TestException
    {
        public TestLogException()
            : this("Log expcetion.")
        {

        }
        public TestLogException(string message)
            : base(message)
        {

        }
    }

    public class CannotSaveScreenPrintException : TestLogException
    {
        public CannotSaveScreenPrintException()
            : this("Can not save screen print.")
        {

        }
        public CannotSaveScreenPrintException(string message)
            : base(message)
        {

        }
    };

    public class CannotWriteLogException : TestLogException
    {
        public CannotWriteLogException()
            : this("Can not write log.")
        {
        }
        public CannotWriteLogException(string message)
            : base(message)
        {

        }
    }

    public class CannotOpenTemplateException : TestLogException
    {
        public CannotOpenTemplateException()
            : this("Can not open log template file.")
        {

        }

        public CannotOpenTemplateException(string message)
            : base(message)
        {

        }
    };

    #endregion

    #region Framework exceptions
    public class TestEngineException : TestException
    {
        public TestEngineException()
            : this("Fatal Error: Engines Error.")
        {

        }

        public TestEngineException(string message)
            : base(message)
        {

        }
    };

    public class UnSupportedKeywordException : TestEngineException
    {
        public UnSupportedKeywordException()
            : this("The keyword is not supported.")
        {

        }
        public UnSupportedKeywordException(string message)
            : base(message)
        {

        }
    };


    #endregion

    #endregion
}
