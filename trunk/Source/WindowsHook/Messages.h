#pragma once

static unsigned int WM_GETPROXY =			::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_GETPROXY");
static unsigned int WM_ISMANAGED =			::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_ISMANAGED");
static unsigned int WM_RELEASEMEM =			::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_RELEASEMEMORY");
static unsigned int WM_SETMGDPROPERTY =		::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_SETMGDPROPERTY");
static unsigned int WM_GETMGDPROPERTY =		::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_GETMGDPROPERTY");
static unsigned int WM_RESETMGDPROPERTY =	::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_RESETMGDPROPERTY");
static unsigned int WM_SUBSCRIBEEVENT =		::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_SUBSCRIBEEVENT");
static unsigned int WM_UNSUBSCRIBEEVENT =	::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_UNSUBSCRIBEEVENT");
static unsigned int WM_EVENTFIRED =			::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_EVENTFIRED");
static unsigned int WM_WINDOWSEVENTFIRED =  ::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_WINDOWSEVENTFIRED");
static unsigned int WM_WINDOWDESTROYED =	::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_WINDOWDESTROYED");
static unsigned int WM_HANDLECHANGED =		::RegisterWindowMessage(L"SHRINERAIN_AUTOTESTER_HANDLECHANGED");
