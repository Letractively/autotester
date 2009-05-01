// Commands.h

#pragma once
#include <list>
#include "Mem.h"
#include "Messages.h"
#include "ControlProxy.h"
#include "EventTargetWindow.h"
#include "CustomEventArgs.h"

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Reflection::Emit;

__declspec( dllexport )
int __stdcall MessageHookProc(int nCode, WPARAM wparam, LPARAM lparam);

BOOL CALLBACK EnumCallback(HWND handle, LPARAM arg);
VOID CALLBACK WinEventProc(HWINEVENTHOOK hWinEventHook,DWORD event,HWND hwnd,LONG idObject,LONG idChild,DWORD dwEventThread,DWORD dwmsEventTime);

namespace Shrinerain{
	namespace AutoTester{
		namespace WindowsHook{

			ref class EventRegister : Object {
			public:
				Control^	sourceWindow;
				int			eventCode;
				EventInfo^	eventInfo;
				IntPtr		targetEventReceiver;

				void OnEventFired(Object^ sender, EventArgs^ args);
			};

			/// <summary>
			/// This class sends and handles our custom window messages (see Messages.h)
			/// </summary>
			ref class Desktop : System::Object {
			private:
				static IntPtr _oatsHandle;
				//private ctor -- uncreatable.
				Desktop(){
				}

			public:
				static List<ControlProxy^>^ topLevelWindows = gcnew List<ControlProxy^>();
				static List<ControlProxy^>^ childWindows = gcnew List<ControlProxy^>();
				static List<DWORD>^ managedProcesses = gcnew List<DWORD>();
				static List<DWORD>^ unmanagedProcesses = gcnew List<DWORD>();
				static EventTargetWindow^ eventWindow = gcnew EventTargetWindow();
				static Dictionary<IntPtr, Dictionary<int, EventRegister^>^>^ eventCallbacks =
					gcnew Dictionary<IntPtr, Dictionary<int, EventRegister^>^>();
				static Dictionary<IntPtr, ControlProxy^>^ proxyCache =
					gcnew Dictionary<IntPtr, ControlProxy^>();
				static Dictionary<Type^, DynamicMethod^>^ eventMethodCache =
					gcnew Dictionary<Type^, DynamicMethod^>();
				static int eventTypeCount = 0;
				static MethodInfo^ eventCallback = nullptr;

				static bool EnableHook(IntPtr windowHandle);
				static void DisableHook();

				static void SubscribeEvent(Control^ w, IntPtr eventWindow, String^ eventName, int eventCode);
				static void UnsubscribeEvent(Control^ w, int eventCode);
				static Object^ GetEventHandler(Type^ eventHandlerType, Object^ instance);

				static bool IsManagedProcess(DWORD procid);
				static Object^ SendMarshaledMessage(IntPtr hWnd, UINT Msg, Object^ parameter) {
					return SendMarshaledMessage(hWnd, Msg, parameter, true);
				}
				static Object^ SendMarshaledMessage(IntPtr hWnd, UINT Msg, Object^ parameter, bool hookRequired);
				static void OnMessage(int nCode, WPARAM wparam, LPARAM lparam);

				static ControlProxy^ GetProxy(System::IntPtr windowHandle);
				static array<ControlProxy^>^ GetTopLevelWindows();
			};
		}
	}
}