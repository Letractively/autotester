#pragma once

namespace Shrinerain{
	namespace AutoTester{
		namespace WindowsHook{

			public ref class EventHook {
			public:
				delegate void WindowsEventHandler(DWORD event, int hwnd,LONG idObject,LONG idChild);
				event WindowsEventHandler^ OnWindowsEventFired;
				void RaiseEvent(DWORD event,HWND hwnd,LONG idObject,LONG idChild);

				bool InstallHook(int processID);
				void UninstallHook();

				static EventHook^ GetInstance();

			private:
				static EventHook^ m_instance;
				EventHook();
			};
		}
	}
}