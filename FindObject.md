# How To Find An Object #

This wiki page will show you how to find an object.


## Find Common Object ##

Common used objects like "Button", "Textbox" ect. you can find it by it's visible text or properties.

For example: the "Google Search" button, it's HTML code is like this
```
<input name=btnG type=submit value="Google Search">.
```

To find the "Google Search" button, you can find it by it's visible text:
```
            TestSession ts = new HTMLTestSession();
            ...
            ts.Objects.Button("Google Search").Click(); 
```

Or you can find it by it's properties:
```
            ts.Objects.Button("name=btnG;type=submit").Click(); 
```

To find an object by properties, you should pass a string parameter like "propertyName1=propertyValue1;propertyName2=propertyValue2...", for example: "name=btnG;type=submit".


## Find any Object ##

If you can not find an object by it's visible text and properties. AutoTester.net support some other APIs to find an object. You can find these APIs in "TestSession.Objects.ObjectPool"

```
 /* Object GetObjectByIndex(int index);
         * index means the 1st object, the 2nd object.
         */
        TestObject GetObjectByIndex(int index);

        /* Object GetObjectByProperty(string property, string value);
         * find object by an internal property, eg: find a button which .id is "btn1"
         */
        TestObject[] GetObjectsByProperties(TestProperty[] properties);

        /* Object GetObjectByID(string id);
         * find object by ".id"
         */
        TestObject GetObjectByID(string id);

        /* Object GetObjectByName(string name);
         * find object by ".name."
         */
        TestObject[] GetObjectsByName(string name);

        /* Object GetObjectByType(string type, string values, int index);
         * find object by "type", eg: GetObjectByType("button","OK",0)
         */
        TestObject[] GetObjectsByType(string type, TestProperty[] properties);


        /* GetObjectByPoint(int x, int y);
         * find object at specfic point.
         */
        TestObject GetObjectByPoint(int x, int y);

        /* Object GetObjectByRect(int top, int left, int width, int height, string typeStr, bool isPercent);
         * find object by specfic rect, type means what type of the object you want return, eg: button
         */
        TestObject GetObjectByRect(int top, int left, int width, int height, string type, bool isPercent);

        /* Object[] GetAllObjects();
         * return all objects in the object pool.
         */
        TestObject[] GetAllObjects();
```

You can use these APIs like this:
```
            TestSession ts = new HTMLTestSession();
            ...
            //find the object which position is 100,100
            TestObject obj = ts.Objects.ObjectPool.GetObjectByPoint(100, 100);
```