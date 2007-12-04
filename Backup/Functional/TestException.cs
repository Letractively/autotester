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
        protected string _url;

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

    #region ie exceptions

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
    public class CanNotStartTestBrowserException : TestBrowserException
    {
        public CanNotStartTestBrowserException()
            : this("Can not start Internet Explorer.")
        {
        }
        public CanNotStartTestBrowserException(string message)
            : base(message)
        {

        }
    };
    public class CanNotAttachTestBrowserException : TestBrowserException
    {
        public CanNotAttachTestBrowserException()
            : this("Can not hook Internet Explorer.")
        {
        }
        public CanNotAttachTestBrowserException(string message)
            : base(message)
        {

        }
    };
    public class CanNotActiveTestBrowserException : TestBrowserException
    {
        public CanNotActiveTestBrowserException()
            : this("Can not active Internet Explorer.")
        {

        }
        public CanNotActiveTestBrowserException(string message)
            : base(message)
        {

        }
    };
    public class CanNotLoadUrlException : TestBrowserException
    {
        public CanNotLoadUrlException()
            : this("Can not load the url, please check the url")
        {
        }
        public CanNotLoadUrlException(string message)
            : base(message)
        {

        }
    }
    public class CanNotNavigateException : TestBrowserException
    {
        public CanNotNavigateException()
            : this("Can not navigate.")
        {
        }
        public CanNotNavigateException(string message)
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

    public class CanNotLoadAutoConfigException : TestConfigException
    {
        public CanNotLoadAutoConfigException()
            : this("AutoConfig error.")
        {

        }

        public CanNotLoadAutoConfigException(string message)
            : base(message)
        {

        }
    };

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
    public class CanNotOpenConfigFileException : TestConfigException
    {
        public CanNotOpenConfigFileException()
            : this("Can not open config file.")
        {

        }
        public CanNotOpenConfigFileException(string message)
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
    public class CanNotOpenDrivenFileException : TestConfigException
    {
        public CanNotOpenDrivenFileException()
            : this("Can not open driven file.")
        {

        }
        public CanNotOpenDrivenFileException(string message)
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

    public class CanNotLoadTestStepsException : TestConfigException
    {
        public CanNotLoadTestStepsException()
            : this("Can not load test steps.")
        {

        }
        public CanNotLoadTestStepsException(string message)
            : base(message)
        {

        }
    };
    public class CanNotLoadDataPoolException : TestConfigException
    {
        public CanNotLoadDataPoolException()
            : this("Can not load data pool.")
        {

        }
        public CanNotLoadDataPoolException(string message)
            : base(message)
        {

        }
    }
    public class CanNotLoadSubException : TestConfigException
    {
        public CanNotLoadSubException()
            : this("Can not load sub.")
        {

        }
        public CanNotLoadSubException(string message)
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
    public class CanNotLoadDllException : TestConfigException
    {
        public CanNotLoadDllException()
            : this("Can not load dll.")
        {

        }
        public CanNotLoadDllException(string message)
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
    public class CanNotBuildObjectException : TestObjectException
    {
        public CanNotBuildObjectException()
            : this("Can not build test object.")
        {
        }
        public CanNotBuildObjectException(string message)
            : base(message)
        {
        }
    }
    public class CanNotGetObjectPositionException : TestObjectException
    {
        public CanNotGetObjectPositionException()
            : this("Can not get the position of the object.")
        {
        }
        public CanNotGetObjectPositionException(string message)
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
    public class CanNotSetPropertyException : TestObjectException
    {
        public CanNotSetPropertyException()
            : this("Can not set the property.")
        {
        }
        public CanNotSetPropertyException(string message)
            : base(message)
        {

        }
    }
    public class CanNotPerformActionException : TestObjectException
    {
        public CanNotPerformActionException()
            : this("Can not perform the action.")
        {

        }
        public CanNotPerformActionException(string message)
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

    public class CanNotHighlightObjectException : TestObjectException
    {
        public CanNotHighlightObjectException()
            : this("Can not highlight the object.")
        {

        }
        public CanNotHighlightObjectException(string message)
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

    public class CanNotSaveScreenPrintException : TestLogException
    {
        public CanNotSaveScreenPrintException()
            : this("Can not save screen print.")
        {

        }
        public CanNotSaveScreenPrintException(string message)
            : base(message)
        {

        }
    };

    public class CanNotWriteLogException : TestLogException
    {
        public CanNotWriteLogException()
            : this("Can not write log.")
        {
        }
        public CanNotWriteLogException(string message)
            : base(message)
        {

        }
    }

    public class CanNotOpenTemplateException : TestLogException
    {
        public CanNotOpenTemplateException()
            : this("Can not open log template file.")
        {

        }

        public CanNotOpenTemplateException(string message)
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

    public class UnSupportedKeywordException : TestException
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
