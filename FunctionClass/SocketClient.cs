using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeMonitorApplication.FunctionClass
{
    class SocketClient
    {
        #region 变量
        /// <summary>
        /// 字节数
        /// </summary>
        const int size = 1024;

        /// <summary>
        /// 字段：_socket。Socket对象
        /// </summary>
        public Socket _socket;
        /// <summary>
        /// 属性：Socket，只读。Socket对象
        /// </summary>
        public Socket Socket
        {
            get
            {
                if (_socket == null)
                {
                    //AddressFamily：指定Socket用来解析地址的寻址方案。例如：InterNetWork指示当Socket使用一个IP版本4地址连接.
                    //SocketType：定义要打开的Socket的类型.SocketType.Stream：支持可靠、双向、基于连接的字节流，而不重复数据。此类型的 Socket 与单个对方主机进行通信，并且在通信开始之前需要远程主机连接。Stream 使用传输控制协议 (Tcp) ProtocolType 和 InterNetworkAddressFamily。
                    //Socket类使用ProtocolType枚举向Windows  Sockets API通知所请求的协议.ProtocolType.Tcp：使用传输控制协议。
                    _socket = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);
                }
                return _socket;
            }
        }

        /// <summary>
        /// 属性：IsConnected，只读。Socket已连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (this._socket != null)
                {
                    return this._socket.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 字段：_receivedBytes。数据接收流对象
        /// </summary>
        private MemoryStream _receivedBytes = new MemoryStream();
        /// <summary>
        /// 属性：ReceivedBytes，只读。数据接收流对象
        /// </summary>
        public byte[] ReceivedBytes
        {
            get
            {
                //当客户端获取该属性值时，将接收到的数据返回，并且将_receivedBytes值清空，准备接收下一条数据
                byte[] bs = _receivedBytes.ToArray();  //将流内容写入字节流数组
                _receivedBytes.SetLength(0);  //将当前流的长度设置为指定值
                return bs;
            }
        }

        /// <summary>
        /// 发布者
        /// 事件：DataReceived。定义了一个名为 EventHandler（发送者） 的委托和一个名为 DataReceived 的事件，该事件在生成的时候会调用委托。
        /// 事件模型基于事件委托，该委托将事件与其处理程序连接。
        /// EventHandler 非范型委托，表示将用于处理不具有事件数据的事件的方法。
        /// </summary>
        public event EventHandler DataReceivedEvent;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ClosedEvent;
        #endregion

        #region socket连接connect
        /// <summary>
        /// 函数：Socket连接
        /// </summary>
        /// <param name="ipAddress">服务器IP</param>
        /// <param name="port">服务器端口</param>
        public void Connect(IPAddress ipAddress, UInt16 port)
        {
            //如果已经连接，操作无效。
            if (this.IsConnected)
            {
                throw new InvalidOperationException("isconnected");//操作无效异常。当方法调用对对象的当前状态无效时引发的异常。
            }

            //定义Socket实例_socket
            _socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

            //IPAddress：包含了一个IP地址
            //IPEndPoint：包含了一对IP地址和端口号
            //Bind():绑定一个本地的IP和端口号（IPEndPoint）            
            EndPoint ep = new IPEndPoint(ipAddress, port);

            Socket.Connect(ep);//初始化与另一个Socket的连接
        }
        #endregion

        #region socket发送send
        /// <summary>
        /// 发送socket
        /// </summary>
        /// <param name="buffer">二进制命令</param>
        public void Send(byte[] buffer)
        {
            this._socket.Send(buffer);
        }
        #endregion

        #region socket接收receive
        /// <summary>
        /// socket接收函数，如果有数据传入，则触发事件DataReceivedEvent
        /// 采用异步操作，
        /// </summary>
        public void BeginReceive()
        {
            try
            {
                AsyncCallback cb = new AsyncCallback(ReceiveCallback);//异步操作完成时调用的方法
                byte[] receiveBuffer = new byte[size];
                IAsyncResult ia = _socket.BeginReceive( //sicket中BeginReceive是异步非阻塞，Receive是同步阻塞
                    receiveBuffer,
                    0,
                    size,
                    SocketFlags.None,
                    cb,
                    receiveBuffer);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("停止接收!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        /// <summary>
        /// 异步回调函数
        /// </summary>
        /// <param name="ia"></param>
        void ReceiveCallback(IAsyncResult ia)
        {
            byte[] receiveBuffer = (byte[])ia.AsyncState;
            int n = 0;
            try
            {
                n = this._socket.EndReceive(ia); //结束接收数据
            }
            catch (ObjectDisposedException objectDisposedEx)
            {
                Function.WriteErrorLog("object disposed exception: " + objectDisposedEx.Message);
                return;
            }
            catch (SocketException socketEx)
            {
                Function.WriteErrorLog("socket exception: " + socketEx.Message);
                this.CloseHelper();
                return;
            }

            if (n > 0)
            {
                _receivedBytes.Write(receiveBuffer, 0, n);
                if (this.DataReceivedEvent != null)
                {
                    DataReceivedEvent?.Invoke(this, EventArgs.Empty);//触发事件：下面将调用委托的目标方法。this是广播者
                }
                BeginReceive();
            }
            else
            {
                CloseHelper();
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void CloseHelper()
        {
            //if( !_isClosed )
            //if (_socket != null)
            //if( _socket.Connected )
            //{
            Console.WriteLine("CloseHelper");

            _socket.Disconnect(false);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            if (this.ClosedEvent != null)
            {
                this.ClosedEvent(this, EventArgs.Empty);
            }
            //_socket = null;
            //_isClosed = true;
            //}
        }
    }
}
