/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestObject.cs
*
* Description: TestObject class is the base class in AutoTester.
*              TestObject defines some standard properties and methods
*              for testing.
*              The actual test object must inherit TestObject.
*
* History:  2007/09/04 wan,yu Init version
*           2008/01/14 wan,yu update, remove id,name,handle,class from TestObject 
*
*********************************************************************/


namespace Shrinerain.AutoTester.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;

    [Serializable]
    public class TestObject : IProperty
    {
        #region fields

        protected TestApp _parentApp;

        //domain means the object type, eg: Win32
        protected string _domain;
        protected bool _isExist = true;

        protected List<String> _idenProperties = new List<String>();
        protected const string _visibleProperty = "VisibleProperty";

        #endregion

        #region properties

        public string Domain
        {
            get { return this._domain; }
        }

        public TestApp ParentApp
        {
            get { return _parentApp; }
            set { _parentApp = value; }
        }

        public static string VisibleProperty
        {
            get { return _visibleProperty; }
        }

        #endregion

        #region public methods

        public TestObject()
            : this(null)
        {
        }

        public TestObject(TestApp app)
            : this(app, "Unknow")
        {
        }

        public TestObject(TestApp app, String domain)
        {
            this._parentApp = app;
            this._domain = domain;
            this._isExist = true;

            SetIdenProperties();
        }

        #region public method

        /*  object GetProperty(string propertyName)
         *  get the expected property value.
         */
        public virtual object GetProperty(string propertyName)
        {
            return null;
        }

        public virtual bool HasProperty(string propertyName)
        {
            object val;
            return TryGetProperty(propertyName, out val);
        }

        public virtual bool TryGetProperty(string propertyName, out object value)
        {
            try
            {
                value = GetProperty(propertyName);
                return value != null;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        /* bool SetProperty(string propertyName, object value)
         * set the expected property, return true if successful.
         */
        public virtual bool SetProperty(string propertyName, object value)
        {
            return false;
        }

        //identification properties is used to identify a test object.
        //you can find an object by these properties.
        //when recoding, test object will return these properties.
        protected virtual void SetIdenProperties()
        {
            _idenProperties.Add(TestConstants.PROPERTY_DOMAIN);
        }

        public virtual void SetIdenProperties(String[] idenProperties)
        {
            if (idenProperties != null && idenProperties.Length > 0)
            {
                for (int i = 0; i < idenProperties.Length; i++)
                {
                    string tpName = idenProperties[i].ToUpper();
                    if (!_idenProperties.Contains(tpName))
                    {
                        _idenProperties.Add(tpName);
                    }
                }
            }
        }

        //these properties is used to identify an object.
        //we will record these properties, and when playing back, use these properties to find an object.
        public virtual List<TestProperty> GetIdenProperties()
        {
            List<TestProperty> properties = new List<TestProperty>();
            if (_idenProperties.Count > 0)
            {
                foreach (String tpName in _idenProperties)
                {
                    if (tpName == TestConstants.PROPERTY_DOMAIN)
                    {
                        properties.Add(new TestProperty(TestConstants.PROPERTY_DOMAIN, _domain));
                    }
                    else
                    {
                        object tpValue;
                        if (TryGetProperty(tpName, out tpValue))
                        {
                            properties.Add(new TestProperty(tpName, tpValue.ToString()));
                        }
                    }
                }
            }
            return properties;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            List<TestProperty> properties = GetIdenProperties();
            if (properties != null && properties.Count > 0)
            {
                foreach (TestProperty tp in properties)
                {
                    sb.Append(tp.Name + "=" + tp.Value + TestProperty.PropertySeparator);
                }
            }

            return sb.ToString();
        }

        public virtual bool IsExist()
        {
            return _isExist;
        }

        #endregion

        #region actions


        #endregion

        #region parent and children

        #endregion

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion
    }

    //this object is only used for checking an object exist or not.
    internal class TestFakeObject : TestObject, ICheckable, IClickable, IContainer, IDragable, IHTML,
                                    IInputable, IInteractive, IMSAA, IPath,
                                    IPicture, ISelectable, IStatus, ITable, IText, IVisible, IWindows
    {
        internal TestFakeObject()
        {
            this._isExist = false;
        }

        #region ICheckable Members

        public void Check()
        {
            throw new ObjectNotFoundException();
        }

        public void UnCheck()
        {
            throw new ObjectNotFoundException();
        }

        public bool IsChecked()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IClickable Members

        public void Click()
        {
            throw new ObjectNotFoundException();
        }

        public void DoubleClick()
        {
            throw new ObjectNotFoundException();
        }

        public void RightClick()
        {
            throw new ObjectNotFoundException();
        }

        public void MiddleClick()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new ObjectNotFoundException();
        }

        public string GetAction()
        {
            throw new ObjectNotFoundException();
        }

        public void DoAction(object parameter)
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IVisible Members

        public void MouseClick()
        {
            throw new ObjectNotFoundException();
        }

        public string GetLabel()
        {
            throw new ObjectNotFoundException();
        }

        public System.Drawing.Point GetCenterPoint()
        {
            throw new ObjectNotFoundException();
        }

        public System.Drawing.Rectangle GetRectOnScreen()
        {
            throw new ObjectNotFoundException();
        }

        public System.Drawing.Bitmap GetControlPrint()
        {
            throw new ObjectNotFoundException();
        }

        public void Hover()
        {
            throw new ObjectNotFoundException();
        }

        public void HighLight()
        {
            throw new ObjectNotFoundException();
        }

        public bool IsVisible()
        {
            throw new ObjectNotFoundException();
        }

        public bool IsEnable()
        {
            throw new ObjectNotFoundException();
        }

        public bool IsReadonly()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IClickable Members

        void IClickable.Click()
        {
            throw new ObjectNotFoundException();
        }

        void IClickable.DoubleClick()
        {
            throw new ObjectNotFoundException();
        }

        void IClickable.RightClick()
        {
            throw new ObjectNotFoundException();
        }

        void IClickable.MiddleClick()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IInteractive Members

        void IInteractive.Focus()
        {
            throw new ObjectNotFoundException();
        }

        string IInteractive.GetAction()
        {
            throw new ObjectNotFoundException();
        }

        void IInteractive.DoAction(object parameter)
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IVisible Members

        string IVisible.GetLabel()
        {
            throw new ObjectNotFoundException();
        }

        System.Drawing.Point IVisible.GetCenterPoint()
        {
            throw new ObjectNotFoundException();
        }

        System.Drawing.Rectangle IVisible.GetRectOnScreen()
        {
            throw new ObjectNotFoundException();
        }

        System.Drawing.Bitmap IVisible.GetControlPrint()
        {
            throw new ObjectNotFoundException();
        }

        void IVisible.Hover()
        {
            throw new ObjectNotFoundException();
        }

        void IVisible.HighLight()
        {
            throw new ObjectNotFoundException();
        }

        bool IVisible.IsVisible()
        {
            throw new ObjectNotFoundException();
        }

        bool IVisible.IsEnable()
        {
            throw new ObjectNotFoundException();
        }

        bool IVisible.IsReadonly()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IContainer Members

        public object[] GetChildren()
        {
            throw new ObjectNotFoundException();
        }

        public object GetChild(int childIndex)
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IDragable Members

        public void Drag(System.Drawing.Point start, System.Drawing.Point end)
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IHTML Members

        public string GetTag()
        {
            throw new ObjectNotFoundException();
        }

        public mshtml.IHTMLElement GetHTMLElement()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IInputable Members

        public void Input(string values)
        {
            throw new ObjectNotFoundException();
        }

        public void InputKeys(string keys)
        {
            throw new ObjectNotFoundException();
        }

        public void Clear()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IText Members

        public string GetText()
        {
            throw new ObjectNotFoundException();
        }

        public string GetFontFamily()
        {
            throw new ObjectNotFoundException();
        }

        public string GetFontSize()
        {
            throw new ObjectNotFoundException();
        }

        public string GetFontColor()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IMSAA Members

        public Accessibility.IAccessible GetIAccInterface()
        {
            throw new ObjectNotFoundException();
        }

        public int GetChildID()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IPath Members

        public int GetDepth()
        {
            throw new ObjectNotFoundException();
        }

        public object[] GetObjectsAtPath(string path)
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IPicture Members


        public string GetSrc()
        {
            throw new ObjectNotFoundException();
        }

        public void Download(string path)
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region ISelectable Members

        public void Select(string str)
        {
            throw new ObjectNotFoundException();
        }

        public void SelectMulti(string[] strs)
        {
            throw new ObjectNotFoundException();
        }

        public void SelectByIndex(int index)
        {
            throw new ObjectNotFoundException();
        }

        public string[] GetAllValues()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IStatus Members

        public object GetCurrentStatus()
        {
            throw new ObjectNotFoundException();
        }

        public bool IsReady()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region ITable Members

        public int GetRowCount(int col)
        {
            throw new ObjectNotFoundException();
        }

        public int GetColCount(int row)
        {
            throw new ObjectNotFoundException();
        }

        public object[] GetObjectsByCell(int row, int col)
        {
            throw new ObjectNotFoundException();
        }

        public string GetTextByCell(int row, int col)
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IText Members

        string IText.GetText()
        {
            throw new ObjectNotFoundException();
        }

        string IText.GetFontFamily()
        {
            throw new ObjectNotFoundException();
        }

        string IText.GetFontSize()
        {
            throw new ObjectNotFoundException();
        }

        string IText.GetFontColor()
        {
            throw new ObjectNotFoundException();
        }

        #endregion

        #region IWindows Members

        public IntPtr GetHandle()
        {
            throw new ObjectNotFoundException();
        }

        public string GetClass()
        {
            throw new ObjectNotFoundException();
        }

        public string GetCaption()
        {
            throw new ObjectNotFoundException();
        }

        #endregion
    }
}
