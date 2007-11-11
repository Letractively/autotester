/////////////////////////////////////////////////////////////////////////////////////
// Site functions
/////////////////////////////////////////////////////////////////////////////////////
	
	// Configure these constants based on the specific site
	var DOMAIN_EXTENSION = "com";
	var DEFAULT_LANG_DIR = "";
	
	// Set host variable based on the location of the page being viewed
	var myLocation = location.hostname;
	
	if(myLocation == "agsosg57.it.statestr.com")
	{
		var hostName = 'http://agsosg57.it.statestr.com';
	} 
	else if(myLocation == "svaw1071.statestr.com") {
		var hostName = 'http://svaw1071.statestr.com';
	}
	else if(myLocation == "pws-uat.statestreet." + DOMAIN_EXTENSION || myLocation == "itsdev01.statestr.com") {
		var hostName = 'http://pws-uat.statestreet.' + DOMAIN_EXTENSION;
	}
	else if(myLocation == "contact.statestreet.com") { 
    	var hostName = 'http://www.statestreet.com';
	}	
	else 
	{
		var hostName = 'http://www.statestreet.com';
	}
	
	// This string stores the domain used for cookies on the site.
	var domainName = hostName.substr(hostName.lastIndexOf('.',hostName.lastIndexOf('.')-1));

	// The languageDirectory string stores the base dirctory used for language the visitor is
	// currently veiwing the site with.  This should be set on each page before loading this JS file.
	// If the variable is not already declared on the page the defualt language will be used.
	if ((typeof languageDirectory) == 'undefined') {
		languageDirectory = DEFAULT_LANG_DIR;
	}

	// This string contains the two character iso code for the country the site is representing
	var countryCode = "us";

	// Includes the proper language specific system file
	document.write("<SCR" + "IPT LANGUAGE='JavaScript1.2' SRC='" + hostName + languageDirectory + "/system/system_language.js' TYPE='text/javascript'></SCR" + "IPT>");
	document.write("<SCR" + "IPT LANGUAGE='JavaScript1.2' SRC='" + hostName + languageDirectory + "/system/pagelauncher.js' TYPE='text/javascript'></SCR" + "IPT>");
	document.write("<SCR" + "IPT LANGUAGE='JavaScript1.2' SRC='" + hostName + languageDirectory + "/navigation/navigation.js' TYPE='text/javascript'></SCR" + "IPT>");
	
	//Centrport tracking code
	function runCentrport() {
		if ( hostName == 'http://www.statestreet.com' ) {
		//if ( hostName == 'http://pws-uat.statestreet.' + DOMAIN_EXTENSION ) {
			_cp_custom_array=new Array();
			_cp_cc='SSB';
			_cp_pc='SSB101';
			_cp_chc='SSBLITE';
			_cp_svr='wsl.centrport.net';
			_cp_send_cookie='0';
			document.write('<scr'+'ipt type="text/javascript" src="'+location.protocol+'//'+_cp_svr+'/wsl.js"></scr'+'ipt>');
			//alert('CENTRPORT ON');
		}
	}	
	
	// This function redirects the browser to a version of the current page in an alternate language
	function toggleLanguage(currentLang, newLang)
	{
		if ((typeof dynamicPage)=='undefined' || !dynamicPage)
		{
			if (location.hostname == "search.statestreet.com")
			{
				searchExpression = new RegExp("/" + countryCode + "/" + currentLang + "/","i");
				myUrl = new String(document.location);

				if (myUrl.search(searchExpression) > -1)
				{
					myUrl = myUrl.replace(searchExpression,"/" + countryCode + "/"+ newLang + "/");
				}
				else return;
			}
			else
			{
				searchExpression = new RegExp("/" + currentLang + "/","i");
				myUrl = new String(document.location);

				if (myUrl.search(searchExpression) > -1)
				{
					myUrl = myUrl.replace(searchExpression,"/"+ newLang + "/");
				}
				else return;
			}
		}
		else
		{
			searchExpression = new RegExp("lang=" + currentLang,"i");
			myUrl = new String(document.location);

			if (myUrl.search(searchExpression) > -1)
			{
				myUrl = myUrl.replace(searchExpression,"lang=" + newLang);
			}
			else return;
		}
		document.location = myUrl;		
	}


	// This function is used to submit a search form
	function submitSearch(searchForm) {
		searchForm.action="http://search.statestreet.com/www/query.html";
		searchForm.method="get";
		searchForm.submit();
	}
		
	// Called when a user selects an international site from the drop down at the top of site pages.
	// Sends the user's browser to the selected site.
	function jumpSites(myForm)
	{
		dropdown=myForm.internationalsite;
		url=dropdown.options[dropdown.selectedIndex].value;
		site=dropdown.options[dropdown.selectedIndex].text;

		if (dropdown.selectedIndex == 0)
		{
			return false;
		}
		else if (dropdown.selectedIndex == 1 || dropdown.selectedIndex == 5 || dropdown.selectedIndex == 10 || dropdown.selectedIndex == 12 || dropdown.selectedIndex == 13 || dropdown.selectedIndex == 14 || dropdown.selectedIndex == 15 || dropdown.selectedIndex == 24 || dropdown.selectedIndex == 28 || dropdown.selectedIndex == 29)
		{
			window.open(url);
		}
		else
		{
			openListing (url,'listing');
		}
		return false;
	}
	
	// Called to open a constrained window that accepts different parameters for each browser type
	function openWindow(url, name, iePC, ieMac, nsPC, nsMac, all)
	{
		var myDetector = new BrowserDetector(navigator.userAgent);
		features = " ";

		if(myDetector.platform.substr(0,7) == "Windows")
		{
			if ( myDetector.browser == "IE")
			{
				features = iePC;
			}
			else
			{
				features = nsPC;
			}
		}
		else
		{
			if ( myDetector.browser == "IE")
			{
				features = ieMac;
			}
			else
			{
				features = nsMac;
			}

		}

		if (all != null)
		{
			features+=","+all;
		}
		window.open(url, name, features);
	}

	// Open a basic listing page
	function openListing (url,name)
	{
		openWindow(url,name,'height=600,width=658','height=584,width=627','height=602,width=654','height=595,width=640','scrollbars=yes');
	}	
	
	//This function reloads content in the target window, also can close the window making the request
	function reloadParentWindow(url,closeSelf) {
		
		self.opener.location = url;
		if(closeSelf) self.close();
		
	}


