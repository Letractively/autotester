#pragma once

//low level mouse hook 
#define WH_MOUSE_LL 14

using namespace System;

extern "C" LRESULT CALLBACK LowLevelMouseProc(int nCode, WPARAM wparam, LPARAM lparam);

namespace Shrinerain{
	namespace AutoTester{
		namespace WindowsHook{

			public ref class MouseHook {
			public:
				delegate void MouseEventHandler(int key, int x, int y);
				event MouseEventHandler^ OnMouseEvent;
				void RaiseEvent(int key, int x, int y);

				bool InstallHook(IntPtr windowHandle);
				void UninstallHook();

				static MouseHook^ GetInstance();

			private:
				static MouseHook^ m_instance;
				MouseHook();
			};
		}
	}
}