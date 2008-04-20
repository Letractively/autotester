/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestFrameworkException.cs
*
* Description: This component defines exceptions used for UAF.
*              UAF = UAT Automation Framework.
*              UAF is a keyword driven framework. 
*              Please refer to Framework component.
*              
* History: 2007/12/28 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestFrameworkException : TestException
    {
        public TestFrameworkException()
            : this("UAF Error.")
        {

        }

        public TestFrameworkException(string message)
            : base(message)
        {

        }
    };

    #region engine

    public class TestEngineException : TestFrameworkException
    {
        public TestEngineException()
            : this("Test engine error.")
        {

        }
        public TestEngineException(string message)
            : base(message)
        {

        }
    }

    public class CannotInitCoreEngineException : TestEngineException
    {
        public CannotInitCoreEngineException()
            : this("Can not init Core engine.")
        {
        }
        public CannotInitCoreEngineException(string message)
            : base(message)
        {

        }
    }

    public class CannotInitObjectEngineException : TestEngineException
    {
        public CannotInitObjectEngineException()
            : this("Can not init Object engine.")
        {

        }
        public CannotInitObjectEngineException(string message)
            : base(message)
        {

        }
    }

    public class CannotInitActionEngineException : TestEngineException
    {
        public CannotInitActionEngineException()
            : this("Can not init Action engine.")
        {

        }
        public CannotInitActionEngineException(string message)
            : base(message)
        {

        }
    }

    public class CannotInitVPEngineException : TestEngineException
    {
        public CannotInitVPEngineException()
            : this("Can not init VP engine.")
        {

        }

        public CannotInitVPEngineException(string message)
            : base(message)
        {

        }
    }

    public class CannotInitSubEngineException : TestEngineException
    {
        public CannotInitSubEngineException()
            : this("Can not init Sub engine.")
        {

        }

        public CannotInitSubEngineException(string message)
            : base(message)
        {

        }
    }

    public class CannotInitDataEngineException : TestEngineException
    {
        public CannotInitDataEngineException()
            : this("Can not init Data engine.")
        {

        }

        public CannotInitDataEngineException(string message)
            : base(message)
        {

        }
    }

    public class CannotInitLogEngineException : TestEngineException
    {
        public CannotInitLogEngineException()
            : this("Can not init Log engine.")
        {
        }

        public CannotInitLogEngineException(string message)
            : base(message)
        {
        }
    }

    public class CannotInitExceptionEngineException : TestEngineException
    {
        public CannotInitExceptionEngineException()
            : this("Can not init Exception engine.")
        {

        }

        public CannotInitExceptionEngineException(string message)
            : base(message)
        {

        }
    }

    #endregion

    #region keywords

    public class TestKeywordException : TestFrameworkException
    {
        public TestKeywordException()
            : this("Error of keyword")
        {
        }
        public TestKeywordException(string message)
            : base(message)
        {
        }
    }

    public class UnSupportedKeywordException : TestKeywordException
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

    public class BadKeywordParametersException : TestKeywordException
    {
        public BadKeywordParametersException()
            : this("The keyword does not support these parameters.")
        {

        }
        public BadKeywordParametersException(string message)
            : base(message)
        {

        }
    }

    #endregion

    #region config
    public class TestConfigException : TestFrameworkException
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

    #endregion

    #region framework dll
    public class TestDLLException : TestFrameworkException
    {
        public TestDLLException()
            : this("Error of dll.")
        {
        }
        public TestDLLException(String message)
            : base(message)
        {

        }
    }
    public class DllNotFoundException : TestDLLException
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
    public class CannotLoadDllException : TestDLLException
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
    public class InvalidDllException : TestDLLException
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

    #endregion

    #region log template
    public class TestLogTemplateException : TestFrameworkException
    {
        public TestLogTemplateException()
            : this("Error of test log template.")
        {

        }
        public TestLogTemplateException(string message)
            : base(message)
        {
        }
    }
    public class TemplateFileNotFoundException : TestLogTemplateException
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

    public class InvalidTemplateException : TestLogTemplateException
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

    #region driver file
    public class TestDriverFileExpcetion : TestFrameworkException
    {
        public TestDriverFileExpcetion()
            : this("Error of driver file.")
        {
        }
        public TestDriverFileExpcetion(string message)
            : base(message)
        {

        }
    }
    public class DriverFileNotFoundException : TestDriverFileExpcetion
    {
        public DriverFileNotFoundException()
            : this("Can not find driven file.")
        {

        }
        public DriverFileNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotOpenDriverFileException : TestConfigException
    {
        public CannotOpenDriverFileException()
            : this("Can not open driven file.")
        {

        }
        public CannotOpenDriverFileException(string message)
            : base(message)
        {

        }
    };
    public class BadFormatDriverFileException : TestConfigException
    {
        public BadFormatDriverFileException()
            : this("Bad format driven file.")
        {

        }
        public BadFormatDriverFileException(string message)
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

    #endregion


}