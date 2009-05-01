#pragma once

//low level keyboard hook
#define WH_KEYBOARD_LL 13

using namespace System;

extern "C" LRESULT CALLBACK LowLevelKeyProc(int nCode, WPARAM wparam, LPARAM lparam);

namespace Shrinerain{
	namespace AutoTester{
		namespace WindowsHook{

			public ref class KeyboardHook {
			public:
				delegate void KeyEventHandler(int key, int x, int y);
				event KeyEventHandler^ OnKeyEvent;
				void RaiseEvent(int key, int x, int y);

				bool InstallHook(IntPtr windowHandle);
				void UninstallHook();

				static KeyboardHook^ GetInstance();

			private:
				static KeyboardHook^ m_instance;
				KeyboardHook();
			};
		}
	}
}