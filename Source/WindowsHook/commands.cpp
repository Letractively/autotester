// This is the main DLL file.

#include "stdafx.h"
#include "Commands.h"

static HHOOK _messageHookHandle = NULL;
static IntPtr _spiedHandle = IntPtr::Zero;

using namespace Shrinerain::AutoTester::WindowsHook;
//-----------------------------------------------------------------------------
//Spying Process functions follow
//-----------------------------------------------------------------------------
bool Desktop::EnableHook(IntPtr windowHandle) {

	HINSTANCE hinstDLL; 
	hinstDLL = LoadLibrary((LPCTSTR) _T("WindowsHook.dll")); 

	DisableHook();
	DWORD tid = GetWindowThreadProcessId((HWND)windowHandle.ToPointer(), NULL);
	_messageHookHandle = SetWindowsHookEx(WH_CALLWNDPROC,
		(HOOKPROC)GetProcAddress(hinstDLL, "MessageHookProc"),
		hinstDLL,
		tid);

	return _messageHookHandle != NULL;
}

void Desktop::DisableHook() {
	if (_messageHookHandle != NULL) {
		UnhookWindowsHookEx(_messageHookHandle);
		_messageHookHandle = NULL;
	}
}

Object^ Desktop::SendMarshaledMessage(IntPtr hWnd, UINT Msg, Object^ parameter, bool hookRequired) {
	if (hWnd == IntPtr::Zero) {
		return nullptr;
	}
	MemoryStore *store = MemoryStore::CreateStore(hWnd);
	if (store == NULL) {
		return nullptr;
	}
	Object^ retval = nullptr;
	if (hookRequired) {
		Desktop::EnableHook(hWnd);
	}

	try {
		retval = store->SendDataMessage(Msg, parameter);
	}
	finally {
		if (hookRequired) {
			Desktop::EnableHook(hWnd);	//enable it during deletion for memory release messages.
		}
		delete store;
		if (hookRequired) {
			Desktop::DisableHook();
		}
	}
	return retval;
}

BOOL CALLBACK EnumCallback(HWND handle, LPARAM arg) {
	Desktop::topLevelWindows->Add(Desktop::GetProxy((System::IntPtr)handle));
	return TRUE;
}



array<ControlProxy^>^ Desktop::GetTopLevelWindows() {
	_ASSERTE(topLevelWindows->Count==0);

	EnumWindows((WNDENUMPROC)EnumCallback, (LPARAM)0);

	array<ControlProxy^>^ winarr = gcnew array<ControlProxy^>(topLevelWindows->Count);
	topLevelWindows->CopyTo(winarr);
	topLevelWindows->Clear();
	return winarr;
}

bool Desktop::IsManagedProcess(DWORD processID) {
	if (managedProcesses->Contains(processID))
		return true;

	if (processID == 0 || unmanagedProcesses->Contains(processID))
		return false;

	Process^ proc = Process::GetProcessById(processID);
	if (proc == nullptr) {
		return false;
	}

	bool ismanaged = false;
	for(int i = 0;i<proc->Modules->Count;i++) {
		if(proc->Modules[i]->ModuleName == _T("mscorlib.dll") ||
			proc->Modules[i]->ModuleName == _T("mscorlib.ni.dll")) {
				//make sure its version 2.0
				System::Reflection::AssemblyName^ name = System::Reflection::AssemblyName::GetAssemblyName(
					proc->Modules[i]->FileName);
				if (name != nullptr && name->Version->Major == 2) {
					ismanaged = true;
				}
				break;
		}
	}

	if (ismanaged) {
		managedProcesses->Add(processID);
	}
	else {
		unmanagedProcesses->Add(processID);
	}
	return ismanaged;
}

ControlProxy^ Desktop::GetProxy(System::IntPtr windowHandle) {
	ControlProxy^ proxy = nullptr;
	if (proxyCache->ContainsKey(windowHandle)) {
		proxy = proxyCache[windowHandle];
	}
	if (proxy == nullptr) {
		DWORD procid = 0;
		GetWindowThreadProcessId((HWND)windowHandle.ToPointer(), &procid);
		if (IsManagedProcess(procid)) {
			List<Object^>^ params = gcnew List<Object^>();
			params->Add(Desktop::eventWindow->Handle);
			proxy = (ControlProxy^) SendMarshaledMessage(windowHandle, WM_GETPROXY, params);
		}
	}
	//not .Net windows.
	if (proxy == nullptr) {
		proxy = gcnew ControlProxy(windowHandle);
	}
	//add proxy to cache, we will use them for events.
	if (proxy != nullptr && !proxyCache->ContainsKey(windowHandle)) {
		proxyCache->Add(windowHandle, proxy);
	}

	return proxy;
}

