/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ObjectCache.cs
*
* Description: ObjectCache use a hashtable to store used object.
*              If we will use the object at 2nd time, we can get the 
*              object from cache directly. 
*
* History: 2007/12/20 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Shrinerain.AutoTester.Helper
{
    public sealed class ObjectCache
    {

        #region fields

        //flag to specify if we will use cache.
        private static bool _useCache = true;

        //we use a hashtable to store our test objects.
        private static Dictionary<String, Object> _testObjectCache = new Dictionary<string, Object>();

        #endregion

        #region properties

        public static bool UseCache
        {
            get { return ObjectCache._useCache; }
            set { ObjectCache._useCache = value; }
        }

        #endregion

        #region methods

        #region ctor

        private ObjectCache()
        {

        }

        #endregion

        #region public methods

        /*  void InsertObjectToCache(string info, TestObject testObj)
         *  insert a test object to the cache.
         */
        public static void InsertObjectToCache(string key, Object testObj)
        {
            if (_useCache)
            {
                if (!String.IsNullOrEmpty(key))
                {
                    if (_testObjectCache.ContainsKey(key))
                    {
                        _testObjectCache.Remove(key);
                    }

                    _testObjectCache.Add(key, testObj);
                }
            }
        }


        /* bool TryGetObjectFromCache(string info)
        *  try to get a test object from the cache.
        */
        public static bool TryGetObjectFromCache(string key, out Object testObj)
        {
            testObj = null;

            if (_useCache)
            {

                if (String.IsNullOrEmpty(key))
                {
                    return false;
                }

                return _testObjectCache.TryGetValue(key, out testObj);

            }
            else
            {
                return false;
            }
        }

        /* void Remove(string key)
         * remove object by key
         */
        public static void Remove(string key)
        {
            _testObjectCache.Remove(key);
        }

        /* static void Clear()
         * Clear all objects.
         */
        public static void Clear()
        {
            _testObjectCache.Clear();
        }

        /* int GetObjectCount()
         * Get the count of test objects.
         */
        public static int GetObjectCount()
        {
            return _testObjectCache.Count;
        }

        /* string[] GetKeys()
         * return all keys in cache.
         */
        public static string[] GetKeys()
        {
            string[] res = new string[_testObjectCache.Keys.Count];

            int i = 0;

            foreach (string k in _testObjectCache.Keys)
            {
                res[i++] = k;
            }

            return res;
        }

        #endregion

        #region private methods

        #endregion

        #endregion

    }
}
