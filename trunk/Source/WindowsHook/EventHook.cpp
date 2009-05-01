#include "StdAfx.h"

#include "EventHook.h"

using namespace Shrinerain::AutoTester::WindowsHook;

static HWINEVENTHOOK _eventHookHandle = NULL;

VOID CALLBACK WinEventProc(HWINEVENTHOOK hWinEventHook, DWORD event,HWND hwnd,LONG idObject,LONG idChild,DWORD dwEventThread,DWORD dwmsEventTime);

EventHook::EventHook()
{
}

bool EventHook::InstallHook(int processID)
{
	CoInitialize(NULL);
	_eventHookHandle = SetWinEventHook(EVENT_MIN,EVENT_MAX,NULL,WinEventProc,processID,NULL,WINEVENT_OUTOFCONTEXT|WINEVENT_SKIPOWNPROCESS);

	return _eventHookHandle != NULL;
}

void EventHook::UninstallHook()
{
	if(_eventHookHandle != NULL)
	{
		UnhookWinEvent(_eventHookHandle);
		CoUninitialize();
	}
}

void EventHook::RaiseEvent(DWORD event,HWND hwnd,LONG idObject,LONG idChild)
{
	OnWindowsEventFired(event,(int)hwnd,idObject,idChild);
}

EventHook^ EventHook::GetInstance()
{
	if(m_instance == nullptr)
	{
		m_instance = gcnew EventHook();
	}

	return m_instance;
}

VOID CALLBACK WinEventProc(HWINEVENTHOOK hWinEventHook, DWORD event,HWND hwnd,LONG idObject,LONG idChild,DWORD dwEventThread,DWORD dwmsEventTime)
{
	EventHook::GetInstance()->RaiseEvent(event,hwnd,idObject,idChild);
}

