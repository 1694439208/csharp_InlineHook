# csharp_InlineHook
net 实现InlineHook


 
## 目录
* [背景介绍](#背景介绍)
* [项目介绍](#项目介绍)
* [使用说明](#使用说明)
       * [接口介绍](#接口介绍)
       * [使用样例](#使用样例)
* [其他](#其他)
 
<a name="背景介绍"></a>
## 背景介绍
 
*csharp_InlineHook*，是一个可以将托管dll注入到非托管进程hook。
 
<a name="项目介绍"></a>
## 项目介绍
 
此项目设计初衷是提供一个可方便扩展的hook库。<br>
 
写此库之前查阅大量的资料并只有两个实现
一个是*esayhook* 缺点是不支持任意点hook，只能hook系统api
一个是网友实现的只支持托管进程hook 

本项目可以支持InlineHook 任意点hook 函数或者流程

<a name="使用说明"></a>
## 使用说明
 net可以直接编译，
 
 dllmain.cpp自己新建项目编译   
 
<a name="使用样例"></a>
#### 使用样例
```      
Inline_Hook.InlineHook(3212659,5, byteSource.ToArray(), getInt(Method),11+10,"接收消息",(obj) =>{
                StringBuilder sb = new StringBuilder();
                sb.Append("接收消息:");
                int a = 0x68;
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
```

![Shurnim icon](https://github.com/1694439208/csharp_InlineHook/blob/master/549875165.png)
<a name="其他"></a>
## 其他
 
时间仓促，功能简陋，望您包涵。特别希望看到该项目对您哪怕一点点的帮助。任意的意见和建议，欢迎随意与我沟通,联系方式：
 
* Email: <1694439208@qq.com>
* QQ:1694439208
* Blog:[Blog](https://my.oschina.net/KFS)
 
项目的Bug和改进点，可在OSChina上以issue的方式直接提交给我。
