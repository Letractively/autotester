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
        IClickable Button(string description);
        IClickable Button(TestProperty[] properties);
        IClickable[] Buttons();
        IClickable[] Buttons(string description);
        IClickable[] Buttons(TestProperty[] properties);

        IInputable TextBox();
        IInputable TextBox(string description);
        IInputable TextBox(TestProperty[] properties);
        IInputable[] TextBoxs();
        IInputable[] TextBoxs(string description);
        IInputable[] TextBoxs(TestProperty[] properties);

        ICheckable CheckBox();
        ICheckable CheckBox(string description);
        ICheckable CheckBox(TestProperty[] propertiese);
        ICheckable[] CheckBoxs();
        ICheckable[] CheckBoxs(string description);
        ICheckable[] CheckBoxs(TestProperty[] properties);

        IComboBox ComboBox();
        IComboBox ComboBox(string description);
        IComboBox ComboBox(TestProperty[] propertiese);
        IComboBox[] ComboBoxs();
        IComboBox[] ComboBoxs(string description);
        IComboBox[] ComboBoxs(TestProperty[] properties);

        ISelectable DropList();
        ISelectable DropList(string description);
        ISelectable DropList(TestProperty[] properties);
        ISelectable[] DropLists();
        ISelectable[] DropLists(string description);
        ISelectable[] DropLists(TestProperty[] properties);

        IPicture Image();
        IPicture Image(string description);
        IPicture Image(TestProperty[] properties);
        IPicture[] Images();
        IPicture[] Images(string description);
        IPicture[] Images(TestProperty[] properties);

        IText Label();
        IText Label(string description);
        IText Label(TestProperty[] properties);
        IText[] Labels();
        IText[] Labels(string description);
        IText[] Labels(TestProperty[] properties);

        IClickable Link();
        IClickable Link(string description);
        IClickable Link(TestProperty[] properties);
        IClickable[] Links();
        IClickable[] Links(string description);
        IClickable[] Links(TestProperty[] properties);

        ISelectable ListBox();
        ISelectable ListBox(string description);
        ISelectable ListBox(TestProperty[] properties);
        ISelectable[] ListBoxs();
        ISelectable[] ListBoxs(string description);
        ISelectable[] ListBoxs(TestProperty[] properties);

        ICheckable RadioBox();
        ICheckable RadioBox(string description);
        ICheckable RadioBox(TestProperty[] properties);
        ICheckable[] RadioBoxs();
        ICheckable[] RadioBoxs(string description);
        ICheckable[] RadioBoxs(TestProperty[] properties);

        ITable Table();
        ITable Table(String description);
        ITable Table(TestProperty[] properties);
        ITable[] Tables();
        ITable[] Tables(String description);
        ITable[] Tables(TestProperty[] properties);

        IClickable Menu();
        IClickable Menu(String description);
        IClickable Menu(TestProperty[] properties);
        IClickable[] Menus();
        IClickable[] Menus(String description);
        IClickable[] Menus(TestProperty[] properties);

        IClickable Tab();
        IClickable Tab(String description);
        IClickable Tab(TestProperty[] properties);
        IClickable[] Tabs();
        IClickable[] Tabs(String description);
        IClickable[] Tabs(TestProperty[] properties);

        IVisible AnyObject();
        IVisible AnyObject(String description);
        IVisible AnyObject(TestProperty[] properties);
        IVisible AnyObject(String type, String description);
        IVisible AnyObject(String type, TestProperty[] properties);
        IVisible[] AnyObjects();
        IVisible[] AnyObjects(String description);
        IVisible[] AnyObjects(TestProperty[] properties);
        IVisible[] AnyObjects(String type, String description);
        IVisible[] AnyObjects(String type, TestProperty[] properties);

        ITestObjectPool ObjectPool { get; }

        #endregion
    }
}
