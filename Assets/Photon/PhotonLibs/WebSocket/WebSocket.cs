#if UNITY_WEBGL || WEBSOCKET || WEBSOCKET_PROXYCONFIG

<<<<<<< Updated upstream
=======
#if UNITY_WEBGL && !UNITY_EDITOR
#define PHOTON_WEBSOCKET_JS
#else
#define PHOTON_WEBSOCKET_CS
#endif

>>>>>>> Stashed changes
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Provided originally by Unity to cover WebSocket support in WebGL and the Editor. Modified by Exit Games GmbH.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


<<<<<<< Updated upstream
namespace ExitGames.Client.Photon
{
    using System;
    using System.Text;
    using ExitGames.Client.Photon;

    #if UNITY_WEBGL && !UNITY_EDITOR
    using System.Runtime.InteropServices;
    #else
    using WebSocketSharp;
    using System.Collections.Generic;
=======
namespace Photon.Client
{
    using System;

    #if PHOTON_WEBSOCKET_JS
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using AOT;
    #else
    using WebSocketSharp;
>>>>>>> Stashed changes
    using System.Security.Authentication;
    #endif


<<<<<<< Updated upstream
    public class WebSocket
    {
        private Uri mUrl;
        private string mProxyAddress;

        /// <summary>Photon uses this to agree on a serialization protocol. Either: GpBinaryV16 or GpBinaryV18. Based on enum SerializationProtocol.</summary>
        private string protocols = "GpBinaryV16";

        public Action<DebugLevel, string> DebugReturn { get; set; }

