using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using static Csharp_InlineHook.Form1;

namespace Csharp_InlineHook
{
    public class WeChetHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DllcallBack(int Default1, int Default2,
            int EAX, int EBX, int ECX, int EDX, int ESI, int EDI, int EBP, int ESP);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetStr(int address);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetInt(int address);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetModule(string address);

        public AutoResetEvent mUnique = new AutoResetEvent(false);
        public int Start(GetStr r, GetInt w, GetModule M)
        {
            Form1 forma = new Form1(r, w ,M);
            forma.ShowDialog();
            return 1;
        }
        public static Type getStr()
        {
            return typeof(GetStr);
        }
        public static Type getInt()
        {
            return typeof(GetInt);
        }
        public static Type getModule()
        {
            return typeof(GetModule);
        }
        public static DllcallBack getWeChetHook()
        {
            return new DllcallBack(Callback);
        }
        
        /// <summary>
        /// 前两个是不属于参数
        /// </summary>
        /// <param name="Default1">默认</param>
        /// <param name="Default2">默认</param>
        /// <param name="EAX"></param>
        /// <param name="EBX"></param>
        /// <param name="ECX"></param>
        /// <param name="EDX"></param>
        /// <param name="ESI"></param>
        /// <param name="EDI"></param>
        /// <param name="EBP"></param>
        /// <param name="ESP"></param>
        public static void Callback(int Default1, int Default2,
            int ECX, int EAX, int EDX, int EBX, int ESP, int EBP, int ESI, int EDI)
        {
            int ptr = NativeAPI.ReadMemoryValue(Default2);
            if (Methods.callBacks.ContainsKey(ptr))
            {
                Methods.callBacks[ptr](Default1, ptr, ECX, EAX, EDX, EBX, ESP, EBP, ESI, EDI);
            }
            //System.Windows.Forms.MessageBox.Show("微信hook消息拦截成功EAX:" + NativeAPI.ReadMemoryValue(Default2).ToString(), "hook成功");
            //System.Windows.Forms.MessageBox.Show("微信hook消息拦截成功ESP:" + Default2.ToString(), "hook成功");
        }
    }
}