/////////////////////////////////////////////////////////////////////////////////////
// Macromedia default functions
/////////////////////////////////////////////////////////////////////////////////////

	/*function MM_swapImgRestore() { //v3.0
		var i,x,a=document.MM_sr; for(i=0;a&&i<a.length&&(x=a[i])&&x.oSrc;i++) x.src=x.oSrc;
	}

	function MM_preloadImages() { //v3.0
		var d=document; if(d.images){ if(!d.MM_p) d.MM_p=new Array();
		var i,j=d.MM_p.length,a=MM_preloadImages.arguments; for(i=0; i<a.length; i++)
		if (a[i].indexOf("#")!=0){ d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];}}
	}

	function MM_swapImage() { //v3.0
		var i,j=0,x,a=MM_swapImage.arguments; document.MM_sr=new Array; for(i=0;i<(a.length-2);i+=3)
		if ((x=MM_findObj(a[i]))!=null){document.MM_sr[j++]=x; if(!x.oSrc) x.oSrc=x.src; x.src=a[i+2];}
	}

	function MM_findObj(n, d) { //v4.01
		var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length) {
		d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);}
		if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
		for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=MM_findObj(n,d.layers[i].document);
		if(!x && d.getElementById) x=d.getElementById(n); return x;
	}

	function MM_showHideLayers() { //v6.0
		var i,p,v,obj,args=MM_showHideLayers.arguments;
		for (i=0; i<(args.length-2); i+=3) if ((obj=MM_findObj(args[i]))!=null) { v=args[i+2];
		if (obj.style) { obj=obj.style; v=(v=='show')?'visible':(v=='hide')?'hidden':v; }
		obj.visibility=v; }
	}*/		

	function MM_reloadPage(init) {  //reloads the window if Nav4 resized
	  if (init==true) with (navigator) {if ((appName=="Netscape")&&(parseInt(appVersion)==4)) {
	    document.MM_pgW=innerWidth; document.MM_pgH=innerHeight; onresize=MM_reloadPage; }}
	  else if (innerWidth!=document.MM_pgW || innerHeight!=document.MM_pgH) location.reload();
	}
	MM_reloadPage(true);

	function MM_findObj(n, d) { //v4.01
	  var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length) {
	    d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);}
	  if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
	  for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=MM_findObj(n,d.layers[i].document);
	  if(!x && d.getElementById) x=d.getElementById(n); return x;
	}

	function MM_showHideLayers() { //v6.0
	  var i,p,v,obj,args=MM_showHideLayers.arguments;
	  for (i=0; i<(args.length-2); i+=3) if ((obj=MM_findObj(args[i]))!=null) { v=args[i+2];
	    if (obj.style) { obj=obj.style; v=(v=='show')?'visible':(v=='hide')?'hidden':v; }
	    obj.visibility=v; }
	}


	function MM_callJS(jsStr) { //v2.0
	  return eval(jsStr)
	}

	function MM_preloadImages() { //v3.0
	  var d=document; if(d.images){ if(!d.MM_p) d.MM_p=new Array();
	    var i,j=d.MM_p.length,a=MM_preloadImages.arguments; for(i=0; i<a.length; i++)
	    if (a[i].indexOf("#")!=0){ d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];}}
	}

	function MM_swapImgRestore() { //v3.0
	  var i,x,a=document.MM_sr; for(i=0;a&&i<a.length&&(x=a[i])&&x.oSrc;i++) x.src=x.oSrc;
	}

	function MM_swapImage() { //v3.0
	  var i,j=0,x,a=MM_swapImage.arguments; document.MM_sr=new Array; for(i=0;i<(a.length-2);i+=3)
	   if ((x=MM_findObj(a[i]))!=null){document.MM_sr[j++]=x; if(!x.oSrc) x.oSrc=x.src; x.src=a[i+2];}
	}

