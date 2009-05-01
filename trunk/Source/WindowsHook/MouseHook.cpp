#include "StdAfx.h"

#include "MouseHook.h"

using namespace Shrinerain::AutoTester::WindowsHook;


static HHOOK _mouseHook = NULL;

MouseHook::MouseHook()
{
}

bool MouseHook::InstallHook(IntPtr windowHandle)
{
	UninstallHook();

	DWORD tid = GetWindowThreadProcessId((HWND)windowHandle.ToPointer(), NULL);
	HINSTANCE hinstDLL; 
	hinstDLL = LoadLibrary((LPCTSTR) _T("WindowsHook.dll"));
	HOOKPROC llMouseProc = (HOOKPROC)GetProcAddress(hinstDLL, "LowLevelMouseProc");

	if(hinstDLL != NULL)
	{
		_mouseHook = SetWindowsHookEx(WH_MOUSE_LL, llMouseProc,hinstDLL,0);
	}

	return _mouseHook != NULL;
}

void MouseHook::UninstallHook()
{
	if(_mouseHook != NULL)
	{
		UnhookWindowsHookEx(_mouseHook);
	}
}

void MouseHook::RaiseEvent(int key, int x, int y)
{
	OnMouseEvent(key,x,y);
}

MouseHook^ MouseHook::GetInstance()
{
	if(m_instance == nullptr)
	{
		m_instance = gcnew MouseHook();
	}

	return m_instance;
}

extern "C" LRESULT CALLBACK LowLevelMouseProc(int nCode, WPARAM msg, LPARAM lParam)
{
	if (nCode == HC_ACTION)
	{
		if(msg == WM_LBUTTONDOWN)
		{
			MSLLHOOKSTRUCT* pp = (MSLLHOOKSTRUCT*)lParam;
			MouseHook::GetInstance()->RaiseEvent(msg,pp->pt.x,pp->pt.y);
		}
		else if(msg == WM_LBUTTONUP)
		{
			MSLLHOOKSTRUCT* pp = (MSLLHOOKSTRUCT*)lParam;
			MouseHook::GetInstance()->RaiseEvent(msg,pp->pt.x,pp->pt.y);
		}
		else if(msg == WM_RBUTTONDOWN)
		{
		}
		else if(msg == WM_RBUTTONUP)
		{
		}
	}

	return CallNextHookEx(_mouseHook, nCode, msg, lParam);  
}