        public WebSocket(Uri url, string proxyAddress, string protocols = null)
        {
            this.mUrl = url;
            this.mProxyAddress = proxyAddress;

            if (protocols != null)
=======
    // changed mProxyAddress to ProxyAddress
    // changed mUrl to Url

    public partial class WebSocket
    {
        /// <summary>Server address</summary>
        public Uri Url { get; private set; }

        /// <summary>Only supported by WebSocket-sharp dll.</summary>
        public string ProxyAddress { get; private set; }

        /// <summary>Photon uses this to agree on a serialization protocol. Either: GpBinaryV16 or GpBinaryV18. Based on enum SerializationProtocol.</summary>
        private readonly string protocols = "GpBinaryV16";


        /// <summary>True after the websocket callback OnConnect until close or (permanent) error.</summary>
        public bool Connected { get; private set; }

        /// <summary>Null until some error happened in underlying websocket.</summary>
        public string Error { get; private set; }


        // callbacks to higher level
        private Action<byte[], int> recvCallback;
        private Action openCallback;
        private Action<int, string> errorCallback;
        private Action<int, string> closeCallback;
        // logging callback
        public Action<LogLevel, string> DebugReturn { get; set; }


        public WebSocket(Uri url, string proxyAddress, Action openCallback, Action<byte[], int>  recvCallback, Action<int, string> errorCallback, Action<int, string> closeCallback, string protocols = null)
        {
            this.Url = url;
            this.ProxyAddress = proxyAddress;

            this.recvCallback = recvCallback;
            this.openCallback = openCallback;
            this.errorCallback = errorCallback;
            this.closeCallback = closeCallback;

            if (!string.IsNullOrEmpty(protocols))
>>>>>>> Stashed changes
            {
                this.protocols = protocols;
            }

<<<<<<< Updated upstream
            string protocol = mUrl.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
            {
                throw new ArgumentException("Unsupported protocol: " + protocol);
            }
        }

        public string ProxyAddress
        {
            get { return mProxyAddress; }
        }

        public void SendString(string str)
        {
            Send(Encoding.UTF8.GetBytes(str));
        }

        public string RecvString()
        {
            byte[] retval = Recv();
            if (retval == null)
                return null;
            return Encoding.UTF8.GetString(retval);
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int SocketCreate (string url, string protocols);

        [DllImport("__Internal")]
        private static extern int SocketState (int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketSend (int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern void SocketRecv (int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern int SocketRecvLength (int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketClose (int socketInstance);

        [DllImport("__Internal")]
        private static extern int SocketError (int socketInstance, byte[] ptr, int length);

        int m_NativeRef = 0;

        public void Send(byte[] buffer)
        {
            SocketSend (m_NativeRef, buffer, buffer.Length);
        }

        public byte[] Recv()
        {
            int length = SocketRecvLength (m_NativeRef);
            if (length == 0)
                return null;
            byte[] buffer = new byte[length];
            SocketRecv (m_NativeRef, buffer, length);
            return buffer;
        }

        public void Connect()
        {
            m_NativeRef = SocketCreate (mUrl.ToString(), this.protocols);

            //while (SocketState(m_NativeRef) == 0)
            //    yield return 0;
        }

        public void Close()
        {
            SocketClose(m_NativeRef);
        }

        public bool Connected
        {
            get { return SocketState(m_NativeRef) != 0; }
        }

        public string Error
        {
            get {
                const int bufsize = 1024;
                byte[] buffer = new byte[bufsize];
                int result = SocketError (m_NativeRef, buffer, bufsize);

                if (result == 0)
                    return null;

                return Encoding.UTF8.GetString (buffer);
            }
        }
        #else
        WebSocketSharp.WebSocket m_Socket;
        Queue<byte[]> m_Messages = new Queue<byte[]>();
        bool m_IsConnected = false;
        string m_Error = null;
=======
            string scheme = this.Url.Scheme;
            if (!scheme.Equals("ws") && !scheme.Equals("wss"))
            {
                throw new ArgumentException("Unsupported protocol: " + scheme);
            }
        }
    }


    // .net specific implementation using websocket-sharp.dll
    public partial class WebSocket
    {
        #if PHOTON_WEBSOCKET_CS

        public const string Implementation = "WebSocketSharp";

        WebSocketSharp.WebSocket m_Socket;
>>>>>>> Stashed changes


        public void Connect()
        {
<<<<<<< Updated upstream
            m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString(), new string[] {this.protocols});
            m_Socket.Log.Output = (ld, f) =>
=======
            this.m_Socket = new WebSocketSharp.WebSocket(this.Url.ToString(), new string[] {this.protocols});
            this.m_Socket.Log.Output = (ld, f) =>
>>>>>>> Stashed changes
                                  {
                                      var s = string.Format("WebSocketSharp: {0}", ld.Message);
                                      switch (ld.Level)
                                      {
                                          case WebSocketSharp.LogLevel.Trace:
                                          case WebSocketSharp.LogLevel.Debug:
<<<<<<< Updated upstream
                                              DebugReturn(DebugLevel.ALL, s);
                                              break;
                                          case WebSocketSharp.LogLevel.Info:
                                              DebugReturn(DebugLevel.INFO, s);
                                              break;
                                          case WebSocketSharp.LogLevel.Warn:
                                              DebugReturn(DebugLevel.WARNING, s);
                                              break;
                                          case WebSocketSharp.LogLevel.Error:
                                          case WebSocketSharp.LogLevel.Fatal:
                                              DebugReturn(DebugLevel.ERROR, s);
=======
                                              DebugReturn(LogLevel.Debug, s);
                                              break;
                                          case WebSocketSharp.LogLevel.Info:
                                              DebugReturn(LogLevel.Info, s);
                                              break;
                                          case WebSocketSharp.LogLevel.Warn:
                                              DebugReturn(LogLevel.Warning, s);
                                              break;
                                          case WebSocketSharp.LogLevel.Error:
                                          case WebSocketSharp.LogLevel.Fatal:
                                              DebugReturn(LogLevel.Error, s);
>>>>>>> Stashed changes
                                              break;
                                      }
                                  };

<<<<<<< Updated upstream
            string user = null;
            string pass = null;

            if (!String.IsNullOrEmpty(mProxyAddress))
            {
                var authDelim = mProxyAddress.IndexOf("@");
                if (authDelim != -1)
                {
                    user = mProxyAddress.Substring(0, authDelim);
                    mProxyAddress = mProxyAddress.Substring(authDelim + 1);
=======
            this.m_Socket.OnOpen += (sender, e) =>
                                    {
                                        this.Connected = true;
                                        this.openCallback();
                                    };
            this.m_Socket.OnMessage += (sender, e) =>
                                       {
                                           this.recvCallback(e.RawData, e.RawData.Length);
                                       };
            this.m_Socket.OnError += (sender, e) =>
                                     {
                                         this.Connected = false;
                                         this.Error = e.Message + (e.Exception == null ? "" : " / " + e.Exception);
                                         this.errorCallback(0, e.Message);
                                     };
            this.m_Socket.OnClose += (sender, e) =>
                                     {
                                         this.Connected = false;
                                         this.closeCallback(e.Code, e.Reason);
                                     };


            if (!String.IsNullOrEmpty(this.ProxyAddress))
            {
                string user = null;
                string pass = null;

                var authDelim = this.ProxyAddress.IndexOf("@");
                if (authDelim != -1)
                {
                    user = this.ProxyAddress.Substring(0, authDelim);
                    this.ProxyAddress = this.ProxyAddress.Substring(authDelim + 1);
>>>>>>> Stashed changes
                    var passDelim = user.IndexOf(":");
                    if (passDelim != -1)
                    {
                        pass = user.Substring(passDelim + 1);
                        user = user.Substring(0, passDelim);
                    }
                }

                // throws an exception, if scheme not specified
<<<<<<< Updated upstream
                m_Socket.SetProxy("http://" + mProxyAddress, user, pass);
            }

            if (m_Socket.IsSecure)
            {
                m_Socket.SslConfiguration.EnabledSslProtocols = m_Socket.SslConfiguration.EnabledSslProtocols | (SslProtocols)(3072 | 768);
            }

            m_Socket.OnMessage += (sender, e) => { m_Messages.Enqueue(e.RawData); };
            m_Socket.OnOpen += (sender, e) => { m_IsConnected = true; };
            m_Socket.OnError += (sender, e) => { m_Error = e.Message + (e.Exception == null ? "" : " / " + e.Exception); };

            this.m_Socket.OnClose += SocketOnClose;

            m_Socket.ConnectAsync();
        }

        private void SocketOnClose(object sender, CloseEventArgs e)
        {
            //UnityEngine.Debug.Log(e.Code.ToString());

            // this code is used for cases when the socket failed to get created (specifically used to detect "blocked by Windows firewall")
            // for some reason this situation is not calling OnError
            if (e.Code == 1006)
            {
                this.m_Error = e.Reason;
                this.m_IsConnected = false;
            }
        }

        public bool Connected
        {
            get { return m_IsConnected; }
=======
                this.m_Socket.SetProxy("http://" + this.ProxyAddress, user, pass);
            }

            if (this.m_Socket.IsSecure)
            {
                this.m_Socket.SslConfiguration.EnabledSslProtocols = this.m_Socket.SslConfiguration.EnabledSslProtocols | (SslProtocols)(3072 | 768);
            }


            this.m_Socket.ConnectAsync();
        }


        public void Close()
        {
            // at this low level we are fine with closing the socket async / non-blocking
            this.m_Socket.CloseAsync();
        }

        public void Send(byte[] buffer)
        {
            this.m_Socket.Send(buffer);
        }

        #endif
    }


    // js/native specific implementation
    public partial class WebSocket
    {
        #if PHOTON_WEBSOCKET_JS

        public const string Implementation = "JsLib";

        static Dictionary<int, WebSocket> instances = new Dictionary<int, WebSocket>();

        [DllImport("__Internal")]
        private static extern int SocketCreate(string url, string protocols, Action<int> openCallbackStatic,  Action<int, IntPtr, int> recvCallbackStatic, Action<int, int> errorCallbackStatic, Action<int, int> closeCallbackStatic);

        [DllImport("__Internal")]
        private static extern int SocketState (int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketSend (int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern void SocketClose (int socketInstance);

        [DllImport("__Internal")]
        private static extern int SocketError (int socketInstance, byte[] ptr, int length);

        private int m_NativeRef = 0;

        // TODO: discuss if we need this anymore?!
        public bool ConnectedOLD
        {
            get { return SocketState(m_NativeRef) != 0; }
        }

        private const int SocketErrorBufferSize = 1024;
        private readonly byte[] socketErrorBuffer = new byte[SocketErrorBufferSize];

        // TODO: discuss if we need this anymore?!
        public string ErrorOLD
        {
            get {
                int result = SocketError (m_NativeRef, this.socketErrorBuffer, SocketErrorBufferSize);

                if (result == 0)
                    return null;

                return Encoding.UTF8.GetString (this.socketErrorBuffer);
            }
        }


        public void Connect()
        {
            m_NativeRef = SocketCreate (this.Url.ToString(), this.protocols, OpenCallbackStatic, RecvCallbackStatic, ErrorCallbackStatic, CloseCallbackStatic);
            instances[m_NativeRef] = this;
        }

        public void Close()
        {
            SocketClose(m_NativeRef);
>>>>>>> Stashed changes
        }


        public void Send(byte[] buffer)
        {
<<<<<<< Updated upstream
            m_Socket.Send(buffer);
        }

        public byte[] Recv()
        {
            if (m_Messages.Count == 0)
                return null;
            return m_Messages.Dequeue();
        }

        public void Close()
        {
            m_Socket.Close();
        }

        public string Error
        {
            get { return m_Error; }
=======
            SocketSend (m_NativeRef, buffer, buffer.Length);
        }


        [MonoPInvokeCallback(typeof(Action<int, IntPtr, int>))]
        public static void RecvCallbackStatic(int instance, IntPtr p, int len)
        {
            instances[instance].RecvCallbackInstance(p, len);
        }

        private byte[] receiveBuffer;

        public void RecvCallbackInstance(IntPtr p, int len)
        {
            if (this.receiveBuffer == null || this.receiveBuffer.Length < len)
            {
                this.receiveBuffer = new byte[len];
            }
            Marshal.Copy(p, this.receiveBuffer, 0, len);

            this.recvCallback(this.receiveBuffer, len);
        }



        [MonoPInvokeCallback(typeof(Action<int>))]
        public static void OpenCallbackStatic(int instance)
        {
            instances[instance].OpenCallbackInstance();
        }

        public void OpenCallbackInstance()
        {
            this.Connected = true;
            this.openCallback();
        }



        [MonoPInvokeCallback(typeof(Action<int, int>))]
        public static void ErrorCallbackStatic(int instance, int code)
        {
            string msg;
            switch (code)
            {
                case 1001:
                    msg = "Endpoint going away.";
                    break;
                case 1002:
                    msg = "Protocol error.";
                    break;
                case 1003:
                    msg = "Unsupported message.";
                    break;
                case 1005:
                    msg = "No status.";
                    break;
                case 1006:
                    msg = "Abnormal disconnection.";
                    break;
                case 1009:
                    msg = "Data frame too large.";
                    break;
                default:
                    msg = "Error " + code;
                    break;
            }

            instances[instance].ErrorCallbackInstance(code, msg);
        }

        public void ErrorCallbackInstance(int code, string msg)
        {
            this.Connected = false;
            this.errorCallback(code, msg);
        }


        [MonoPInvokeCallback(typeof(Action<int, int>))]
        public static void CloseCallbackStatic(int instance, int code)
        {
            string msg = "n/a from jslib";
            instances[instance].CloseCallbackInstance(code, msg);
        }

        public void CloseCallbackInstance(int code, string msg)
        {
            this.Connected = false;
            this.closeCallback(code, msg);
>>>>>>> Stashed changes
        }
        #endif
    }
}
#endif