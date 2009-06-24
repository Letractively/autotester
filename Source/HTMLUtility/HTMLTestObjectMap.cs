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

        public new HTMLTestButton Button()
        {
            return Buttons()[0];
        }

        public new HTMLTestButton[] Buttons()
        {
            return Buttons(null);
        }

        public new HTMLTestButton Button(string name)
        {
            return Buttons(name)[0];
        }

        public new HTMLTestButton[] Buttons(string name)
        {
            GetMapObjects("button", name);
            HTMLTestButton[] tmp = new HTMLTestButton[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestTextBox TextBox()
        {
            return TextBoxs()[0];
        }

        public new HTMLTestTextBox[] TextBoxs()
        {
            return TextBoxs(null);
        }

        public new HTMLTestTextBox TextBox(string name)
        {
            return TextBoxs(name)[0];
        }

        public new HTMLTestTextBox[] TextBoxs(string name)
        {
            GetMapObjects("TextBox", name);
            HTMLTestTextBox[] tmp = new HTMLTestTextBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestCheckBox CheckBox()
        {
            return CheckBoxs()[0];
        }

        public new HTMLTestCheckBox[] CheckBoxs()
        {
            return CheckBoxs(null);
        }

        public new HTMLTestCheckBox CheckBox(string name)
        {
            return CheckBoxs(name)[0];
        }

        public new HTMLTestCheckBox[] CheckBoxs(string name)
        {
            GetMapObjects("CheckBox", name);
            HTMLTestCheckBox[] tmp = new HTMLTestCheckBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestComboBox ComboBox()
        {
            return ComboBoxs()[0];
        }

        public new HTMLTestComboBox[] ComboBoxs()
        {
            return ComboBoxs(null);
        }

        public new HTMLTestComboBox ComboBox(string name)
        {
            return ComboBoxs(name)[0];
        }

        public new HTMLTestComboBox[] ComboBoxs(string name)
        {
            GetMapObjects("ComboBox", name);
            HTMLTestComboBox[] tmp = new HTMLTestComboBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestImage Image()
        {
            return Images()[0];
        }

        public new HTMLTestImage[] Images()
        {
            return Images(null);
        }

        public new HTMLTestImage Image(string name)
        {
            return Images(name)[0];
        }

        public new HTMLTestImage[] Images(string name)
        {
            GetMapObjects("Image", name);
            HTMLTestImage[] tmp = new HTMLTestImage[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestLabel Label()
        {
            return Labels()[0];
        }

        public new HTMLTestLabel[] Labels()
        {
            return Labels(null);
        }

        public new HTMLTestLabel Label(string name)
        {
            return Labels(name)[0];
        }

        public new HTMLTestLabel[] Labels(string name)
        {
            GetMapObjects("Label", name);
            HTMLTestLabel[] tmp = new HTMLTestLabel[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestLink Link()
        {
            return Links()[0];
        }

        public new HTMLTestLink[] Links()
        {
            return Links(null);
        }

        public new HTMLTestLink Link(string name)
        {
            return Links(name)[0];
        }

        public new HTMLTestLink[] Links(string name)
        {
            GetMapObjects("Link", name);
            HTMLTestLink[] tmp = new HTMLTestLink[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestListBox ListBox()
        {
            return ListBoxs()[0];
        }

        public new HTMLTestListBox[] ListBoxs()
        {
            return ListBoxs(null);
        }

        public new HTMLTestListBox ListBox(string name)
        {
            return ListBoxs(name)[0];
        }

        public new HTMLTestListBox[] ListBoxs(string name)
        {
            GetMapObjects("ListBox", name);
            HTMLTestListBox[] tmp = new HTMLTestListBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestRadioBox RadioBox()
        {
            return RadioBoxs()[0];
        }

        public new HTMLTestRadioBox[] RadioBoxs()
        {
            return RadioBoxs(null);
        }

        public new HTMLTestRadioBox RadioBox(string name)
        {
            return RadioBoxs(name)[0];
        }

        public new HTMLTestRadioBox[] RadioBoxs(string name)
        {
            GetMapObjects("radiobox", name);
            HTMLTestRadioBox[] tmp = new HTMLTestRadioBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestTable Table()
        {
            return Tables()[0];
        }

        public new HTMLTestTable[] Tables()
        {
            return Tables(null);
        }

        public new HTMLTestTable Table(String name)
        {
            return Tables(name)[0];
        }

        public new HTMLTestTable[] Tables(String name)
        {
            GetMapObjects("Table", name);
            HTMLTestTable[] tmp = new HTMLTestTable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public new HTMLTestGUIObject AnyObject()
        {
            return AnyObjects()[0];
        }

        public new HTMLTestGUIObject AnyObject(String name)
        {
            return AnyObjects(name)[0];
        }

        public new HTMLTestGUIObject AnyObject(String type, String name)
        {
            return AnyObjects(type, name)[0];
        }

        public new HTMLTestGUIObject[] AnyObjects()
        {
            return AnyObjects(null);
        }

        public new HTMLTestGUIObject[] AnyObjects(String name)
        {
            GetMapObjects(null, name);
            HTMLTestGUIObject[] tmp = new HTMLTestGUIObject[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public new HTMLTestGUIObject[] AnyObjects(String type, String name)
        {
            GetMapObjects(type, name);
            HTMLTestGUIObject[] tmp = new HTMLTestGUIObject[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion
    }
}
