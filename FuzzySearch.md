# Fuzzy Search #

Fuzzy Search is a powerful function to let you find an object.


## Details ##

In common situation, if you want to find an object, your need to pass the correct properties to API.

For example, if you try to find the "Google Search" button, you need to write code like this:
```
test.Objects.Button("Google Search").Click(); // work well.
```

But because of your careless, you write code like this:
```
test.Objects.Button("Google Searc").Click(); //can not work!!!
```
Of coz, you can not get anything because the button is "Google Search" not "Google Searc".

Fuzzy Search can make it work for you, after you enable Fuzzy search, AutoTester.net will find an object even it is not 100% match. It will return the most similar object to you.
That means `  test.Objects.Button("Google Searc").Click(); ` can also return the "Google Search" button to you.


## How to enable/disable fuzzy search ##

By passing the parameter "true" to FuzzySearch, passing "false" to disable Fuzzy search.

Note that Fuzzy Seach is disabled by default!!!
```
            test.Objects.ObjectPool.FuzzySearch = true;
            test.Objects.Button("Google Searc").Click(); // now it can work correctly.
```