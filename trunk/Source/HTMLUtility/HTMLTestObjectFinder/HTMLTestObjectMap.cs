using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestObjectMap : TestObjectMap
    {
        #region

        public HTMLTestObjectMap(HTMLTestObjectPool pool)
            : base(pool)
        {
        }

        #endregion

        #region test object

        #region Button

        public new HTMLTestButton Button()
        {
            return Button(String.Empty);
        }

        public new HTMLTestButton Button(string name)
        {
            return Buttons(name)[0];
        }

        public new HTMLTestButton Button(TestProperty[] properties)
        {
            return Buttons(properties)[0];
        }

        public new HTMLTestButton[] Buttons()
        {
            return Buttons(String.Empty);
        }

        public new HTMLTestButton[] Buttons(string name)
        {
            GetMapObjects(HTMLTestObjectType.Button, name);
            HTMLTestButton[] tmp = new HTMLTestButton[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestButton[] Buttons(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.Button, properties);
            HTMLTestButton[] tmp = new HTMLTestButton[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region textbox

        public new HTMLTestTextBox TextBox()
        {
            return TextBox(String.Empty);
        }

        public new HTMLTestTextBox TextBox(string name)
        {
            return TextBoxs(name)[0];
        }

        public new HTMLTestTextBox TextBox(TestProperty[] properties)
        {
            return TextBoxs(properties)[0];
        }

        public new HTMLTestTextBox[] TextBoxs()
        {
            return TextBoxs(String.Empty);
        }

        public new HTMLTestTextBox[] TextBoxs(string name)
        {
            GetMapObjects(HTMLTestObjectType.TextBox, name);
            HTMLTestTextBox[] tmp = new HTMLTestTextBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestTextBox[] TextBoxs(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.TextBox, properties);
            HTMLTestTextBox[] tmp = new HTMLTestTextBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region checkbox

        public new HTMLTestCheckBox CheckBox()
        {
            return CheckBox(String.Empty);
        }
        public new HTMLTestCheckBox CheckBox(string name)
        {
            return CheckBoxs(name)[0];
        }

        public new HTMLTestCheckBox CheckBox(TestProperty[] properties)
        {
            return CheckBoxs(properties)[0];
        }

        public new HTMLTestCheckBox[] CheckBoxs()
        {
            return CheckBoxs(String.Empty);
        }

        public new HTMLTestCheckBox[] CheckBoxs(string name)
        {
            GetMapObjects(HTMLTestObjectType.CheckBox, name);
            HTMLTestCheckBox[] tmp = new HTMLTestCheckBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestCheckBox[] CheckBoxs(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.CheckBox, properties);
            HTMLTestCheckBox[] tmp = new HTMLTestCheckBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region droplist

        public new HTMLTestDropList DropList()
        {
            return DropList(String.Empty);
        }

        public new HTMLTestDropList DropList(string name)
        {
            return DropLists(name)[0];
        }

        public new HTMLTestDropList DropList(TestProperty[] properties)
        {
            return DropLists(properties)[0];
        }

        public new HTMLTestDropList[] DropLists()
        {
            return DropLists(String.Empty);
        }

        public new HTMLTestDropList[] DropLists(string name)
        {
            GetMapObjects(HTMLTestObjectType.DropList, name);
            HTMLTestDropList[] tmp = new HTMLTestDropList[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestDropList[] DropLists(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.DropList, properties);
            HTMLTestDropList[] tmp = new HTMLTestDropList[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region image

        public new HTMLTestImage Image()
        {
            return Image(String.Empty);
        }

        public new HTMLTestImage Image(string name)
        {
            return Images(name)[0];
        }

        public new HTMLTestImage Image(TestProperty[] properties)
        {
            return Images(properties)[0];
        }

        public new HTMLTestImage[] Images()
        {
            return Images(String.Empty);
        }

        public new HTMLTestImage[] Images(string name)
        {
            GetMapObjects(HTMLTestObjectType.Image, name);
            HTMLTestImage[] tmp = new HTMLTestImage[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestImage[] Images(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.Image, properties);
            HTMLTestImage[] tmp = new HTMLTestImage[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region label

        public new HTMLTestLabel Label()
        {
            return Label(String.Empty);
        }

        public new HTMLTestLabel Label(string name)
        {
            return Labels(name)[0];
        }

        public new HTMLTestLabel Label(TestProperty[] properties)
        {
            return Labels(properties)[0];
        }

        public new HTMLTestLabel[] Labels()
        {
            return Labels(String.Empty);
        }

        public new HTMLTestLabel[] Labels(string name)
        {
            GetMapObjects(HTMLTestObjectType.Label, name);
            HTMLTestLabel[] tmp = new HTMLTestLabel[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestLabel[] Labels(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.Label, properties);
            HTMLTestLabel[] tmp = new HTMLTestLabel[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region link

        public new HTMLTestLink Link()
        {
            return Link(String.Empty);
        }

        public new HTMLTestLink Link(string name)
        {
            return Links(name)[0];
        }

        public new HTMLTestLink Link(TestProperty[] properties)
        {
            return Links(properties)[0];
        }

        public new HTMLTestLink[] Links()
        {
            return Links(String.Empty);
        }

        public new HTMLTestLink[] Links(string name)
        {
            GetMapObjects(HTMLTestObjectType.Link, name);
            HTMLTestLink[] tmp = new HTMLTestLink[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestLink[] Links(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.Link, properties);
            HTMLTestLink[] tmp = new HTMLTestLink[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region listbox

        public new HTMLTestListBox ListBox()
        {
            return ListBox(String.Empty);
        }

        public new HTMLTestListBox ListBox(string name)
        {
            return ListBoxs(name)[0];
        }

        public new HTMLTestListBox ListBox(TestProperty[] properties)
        {
            return ListBoxs(properties)[0];
        }

        public new HTMLTestListBox[] ListBoxs()
        {
            return ListBoxs(String.Empty);
        }

        public new HTMLTestListBox[] ListBoxs(string name)
        {
            GetMapObjects(HTMLTestObjectType.ListBox, name);
            HTMLTestListBox[] tmp = new HTMLTestListBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestListBox[] ListBoxs(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.ListBox, properties);
            HTMLTestListBox[] tmp = new HTMLTestListBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region radiobox

        public new HTMLTestRadioBox RadioBox()
        {
            return RadioBox(String.Empty);
        }

        public new HTMLTestRadioBox RadioBox(string name)
        {
            return RadioBoxs(name)[0];
        }

        public new HTMLTestRadioBox RadioBox(TestProperty[] properties)
        {
            return RadioBoxs(properties)[0];
        }

        public new HTMLTestRadioBox[] RadioBoxs()
        {
            return RadioBoxs(String.Empty);
        }

        public new HTMLTestRadioBox[] RadioBoxs(string name)
        {
            GetMapObjects(HTMLTestObjectType.RadioBox, name);
            HTMLTestRadioBox[] tmp = new HTMLTestRadioBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestRadioBox[] RadioBoxs(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.RadioBox, properties);
            HTMLTestRadioBox[] tmp = new HTMLTestRadioBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region table

        public new HTMLTestTable Table()
        {
            return Table(String.Empty);
        }

        public new HTMLTestTable Table(String name)
        {
            return Tables(name)[0];
        }

        public new HTMLTestTable Table(TestProperty[] properties)
        {
            return Tables(properties)[0];
        }

        public new HTMLTestTable[] Tables()
        {
            return Tables(String.Empty);
        }

        public new HTMLTestTable[] Tables(String name)
        {
            GetMapObjects(HTMLTestObjectType.Table, name);
            HTMLTestTable[] tmp = new HTMLTestTable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestTable[] Tables(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.Table, properties);
            HTMLTestTable[] tmp = new HTMLTestTable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region form

        public HTMLTestForm Form()
        {
            return Form(String.Empty);
        }

        public HTMLTestForm Form(String name)
        {
            return Forms(name)[0];
        }

        public HTMLTestForm Form(TestProperty[] properties)
        {
            return Forms(properties)[0];
        }

        public HTMLTestForm[] Forms()
        {
            return Forms(String.Empty);
        }

        public HTMLTestForm[] Forms(String name)
        {
            GetMapObjects(HTMLTestObjectType.Form, name);
            HTMLTestForm[] tmp = new HTMLTestForm[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public HTMLTestForm[] Forms(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.Form, properties);
            HTMLTestForm[] tmp = new HTMLTestForm[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region activex

        public HTMLTestActiveXObject ActiveX()
        {
            return ActiveX(String.Empty);
        }

        public HTMLTestActiveXObject ActiveX(String name)
        {
            return ActiveXs(name)[0];
        }

        public HTMLTestActiveXObject ActiveX(TestProperty[] properties)
        {
            return ActiveXs(properties)[0];
        }

        public HTMLTestActiveXObject[] ActiveXs()
        {
            return ActiveXs(String.Empty);
        }

        public HTMLTestActiveXObject[] ActiveXs(String name)
        {
            GetMapObjects(HTMLTestObjectType.ActiveX, name);
            HTMLTestActiveXObject[] tmp = new HTMLTestActiveXObject[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public HTMLTestActiveXObject[] ActiveXs(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.ActiveX, properties);
            HTMLTestActiveXObject[] tmp = new HTMLTestActiveXObject[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region dialog

        public HTMLTestDialog Dialog()
        {
            return Dialog(String.Empty);
        }

        public HTMLTestDialog Dialog(String name)
        {
            return Dialogs(name)[0];
        }

        public HTMLTestDialog Dialog(TestProperty[] properties)
        {
            return Dialogs(properties)[0];
        }

        public HTMLTestDialog[] Dialogs()
        {
            return Dialogs(String.Empty);
        }

        public HTMLTestDialog[] Dialogs(String name)
        {
            GetMapObjects(HTMLTestObjectType.Dialog, name);
            HTMLTestDialog[] tmp = new HTMLTestDialog[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public HTMLTestDialog[] Dialogs(TestProperty[] properties)
        {
            GetMapObjects(HTMLTestObjectType.Dialog, properties);
            HTMLTestDialog[] tmp = new HTMLTestDialog[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region Any type

        public new HTMLTestGUIObject AnyObject()
        {
            return AnyObject(String.Empty);
        }

        public new HTMLTestGUIObject AnyObject(String name)
        {
            return AnyObject(String.Empty, name);
        }

        public new HTMLTestGUIObject AnyObject(String type, String name)
        {
            return AnyObjects(type, name)[0];
        }

        public new HTMLTestGUIObject AnyObject(TestProperty[] properties)
        {
            return AnyObjects(properties)[0];
        }

        public new HTMLTestGUIObject AnyObject(String type, TestProperty[] properties)
        {
            return AnyObjects(type, properties)[0];
        }

        public new HTMLTestGUIObject[] AnyObjects()
        {
            return AnyObjects(String.Empty);
        }

        public new HTMLTestGUIObject[] AnyObjects(String name)
        {
            return AnyObjects(String.Empty, name);
        }

        public new HTMLTestGUIObject[] AnyObjects(String type, String name)
        {
            GetMapObjects(type, name);
            HTMLTestGUIObject[] tmp = new HTMLTestGUIObject[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestGUIObject[] AnyObjects(TestProperty[] properties)
        {
            return AnyObjects(String.Empty, properties);
        }

        public new HTMLTestGUIObject[] AnyObjects(String type, TestProperty[] properties)
        {
            GetMapObjects(type, properties);
            HTMLTestGUIObject[] tmp = new HTMLTestGUIObject[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #endregion
    }
}
