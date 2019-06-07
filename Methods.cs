using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharp_InlineHook
{
    static class Methods
    {
        //int Default1, int Default2,
        //int EAX, int EBX, int ECX, int EDX, int ESI, int EDI, int EBP, int ESP
        public struct Register {
            public int EAX { set; get; }
            public int EBX { set; get; }
            public int ECX { set; get; }
            public int EDX { set; get; }
            public int ESI { set; get; }
            public int EDI { set; get; }
            public int EBP { set; get; }
            public int ESP { set; get; }
        }
        static public Dictionary<int,WeChetHook.DllcallBack> callBacks =
            new Dictionary<int, WeChetHook.DllcallBack>();
    }
}