/////////////////////////////////////////////////////////////////////////////////////
// Webmonkey utility functions
/////////////////////////////////////////////////////////////////////////////////////

/*
BrowserDetector()
Parses User-Agent string into useful info.

Source: Webmonkey Code Library
(http://www.hotwired.com/webmonkey/javascript/code_library/)

Author: Richard Blaylock
Author Email: blaylock@wired.com

Usage: var bd = new BrowserDetector(navigator.userAgent);
*/


	// Utility function to trim spaces from both ends of a string
	function Trim(inString) {
	  var retVal = "";
	  var start = 0;
	  while ((start < inString.length) && (inString.charAt(start) == ' ')) {
	    ++start;
	  }
	  var end = inString.length;
	  while ((end > 0) && (inString.charAt(end - 1) == ' ')) {
	    --end;
	  }
	  retVal = inString.substring(start, end);
	  return retVal;
	}
	
	function BrowserDetector(ua) {
	
	// Defaults
	  this.browser = "Unknown";
	  this.platform = "Unknown";
	  this.version = "";
	  this.majorver = "";
	  this.minorver = "";
	
	  uaLen = ua.length;
	
	// ##### Split into stuff before parens and stuff in parens
	  var preparens = "";
	  var parenthesized = "";
	
	  i = ua.indexOf("(");
	  if (i >= 0) {
	    preparens = Trim(ua.substring(0,i));
	        parenthesized = ua.substring(i+1, uaLen);
	        j = parenthesized.indexOf(")");
	        if (j >= 0) {
	          parenthesized = parenthesized.substring(0, j);
	        }
	  }
	  else {
	    preparens = ua;
	  }
	
	// ##### First assume browser and version are in preparens
	// ##### override later if we find them in the parenthesized stuff
	  var browVer = preparens;
	
	  var tokens = parenthesized.split(";");
	  var token = "";
	// # Now go through parenthesized tokens
	  for (var i=0; i < tokens.length; i++) {
	    token = Trim(tokens[i]);
	        //## compatible - might want to reset from Netscape
	        if (token == "compatible") {
	          //## One might want to reset browVer to a null string
	          //## here, but instead, we'll assume that if we don't
	          //## find out otherwise, then it really is Mozilla
	          //## (or whatever showed up before the parens).
	        //## browser - try for Opera or IE
	    }
	        else if (token.indexOf("MSIE") >= 0) {
	      browVer = token;
	    }
	    else if (token.indexOf("Opera") >= 0) {
	      browVer = token;
	    }
	        //'## platform - try for X11, SunOS, Win, Mac, PPC
	    else if ((token.indexOf("X11") >= 0) || (token.indexOf("SunOS") >= 0) ||
	(token.indexOf("Linux") >= 0)) {
	      this.platform = "Unix";
	        }
	    else if (token.indexOf("Win") >= 0) {
	      this.platform = token;
	        }
	    else if ((token.indexOf("Mac") >= 0) || (token.indexOf("PPC") >= 0)) {
	      this.platform = token;
	        }
	  }
	
	  var msieIndex = browVer.indexOf("MSIE");
	  if (msieIndex >= 0) {
	    browVer = browVer.substring(msieIndex, browVer.length);
	  }
	
	  var leftover = "";
	  if (browVer.substring(0, "Mozilla".length) == "Mozilla") {
	    this.browser = "Netscape";
	        leftover = browVer.substring("Mozilla".length+1, browVer.length);
	  }
	  else if (browVer.substring(0, "Lynx".length) == "Lynx") {
	    this.browser = "Lynx";
	        leftover = browVer.substring("Lynx".length+1, browVer.length);
	  }
	  else if (browVer.substring(0, "MSIE".length) == "MSIE") {
	    this.browser = "IE";
	    leftover = browVer.substring("MSIE".length+1, browVer.length);
	  }
	  else if (browVer.substring(0, "Microsoft Internet Explorer".length) ==
	"Microsoft Internet Explorer") {
	    this.browser = "IE"
	        leftover = browVer.substring("Microsoft Internet Explorer".length+1,
	browVer.length);
	  }
	  else if (browVer.substring(0, "Opera".length) == "Opera") {
	    this.browser = "Opera"
	    leftover = browVer.substring("Opera".length+1, browVer.length);
	  }
	
	  leftover = Trim(leftover);
	
	  // # Try to get version info out of leftover stuff
	  i = leftover.indexOf(" ");
	  if (i >= 0) {
	    this.version = leftover.substring(0, i);
	  }
	  else
	  {
	    this.version = leftover;
	  }
	  j = this.version.indexOf(".");
	  if (j >= 0) {
	    this.majorver = this.version.substring(0,j);
	    this.minorver = this.version.substring(j+1, this.version.length);
	  }
	  else {
	    this.majorver = this.version;
	  }
	} // function BrowserCap