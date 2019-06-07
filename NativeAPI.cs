using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Csharp_InlineHook
{
    public class NativeAPI
    {
        #region API
        //打开一个已存在的进程对象，并返回进程的句柄
        [DllImportAttribute("kernel32.dll", EntryPoint = "VirtualAlloc")]
        public static extern IntPtr VirtualAlloc(int lpAddress, int dwSize, int flAllocationType, int flProtect);

        [DllImportAttribute("kernel32.dll", EntryPoint = "lstrlenW")]
        public static extern int lstrlenW(int lpAddress);
        //从指定内存中读取字节集数据
        [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern bool ReadProcessMemory(int hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, int lpNumberOfBytesRead);

        //从指定内存中写入字节集数据
        [DllImportAttribute("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        public static extern bool WriteProcessMemory(int hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

        //打开一个已存在的进程对象，并返回进程的句柄
        [DllImportAttribute("kernel32.dll", EntryPoint = "OpenProcess")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        //关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。
        [DllImport("kernel32.dll")]
        private static extern void CloseHandle(IntPtr hObject);

        #endregion

        #region 使用方法

        //根据进程名获取PID
        public static int GetPidByProcessName(string processName)
        {
            Process[] arrayProcess = Process.GetProcessesByName(processName);
            foreach (Process p in arrayProcess)
            {
                return p.Id;
            }
            return 0;
        }

        //读取内存中的值
        public static int ReadMemoryValue(int baseAddress)
        {
            try
            {
                byte[] buffer = new byte[4];
                //获取缓冲区地址
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                //IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(""));
                //将制定内存中的值读入缓冲区
                ReadProcessMemory(-1, (IntPtr)baseAddress, byteAddress, 4, IntPtr.Zero.ToInt32());
                //关闭操作
                //CloseHandle(hProcess);
                //从非托管内存中读取一个 32 位带符号整数。
                return Marshal.ReadInt32(byteAddress);
            }
            catch
            {
                return 0;
            }
        }
        //读取内存中的值
        public static string ReadMemoryStrValue(int baseAddress,int len)
        {
            try
            {
                byte[] buffer = new byte[len];
                //获取缓冲区地址
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                //IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(""));
                //将制定内存中的值读入缓冲区
                ReadProcessMemory(-1, (IntPtr)baseAddress, byteAddress, len, IntPtr.Zero.ToInt32());
                //关闭操作
                //CloseHandle(hProcess);
                //从非托管内存中读取一个 32 位带符号整数。
                return Encoding.Unicode.GetString(buffer);
            }
            catch
            {
                return "";
            }
        }

        //将值写入指定内存地址中

        public static void WriteMemoryValue(int baseAddress, string processName, int value)
        {
            try
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(processName));
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess.ToInt32(), (IntPtr)baseAddress, BitConverter.GetBytes(value), 4, IntPtr.Zero.ToInt32());
                //关闭操作
                CloseHandle(hProcess);
            }
            catch { }
        }

        #endregion
        public static int GetMethodPTR(Type type,string name) {
            return type.GetMethod(name).MethodHandle.GetFunctionPointer().ToInt32();
        }
        public static string GetPtrToStr_u(int ptr)
        {
            return Marshal.PtrToStringUni(new IntPtr(ptr));
        }
        public static string GetPtrToStr_u(int ptr, int len)
        {
            return Marshal.PtrToStringUni(new IntPtr(ptr), len);
        }
    }
}