void EventRegister::OnEventFired(Object^ sender, EventArgs^ args) {
	List<Object^>^ params = gcnew List<Object^>();
	params->Add((Object^)sourceWindow->Handle);
	params->Add((Object^)eventCode);
	if (args == EventArgs::Empty || args->GetType()->IsSerializable) {
		params->Add(args);
	}
	else {
		if (MouseEventArgs::typeid->IsAssignableFrom(args->GetType())) {
			params->Add( gcnew SerializableMouseEventArgs((MouseEventArgs^)args));
		}
		else {
			params->Add(gcnew NonSerializableEventArgs(args));
		}
	}
	Desktop::SendMarshaledMessage(targetEventReceiver, WM_EVENTFIRED, params, false);
}

//-----------------------------------------------------------------------------
//Spied Process functions follow
//-----------------------------------------------------------------------------
__declspec( dllexport ) 
int __stdcall MessageHookProc(int nCode, WPARAM wparam, LPARAM lparam) {
	try {
		if (nCode == HC_ACTION) {
			Shrinerain::AutoTester::WindowsHook::Desktop::OnMessage(nCode, wparam, lparam);
		}
	}
	catch(...) {}

	return CallNextHookEx(_messageHookHandle,
		nCode, wparam, lparam);
}

Object^ Desktop::GetEventHandler(Type^ eventHandlerType, Object^ instance) {

	Object^ o = nullptr;
	if (instance == nullptr || eventHandlerType == nullptr) {
		return nullptr;
	}
	array<Object^>^ dmparams = gcnew array<Object^>(1);
	dmparams[0] = instance;

	if (eventMethodCache->ContainsKey(eventHandlerType)) {
		return eventMethodCache[eventHandlerType]->Invoke(nullptr, dmparams);
	}
	try {
		String^ fnName = gcnew String(L"GenerateEH");
		fnName = fnName + eventTypeCount.ToString();

		//get the event callback methodinfo.
		if (eventCallback == nullptr) {
			eventCallback = EventRegister::typeid->GetMethod(L"OnEventFired", BindingFlags::Instance | BindingFlags::Public);
		}

		array<ConstructorInfo^>^ ctors = eventHandlerType->GetConstructors();
		if (ctors->Length < 1) {
			return nullptr;
		}
		ConstructorInfo^ ehCtor = ctors[0];

		array<Type^>^ params = gcnew array<Type^>(1);
		params[0] = Object::typeid;

		DynamicMethod^ dm = gcnew DynamicMethod(fnName, EventHandler::typeid,
			params, Desktop::typeid);

		ILGenerator^ methodIL = dm->GetILGenerator();
		methodIL->Emit(OpCodes::Ldarg_0);
		methodIL->Emit(OpCodes::Ldftn, eventCallback);
		methodIL->Emit(OpCodes::Newobj, ehCtor);
		methodIL->Emit(OpCodes::Ret);

		o = dm->Invoke(nullptr, dmparams);
		eventTypeCount++;
		eventMethodCache->Add(eventHandlerType, dm);
	}
	catch(...) {
		o = nullptr;
	}
	return o;
}

void Desktop::SubscribeEvent(Control^ target, IntPtr eventWindow, String^ eventName, int eventCode) {
	EventRegister^ eventreg = gcnew EventRegister();
	eventreg->eventCode = eventCode;
	eventreg->sourceWindow = target;
	eventreg->targetEventReceiver = eventWindow;
	eventreg->eventInfo = target->GetType()->GetEvent(eventName);

	//if everything is valid, add it to our list and subscribe
	if (eventreg->sourceWindow != nullptr && 
		eventreg->targetEventReceiver != IntPtr::Zero &&
		eventreg->eventInfo != nullptr ) {
			Dictionary<int, EventRegister^>^ windowEventList;

			if (!eventCallbacks->ContainsKey(target->Handle)) {
				windowEventList = gcnew Dictionary<int, EventRegister^>();
				eventCallbacks->Add(target->Handle, windowEventList);
			}
			else {
				windowEventList = eventCallbacks[target->Handle];
			}
			if (windowEventList->ContainsKey(eventCode)) {
				UnsubscribeEvent(target, eventCode);
			}
			MethodInfo^ addmethod = eventreg->eventInfo->GetType()->GetMethod(L"AddEventHandler");
			_ASSERTE(addmethod != nullptr);
			array<Object^>^ addparams = gcnew array<Object^>(2);
			addparams[0] = eventreg->sourceWindow;
			addparams[1] = GetEventHandler(eventreg->eventInfo->EventHandlerType, eventreg);
			addmethod->Invoke(eventreg->eventInfo, addparams);
			windowEventList->Add(eventCode, eventreg);
	}
}

void Desktop::UnsubscribeEvent(Control^ target, int eventCode) {
	if (target!= nullptr && target->Handle != IntPtr::Zero) {
		if (eventCallbacks->ContainsKey(target->Handle)) {
			Dictionary<int, EventRegister^>^ windowEventList = eventCallbacks[target->Handle];
			_ASSERTE(windowEventList!= nullptr);
			if (windowEventList != nullptr && windowEventList->ContainsKey(eventCode)) {
				EventRegister^ eventreg = windowEventList[eventCode];
				if (eventreg != nullptr) {
					windowEventList->Remove(eventCode);
					if (eventreg->eventInfo != nullptr) {
						MethodInfo^ removemethod = eventreg->eventInfo->GetType()->GetMethod(L"RemoveEventHandler");
						_ASSERTE(removemethod != nullptr);
						array<Object^>^ removeparams = gcnew array<Object^>(2);
						removeparams[0] = eventreg->sourceWindow;
						removeparams[1] = GetEventHandler(eventreg->eventInfo->EventHandlerType, eventreg);
						removemethod->Invoke(eventreg->eventInfo, removeparams);
					}
				}
			}
		}
	}
}

