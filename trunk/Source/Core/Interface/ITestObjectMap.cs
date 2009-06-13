/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ITestObjectMap.cs
*
* Description: This interface defines some common objects we used in
*              testing, user do not need to get object by using 
*              object pool API if user just want to find common objects.               
*    
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface ITestObjectMap
    {
        #region test object

        IClickable Button();
        IClickable[] Buttons();
        IClickable Button(string name);
        IClickable[] Buttons(string name);

        IInputable TextBox();
        IInputable[] TextBoxs();
        IInputable TextBox(string name);
        IInputable[] TextBoxs(string name);

        ICheckable CheckBox();
        ICheckable[] CheckBoxs();
        ICheckable CheckBox(string name);
        ICheckable[] CheckBoxs(string name);

        ISelectable ComboBox();
        ISelectable[] ComboBoxs();
        ISelectable ComboBox(string name);
        ISelectable[] ComboBoxs(string name);

        IPicture Image();
        IPicture[] Images();
        IPicture Image(string name);
        IPicture[] Images(string name);

        IText Label();
        IText[] Labels();
        IText Label(string name);
        IText[] Labels(string name);

        IClickable Link();
        IClickable[] Links();
        IClickable Link(string name);
        IClickable[] Links(string name);

        ISelectable ListBox();
        ISelectable[] ListBoxs();
        ISelectable ListBox(string name);
        ISelectable[] ListBoxs(string name);

        ICheckable RadioBox();
        ICheckable[] RadioBoxs();
        ICheckable RadioBox(string name);
        ICheckable[] RadioBoxs(string name);

        ITable Table();
        ITable[] Tables();
        ITable Table(String name);
        ITable[] Tables(String name);

        IClickable Menu();
        IClickable[] Menus();
        IClickable Menu(String name);
        IClickable[] Menus(String name);

        IClickable Tab();
        IClickable[] Tabs();
        IClickable Tab(String name);
        IClickable[] Tabs(String name);

        IVisible AnyObject();
        IVisible[] AnyObjects();
        IVisible AnyObject(String name);
        IVisible[] AnyObjects(String name);

        #endregion
    }
}
