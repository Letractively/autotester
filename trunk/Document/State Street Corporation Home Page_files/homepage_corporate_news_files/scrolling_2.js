<!--

var showScrollbar = true;

if (iens6three){
document.write('</div></div>')
var scanacross=document.getElementById? document.getElementById("copy") : document.all.copy
var copyhoriz=scanacross.offsetHeight
}
else if (ns4three){
var scanacross=document.nsholder.document.nscopy
var copyhoriz=scanacross.clip.height
}

if (copyhoriz <= 150)
{
	showScrollbar = false;
}

function lower(){
if (window.raisevar) clearTimeout(raisevar)
if (iens6three&&parseInt(scanacross.style.top)>=(copyhoriz*(-1)+75))
scanacross.style.top=parseInt(scanacross.style.top)-speed
else if (ns4three&&scanacross.top>=(copyhoriz*(-1)+75))
scanacross.top-=speed
lowervar=setTimeout("lower()",100)
}

function raise(){
if (window.lowervar) clearTimeout(lowervar)
if (iens6three&&parseInt(scanacross.style.top)<=0)
scanacross.style.top=parseInt(scanacross.style.top)+speed
else if (ns4three&&scanacross.top<=0)
scanacross.top+=speed
raisevar=setTimeout("raise()",100)
}

function cease(){
if (window.raisevar) clearTimeout(raisevar)
if (window.lowervar) clearTimeout(lowervar)
}

function startup(){
cease()
if (iens6three)
scanacross.style.top=0
else if (ns4three)
scanacross.top=0
}

//Display scrollbars
if (showScrollbar == true)
{
	document.write('<div style="width:15; top:0; left:260; position:absolute;"><a href="#" onMouseOver="javascript:raise()" onMouseOut="javascript:cease()"><img src="/images/menu_up3.gif" alt="" width="13" height="13" border="0"></a><br><img src="/statestreet/en/images/spacer.gif" height="120" width="1" border="0"><br><a href="#" onMouseOver="javascript:lower()" onMouseOut="javascript:cease()"><img src="/images/menu_down3.gif" alt="" width="13" height="13" border="0"></a><br></div>');
}

//-->