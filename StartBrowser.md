# Operations on a browser #

This wiki page will show you how to start/find/navigate browser.


## Details ##

You can start a browser by writing code like this:
```
TestSession test = new HTMLTestSession();
test.Browser.Start(); //start a browser with blank page.
or
test.Browser.Start("http://google.com"); //start browser then navigate to google.com
```

If you have already started a browser, you can find it by it's title or Url:
```
TestSession test = new HTMLTestSession();
test.Browser.Find("Google"); //by title it displayed.
```

After a browser is started or found, you can navigate to any url you want.
```
TestSession test = new HTMLTestSession();
test.Browser.Start();
test.Browser.Load("Google.com", true); //navigate to "google.com", wait until page load.
```