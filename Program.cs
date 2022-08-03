using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeMonitorApplication
{
    static class Program
    {

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdshow);  //确保窗口没有被最小化或最大化
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);  //将窗口放置最前端
        private const int WS_SHOWNORMAL = 1;  //指定窗口如何显示，不同值不同方式

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]//Single Thread Apartment Thread.(单一线程单元线程)。加不加，取决于你有没有用第三方的组件，它的线程模型是否和你的程序兼容。
        static void Main()
        {
            Process instance = RunningInstance();//Process类提供对本地和远程进程的访问权限并使你能够启动和停止本地系统进程
            if (instance == null)//当前没有开启程序
            {
                Application.EnableVisualStyles();//启用应用程序的可视样式。Application类具有用于启动和停止应用程序和线程以及处理 Windows 消息的方法。
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                HandleRunningInstance(instance);// 处理正在运行的实例，显示已运行的程序
            }
        }

        /// <summary>
        /// 获取该程序进程
        /// </summary>
        /// <returns>已经存在进程时，返回进程；不存在时，返回null</returns>
        private static Process RunningInstance()
        {
            //1、获得当前线程
            Process current = Process.GetCurrentProcess();  //这个WinAPI函数来获得当前进程的一个伪句柄。
            //2、获得电脑中与当前线程名称相同的线程
            Process[] processes = Process.GetProcessesByName(current.ProcessName);//GetProcessesByName(String)创建新的 Process 组件的数组，并将它们与本地计算机上共享指定的进程名称的所有进程资源关联。
            //3、遍历名称相同的线程，如果存在名称相同，id不同，则证明该应用程序已经存在，返回已经存在的进程。
            foreach (Process process in processes)
            {
                //线程名称相同时，如果线程Id不同，则证明当前程序已经打开过了
                if (process.Id != current.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)//获取包含当前执行的代码的程序集。保证要打开的进程同已经存在的进程来自同一文件路径
                    {
                        return process;//返回已经存在的进程
                    }
                }
            }
            //4、如果没有名称相同的线程，则返回null
            return null;
        }

        /// <summary>
        /// 激活程序进程
        /// </summary>
        /// <param name="instance"></param>
        private static void HandleRunningInstance(Process instance)
        {
            //确保窗口没有被最小化或最大化
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL); //调用api函数，正常显示窗口

            //设置真实例程为foreground window
            SetForegroundWindow(instance.MainWindowHandle); //将窗口放置最前端
        }

    }
}
