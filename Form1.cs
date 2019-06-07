using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Csharp_InlineHook.WeChetHook;

namespace Csharp_InlineHook
{
    public partial class Form1 : Form
    {
        public GetStr getStr
        {
            set;get;
        }
        public GetInt getInt
        {
            set; get;
        }
        public GetModule getModule
        {
            set; get;
        }
        public Form1(GetStr r, GetInt w,GetModule getM)
        {
            getStr = r;
            getInt = w;
            getModule = getM;
            InitializeComponent();
        }
        int ModuleAddress;
        private void button1_Click(object sender, EventArgs e)
        {
            ModuleAddress = getModule(textBox1.Text);
            label1.Text = ModuleAddress.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int a = int.Parse(textBox2.Text);
            int s = getStr(a);
            string data = NativeAPI.GetPtrToStr_u(s);
            label2.Text = data;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string data = NativeAPI.GetPtrToStr_u(int.Parse(textBox3.Text));
            MessageBox.Show(data);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int Method = NativeAPI.GetMethodPTR(typeof(WeChetHook), "test");
            getInt(Method);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] jmp_inst =
            {
                233,0,0,0,0,//JMP Address
            };
            int Method = NativeAPI.GetMethodPTR(typeof(WeChetHook), "Callback");
            textBox3.Text = (3212659 + int.Parse(label1.Text)).ToString();

            List<byte> byteSource = new List<byte>();
            byteSource.AddRange(new byte[] { 199, 134, 236, 2, 0, 0 });//mov dword [esi+0x000002EC],
            byteSource.AddRange(BitConverter.GetBytes(int.Parse(textBox3.Text) + 5));//0x00000000  把hook的后五个字节地址压进寄存器
            byteSource.AddRange(jmp_inst);//让他跳到跳板函数
            //这部分根据实际情况填写
            byteSource.Add(185);//补充替换的汇编指令
            byteSource.AddRange(BitConverter.GetBytes(int.Parse(label1.Text) + 19255272));//补充替换的汇编指令地址
            //开始hook
            Inline_Hook.InlineHook(int.Parse(textBox3.Text),5, byteSource.ToArray(), getInt(Method),11+10,"接收消息",(obj) =>{
                StringBuilder sb = new StringBuilder();
                sb.Append("接收消息:");
                int a = 0x68;
                //System.Windows.Forms.MessageBox.Show("esp:"+a.ToString());
                try
                {
                    if (obj.ESP == 0)
                        return;
                    int MsgPtr = NativeAPI.ReadMemoryValue(obj.ESP);
                    if (MsgPtr == 0)
                        return;
                    MsgPtr = NativeAPI.ReadMemoryValue(MsgPtr);
                    if (MsgPtr == 0)
                        return;
                    MsgPtr = NativeAPI.ReadMemoryValue(MsgPtr + 0x68);
                    if (MsgPtr == 0)
                        return;
                    int len = NativeAPI.lstrlenW(MsgPtr);
                    if (len == 0)
                        return;
                    sb.Append(NativeAPI.ReadMemoryStrValue(MsgPtr, len*2+2));
                    sb.Append("\r\n");
                    listBox1.Items.Add(sb.ToString());
                }
                catch (Exception es)
                {
                    File.AppendAllText("error.txt", es.Message);
                }
            });
        }
    }
}
