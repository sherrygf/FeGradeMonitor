using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GradeMonitorApplication.FunctionClass
{
    class Win32API
    {

        //[StructLayout(LayoutKind.Sequential)]
        /// <summary>
        /// 复制数据结构：使用COPYDATASTRUCT来传递字符串
        /// </summary>
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
    }
}