void Desktop::OnMessage(int nCode, WPARAM wparam, LPARAM lparam) {

	MGD_CWPSTRUCT* msg = (MGD_CWPSTRUCT*)lparam;

	if (msg != NULL) {
		if (msg->message == WM_ISMANAGED) {
			//query whether this window is managed.
			Control^ w = System::Windows::Forms::Control::FromHandle((System::IntPtr)msg->hwnd);
			MemoryStore* store = MemoryStore::OpenStore(msg);
			if (store != NULL) {
				if (w != nullptr) {
					store->StoreReturnValue((Object^)true);
				}
				else {
					store->StoreReturnValue((Object^)false);
				}
			}
		}
		else if (msg->message == WM_GETPROXY) {
			MemoryStore* store = MemoryStore::OpenStore(msg);
			if (store != NULL) {
				List<Object^>^ params= (List<Object^>^)store->GetParameters();
				if (params->Count == 1) {
					ControlProxy^ proxy = nullptr;
					if (proxyCache->ContainsKey((System::IntPtr)msg->hwnd)) {
						proxy = proxyCache[(System::IntPtr)msg->hwnd];
					}
					if (proxy == nullptr) {
						Control^ w = System::Windows::Forms::Control::FromHandle((System::IntPtr)msg->hwnd);
						if (w != nullptr) {
							proxy = gcnew ControlProxy(w);
						}
					}

					proxy->SetEventWindow((IntPtr)params[0]);

					store->StoreReturnValue(proxy);
				}
			}
		}
		else if (msg->message == WM_RELEASEMEM) {
			MemoryStore* store = MemoryStore::OpenStore((int)msg->wParam, (int)msg->lParam, false);
			if (store) {
				store->Release();
			}
		}
		else if (msg->message == WM_GETMGDPROPERTY) {
			Control^ w = System::Windows::Forms::Control::FromHandle((System::IntPtr)msg->hwnd);
			MemoryStore* store = MemoryStore::OpenStore(msg);
			if (w != nullptr && store != NULL) {
				String^ propname = (String^)store->GetParameters();
				PropertyDescriptor^ pd = TypeDescriptor::GetProperties(w)[propname];
				if (pd != nullptr) {
					Object^ val = pd->GetValue(w);
					store->StoreReturnValue(val);
				}
			}
		}
		else if (msg->message == WM_RESETMGDPROPERTY) {
			Control^ w = System::Windows::Forms::Control::FromHandle((System::IntPtr)msg->hwnd);
			MemoryStore* store = MemoryStore::OpenStore(msg);
			if (w != nullptr && store != NULL) {
				String^ propname = (String^)store->GetParameters();
				PropertyDescriptor^ pd = TypeDescriptor::GetProperties(w)[propname];
				if (pd != nullptr) {
					pd->ResetValue(w);
				}
			}
		}
		else if (msg->message == WM_SETMGDPROPERTY) {
			Control^ w = System::Windows::Forms::Control::FromHandle((System::IntPtr)msg->hwnd);
			MemoryStore* store = MemoryStore::OpenStore(msg);
			if (w != nullptr && store != NULL) {
				List<Object^>^ params= (List<Object^>^)store->GetParameters();
				if (params != nullptr && params->Count == 2) {
					PropertyDescriptor^ pd = TypeDescriptor::GetProperties(w)[(String^)params[0]];
					if (pd != nullptr) {
						pd->SetValue(w, params[1]);
					}
				}
			}
		}
		else if (msg->message == WM_SUBSCRIBEEVENT) {
			Control^ w = System::Windows::Forms::Control::FromHandle((System::IntPtr)msg->hwnd);
			MemoryStore* store = MemoryStore::OpenStore(msg);
			if (w != nullptr && store != NULL) {
				List<Object^>^ params= (List<Object^>^)store->GetParameters();
				if (params != nullptr && params->Count == 3) {
					SubscribeEvent(w, (IntPtr)params[0], (String^)params[1], (int)params[2]);
				}
			}
		}
		else if (msg->message == WM_UNSUBSCRIBEEVENT) {
			Control^ w = System::Windows::Forms::Control::FromHandle((System::IntPtr)msg->hwnd);
			MemoryStore* store = MemoryStore::OpenStore(msg);
			if (w != nullptr && store != NULL) {
				List<Object^>^ params= (List<Object^>^)store->GetParameters();
				if (params != nullptr && params->Count == 1) {
					UnsubscribeEvent(w, (int)params[0]);
				}
			}
		}
	}
}

