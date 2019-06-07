using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static Csharp_InlineHook.Methods;

namespace Csharp_InlineHook
{
    class Inline_Hook
    {
        /// <summary>
        /// InlineHook
        /// </summary>
        /// <param name="HookAddress">Hook地址</param>
        /// <param name="Hooklen">Hook长度</param>
        /// <param name="HookBytes0">Hook数据</param>
        /// <param name="Callback">回调地址</param>
        /// <param name="CallbackOffset">回调偏移</param>
        /// <param name="IsFront">是否前置</param>
        /// <param name="CallAddress">CALL地址</param>
        public static IntPtr InlineHook(int HookAddress, int Hooklen,
            byte[] HookBytes0,int Callback,int CallbackOffset,
            bool IsFront,int CallAddress,string name, Action<Methods.Register> func)
        {
            WeChetHook.DllcallBack dllcallBack = new WeChetHook.DllcallBack((de1, de2, ECX1, EAX1, EDX1, EBX1, ESP1, EBP1, ESI1, EDI1) => {
                func(new Register
                {
                    EAX = EAX1,
                    EBP = EBP1,
                    EBX = EBX1,
                    ECX = ECX1,
                    EDI = EDI1,
                    EDX = EDX1,
                    ESI = ESI1,
                    ESP = ESP1
                });
            });
            int CallHandle = ComputeHash(name);
            Methods.callBacks.Add(CallHandle, dllcallBack);

            List<byte> byteSource1 = new List<byte>();
            byteSource1.AddRange(new byte[] { 199, 134, 240, 2, 0, 0 });
            byteSource1.AddRange(BitConverter.GetBytes(CallHandle));
            byteSource1.AddRange(HookBytes0);

            byte[] hookbytes = byteSource1.ToArray();


            List<byte> byteSource = new List<byte>();
            IntPtr ptr = NativeAPI.VirtualAlloc(0, 128, 4096, 64);
            if (IsFront)
            {
                NativeAPI.WriteProcessMemory(-1, ptr, Add(new byte[] { 232 }, Inline_GetBuf(ptr, CallAddress)), 5, 0);
                NativeAPI.WriteProcessMemory(-1, ptr + 5, hookbytes, hookbytes.Length, 0);
                NativeAPI.WriteProcessMemory(-1, ptr + 5 + CallbackOffset, Inline_GetBuf(ptr + 5 + CallbackOffset - 1, Callback), 4, 0);
                NativeAPI.WriteProcessMemory(-1, ptr + 5 + hookbytes.Length, Add(new byte[] { 233 }, Inline_GetBuf(ptr + 5 + HookBytes0.Length, HookAddress + Hooklen)), 5, 0);
            }
            else {
                NativeAPI.WriteProcessMemory(-1, ptr, hookbytes, hookbytes.Length, 0);
                NativeAPI.WriteProcessMemory(-1, ptr + CallbackOffset, Inline_GetBuf(ptr + CallbackOffset - 1, Callback), 4, 0);
                NativeAPI.WriteProcessMemory(-1, ptr + hookbytes.Length,Add(new byte[] { 232 },Inline_GetBuf(ptr + hookbytes.Length, CallAddress)), Hooklen, 0);
                NativeAPI.WriteProcessMemory(-1, ptr + Hooklen + hookbytes.Length, Add(new byte[] { 233 }, Inline_GetBuf(ptr + Hooklen + HookBytes0.Length, HookAddress + Hooklen)), 5, 0);
            }
            NativeAPI.WriteProcessMemory(-1, new IntPtr(HookAddress), Add(new byte[] { 233 }, Inline_GetBuf(HookAddress, ptr.ToInt32())), 5, 0);
            for (int i = 0; i < Hooklen - 5; i++)
            {
                byteSource.Add(144);
            }
            byte[] ByteFill = byteSource.ToArray();
            NativeAPI.WriteProcessMemory(-1, new IntPtr(HookAddress + 5), ByteFill, ByteFill.Length, 0);
            return ptr;
        }
        /// <summary>
        /// InlineHook
        /// </summary>
        /// <param name="HookAddress">Hook地址</param>
        /// <param name="Hooklen">Hook长度</param>
        /// <param name="HookBytes0">Hook数据</param>
        /// <param name="Callback">回调地址</param>
        /// <param name="CallbackOffset">回调偏移</param>
        public static IntPtr InlineHook(int HookAddress, int Hooklen,
            byte[] HookBytes0, int Callback, int CallbackOffset,string name, Action<Methods.Register> func)
        {

            WeChetHook.DllcallBack dllcallBack = new WeChetHook.DllcallBack((de1, de2, ECX1, EAX1, EDX1, EBX1, ESP1, EBP1, ESI1, EDI1) => {
                //int ECX, int EAX, int EDX, int EBX, int ESP, int EBP, int ESI, int EDI
                func(new Register
                {
                    EAX = EAX1,
                    EBP = EBP1,
                    EBX = EBX1,
                    ECX = ECX1,
                    EDI = EDI1,
                    EDX = EDX1,
                    ESI = ESI1,
                    ESP = ESP1
                });
            });
            int CallHandle = ComputeHash(name);
            System.Windows.Forms.MessageBox.Show("CallHandle:" + CallHandle.ToString());
            Methods.callBacks.Add(CallHandle, dllcallBack);

            List<byte> byteSource1 = new List<byte>();
            byteSource1.AddRange(new byte[] { 199, 134, 240, 2, 0, 0 });
            byteSource1.AddRange(BitConverter.GetBytes(CallHandle));//把标识指针绑定到寄存器我觉得不靠谱但是目前没啥问题
            byteSource1.AddRange(HookBytes0);

            byte[] hookbytes = byteSource1.ToArray();


            List<byte> byteSource = new List<byte>();
            IntPtr ptr = NativeAPI.VirtualAlloc(0, 128, 4096, 64);
            NativeAPI.WriteProcessMemory(-1, ptr, hookbytes, hookbytes.Length, 0);
            NativeAPI.WriteProcessMemory(-1, ptr + CallbackOffset, Inline_GetBuf(ptr + CallbackOffset - 1, Callback), 4, 0);
            NativeAPI.WriteProcessMemory(-1, ptr + hookbytes.Length, Add(new byte[] { 233 }, Inline_GetBuf(ptr + hookbytes.Length, HookAddress+ Hooklen)), 5, 0);

            NativeAPI.WriteProcessMemory(-1, new IntPtr(HookAddress), Add(new byte[] { 233 }, Inline_GetBuf(HookAddress, ptr.ToInt32())), 5, 0);
            for (int i = 0; i < Hooklen - 5; i++)
            {
                byteSource.Add(144);
            }
            byte[] ByteFill = byteSource.ToArray();
            NativeAPI.WriteProcessMemory(-1, new IntPtr(HookAddress + 5), ByteFill, ByteFill.Length, 0);
            return ptr;
        }
        /// <summary>
        /// InlineHook
        /// </summary>
        /// <param name="HookAddress">Hook地址</param>
        /// <param name="Hooklen">Hook长度</param>
        /// <param name="Callback">回调地址</param>
        /// <param name="CallbackOffset">回调偏移</param>
        public static IntPtr InlineHook(int HookAddress, int Hooklen,int Callback)
        {
            List<byte> byteSource = new List<byte>();
            NativeAPI.WriteProcessMemory(-1, new IntPtr(HookAddress), Add(new byte[] { 233 }, Inline_GetBuf(HookAddress, Callback)), 5, 0);
            for (int i = 0; i < Hooklen - 5; i++)
            {
                byteSource.Add(144);
            }
            byte[] ByteFill = byteSource.ToArray();
            NativeAPI.WriteProcessMemory(-1, new IntPtr(HookAddress + 5), ByteFill, ByteFill.Length, 0);
            return IntPtr.Zero;
        }
        /// <summary>
        /// 计算哈希值字符串
        /// </summary>
        public static int ComputeHash(string buffer)
        {
            return buffer.GetHashCode();
        }
        public static byte[] Inline_GetBuf(int Address,int jmp)
        {
            return BitConverter.GetBytes(jmp - Address - 5);
        }
        public static byte[] Inline_GetBuf(IntPtr Address, int jmp)
        {
            return BitConverter.GetBytes(jmp - Address.ToInt32() - 5);
        }
        public static byte[] Add(byte[] byteABytes,byte[] bytes)
        {
            byte[] bytes_ = new byte[byteABytes.Length + bytes.Length];
            byteABytes.CopyTo(bytes_, 0);
            bytes.CopyTo(bytes_, byteABytes.Length);
            return bytes_;
        }
        public int FindBytes(byte[] src, byte[] find)
        {
            int index = -1;
            int matchIndex = 0;
            // handle the complete source array
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == find[matchIndex])
                {
                    if (matchIndex == (find.Length - 1))
                    {
                        index = i - matchIndex;
                        break;
                    }
                    matchIndex++;
                }
                else if (src[i] == find[0])
                {
                    matchIndex = 1;
                }
                else
                {
                    matchIndex = 0;
                }

            }
            return index;
        }

        public byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            byte[] dst = null;
            int index = FindBytes(src, search);
            if (index >= 0)
            {
                dst = new byte[src.Length - search.Length + repl.Length];
                // before found array
                Buffer.BlockCopy(src, 0, dst, 0, index);
                // repl copy
                Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
                // rest of src array
                Buffer.BlockCopy(
                    src,
                    index + search.Length,
                    dst,
                    index + repl.Length,
                    src.Length - (index + search.Length));
            }
            return dst;
        }
    }
}
