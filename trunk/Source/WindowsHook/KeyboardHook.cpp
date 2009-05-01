#include "StdAfx.h"

#include "KeyboardHook.h"

using namespace Shrinerain::AutoTester::WindowsHook;


static HHOOK _keyBoardHook = NULL;

KeyboardHook::KeyboardHook()
{
}

bool KeyboardHook::InstallHook(IntPtr windowHandle)
{
	UninstallHook();

	DWORD tid = GetWindowThreadProcessId((HWND)windowHandle.ToPointer(), NULL);
	HINSTANCE hinstDLL; 
	hinstDLL = LoadLibrary((LPCTSTR) _T("WindowsHook.dll"));
	HOOKPROC llMouseProc = (HOOKPROC)GetProcAddress(hinstDLL, "LowLevelKeyProc");

	if(hinstDLL != NULL)
	{
		_keyBoardHook = SetWindowsHookEx(WH_KEYBOARD_LL, llMouseProc,hinstDLL,0);
	}

	return _keyBoardHook != NULL;
}

void KeyboardHook::UninstallHook()
{
	if(_keyBoardHook != NULL)
	{
		UnhookWindowsHookEx(_keyBoardHook);
	}
}

void KeyboardHook::RaiseEvent(int key, int x, int y)
{
	OnKeyEvent(key,x,y);
}

KeyboardHook^ KeyboardHook::GetInstance()
{
	if(m_instance == nullptr)
	{
		m_instance = gcnew KeyboardHook();
	}

	return m_instance;
}

extern "C" LRESULT CALLBACK LowLevelKeyProc(int nCode, WPARAM msg, LPARAM lParam)
{
	if (nCode == HC_ACTION)
	{

	}

	return CallNextHookEx(_keyBoardHook, nCode, msg, lParam);  
}

