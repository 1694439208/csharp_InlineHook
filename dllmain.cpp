// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "stdafx.h"
#include <gcroot.h>
#include <stdio.h>
#include "commctrl.h"
#include <TlHelp32.h>
#include <stdlib.h>

#define _ASSERT(expr) ((void)0)
#using "C:\Users\16944\source\repos\测试注入Net Hook\测试注入Net Hook\bin\Debug\测试注入Net Hook.dll"
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace Csharp_InlineHook;
using namespace System::Windows::Forms;
#define String2LPSTR(str) (char*)(void*)Marshal::StringToHGlobalAnsi(str)
#define String2WLPSTR(str) (wchar_t*)(void*)Marshal::StringToHGlobalAnsi(str)
#define LPSTR2String(lpstr) Marshal::PtrToStringAnsi((IntPtr)(void*)(lpstr))
#define Ptr2IntPtr(ptr) (IntPtr)(void*)(ptr);
#define IntPtr2Ptr(ptr, type) (type*)(void*)(ptr);
#define I(hobj) (int)(hobj)
//实现to_string函数
#define max 100

DWORD cEax = 0;
DWORD cEcx = 0;
DWORD cEdx = 0;
DWORD cEbx = 0;
DWORD cEsp = 0;
DWORD cEbp = 0;
DWORD cEsi = 0;
DWORD cEdi = 0;
DWORD retAdd = 0;

typedef void(*PF)(size_t, size_t, size_t, size_t, size_t, size_t, size_t, size_t, size_t, size_t);
PF Netaddress;
//获取模块基址
DWORD getModuleAddress(System::String^ name)
{
	//String2WLPSTR(name)
	//L"WeChatWin.dll"
	
	Netaddress = (PF)Marshal::GetFunctionPointerForDelegate(WeChetHook::getWeChetHook()).ToPointer();
	return (DWORD)LoadLibrary(L"WeChatWin.dll");

}
gcroot<WeChetHook^> WeChet;

wchar_t* GetStr(int address) {
	
	/*char str[100];
	_itoa_s(address, str, 10);
	MessageBoxA(NULL, str, "提示", 0);*/
	DWORD temWxidAdd = address;
	wchar_t wxid[0x100] = { 0 };
	if ((LPVOID *)temWxidAdd) {
		swprintf_s(wxid, L"%s", (LPVOID *)temWxidAdd);
		MessageBoxW(NULL, wxid, L"111提示", 0);
		return wxid;
	}
	return L"";
}

VOID __declspec(naked) GetThis()
{
	__asm {//保存寄存器
		mov cEax, eax
		mov cEcx, ecx
		mov cEdx, edx
		mov cEbx, ebx
		mov cEsp, esp
		mov cEbp, ebp
		mov cEsi, esi
		mov cEdi, edi
	}
	//int EAX, int EBX, int ECX, int EDX, int ESI, int EDI, int EBP, int ESP
	Netaddress(0, cEsi +0x2F0,cEax, cEcx, cEdx, cEbx, cEsp, cEbp, cEsi, cEdi);
	__asm {//恢复寄存器
		mov eax, cEax
		mov ecx, cEcx
		mov edx, cEdx
		mov ebx, cEbx
		mov esp, cEsp
		mov ebp, cEbp
		mov esi, cEsi
		mov edi, cEdi
		jmp dword ptr ds : [esi + 0x2EC]
	}
}

int GetInt(int address) {
	/*char str[100];
	_itoa_s(address, str, 10);
	MessageBoxA(NULL, str, "提示", 0);
	_asm {
		push 0x10
		push 0x12
		push 0x13
		call address
	}*/
	return (int)&GetThis;
}

void main()
{
	MessageBoxA(NULL, "注入成功", "提示", 0);
	WeChet = gcnew WeChetHook();
	WeChetHook::GetStr^ str = (WeChetHook::GetStr^)Marshal::GetDelegateForFunctionPointer(IntPtr(GetStr), WeChetHook::getStr());
	WeChetHook::GetInt^ Int = (WeChetHook::GetInt^)Marshal::GetDelegateForFunctionPointer(IntPtr(GetInt), WeChetHook::getInt());
	WeChetHook::GetModule^ Module = (WeChetHook::GetModule^)Marshal::GetDelegateForFunctionPointer(IntPtr(getModuleAddress), WeChetHook::getModule());
	((WeChetHook^)WeChet)->Start(str, Int, Module);
	//MessageBoxA(NULL, "pssspp", "sssss", 0);
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		//main();
		CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)main, hModule, 0, NULL);
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

