#if UNITY_WEBGL || WEBSOCKET || WEBSOCKET_PROXYCONFIG

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketWebTcp.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Internal class to encapsulate the network i/o functionality for the realtime library.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


<<<<<<< Updated upstream
namespace ExitGames.Client.Photon
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Scripting;
    using SupportClassPun = SupportClass;


    /// <summary>
    /// Yield Instruction to Wait for real seconds. Very important to keep connection working if Time.TimeScale is altered, we still want accurate network events
    /// </summary>
    public sealed class WaitForRealSeconds : CustomYieldInstruction
    {
        private readonly float _endTime;

        public override bool keepWaiting
        {
            get { return this._endTime > Time.realtimeSinceStartup; }
        }

        public WaitForRealSeconds(float seconds)
        {
            this._endTime = Time.realtimeSinceStartup + seconds;
        }
    }


    /// <summary>
    /// Internal class to encapsulate the network i/o functionality for the realtime libary.
    /// </summary>
    public class SocketWebTcp : IPhotonSocket, IDisposable
=======
namespace Photon.Client
{
    using System;

    #if UNITY_2019_3_OR_NEWER
    using UnityEngine.Scripting;
    #endif

    /// <summary>
    /// Internal class to encapsulate the network i/o functionality for the realtime library.
    /// </summary>
    [Preserve]
    public class SocketWebTcp : PhotonSocket, IDisposable
>>>>>>> Stashed changes
    {
        private WebSocket sock;

        private readonly object syncer = new object();

        [Preserve]
        public SocketWebTcp(PeerBase npeer) : base(npeer)
        {
            this.ServerAddress = npeer.ServerAddress;
            this.ProxyServerAddress = npeer.ProxyServerAddress;
<<<<<<< Updated upstream
            if (this.ReportDebugOfLevel(DebugLevel.INFO))
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "new SocketWebTcp() for Unity. Server: " + this.ServerAddress + (String.IsNullOrEmpty(this.ProxyServerAddress) ? "" : ", Proxy: " + this.ProxyServerAddress));
            }

            //this.Protocol = ConnectionProtocol.WebSocket;
=======
            if (this.ReportDebugOfLevel(LogLevel.Info))
            {
                this.Listener.DebugReturn(LogLevel.Info, "SocketWebTcp() "+ WebSocket.Implementation+". Server: " + this.ServerAddress + (String.IsNullOrEmpty(this.ProxyServerAddress) ? "" : ", Proxy: " + this.ProxyServerAddress));
            }

>>>>>>> Stashed changes
            this.PollReceive = false;
        }

        public void Dispose()
        {
            this.State = PhotonSocketState.Disconnecting;

            if (this.sock != null)
            {
                try
                {
                    if (this.sock.Connected)
                    {
                        this.sock.Close();
                    }
                }
                catch (Exception ex)
                {
<<<<<<< Updated upstream
                    this.EnqueueDebugReturn(DebugLevel.INFO, "Exception in SocketWebTcp.Dispose(): " + ex);
=======
                    this.EnqueueDebugReturn(LogLevel.Info, "Exception in SocketWebTcp.Dispose(): " + ex);
>>>>>>> Stashed changes
                }
            }

            this.sock = null;
            this.State = PhotonSocketState.Disconnected;
        }

<<<<<<< Updated upstream
        GameObject websocketConnectionObject;

        public override bool Connect()
        {
            //bool baseOk = base.Connect();
            //if (!baseOk)
            //{
            //    return false;
            //}


            this.State = PhotonSocketState.Connecting;


            if (this.websocketConnectionObject != null)
            {
                UnityEngine.Object.Destroy(this.websocketConnectionObject);
            }

            this.websocketConnectionObject = new GameObject("websocketConnectionObject");
            MonoBehaviour mb = this.websocketConnectionObject.AddComponent<MonoBehaviourExt>();
            this.websocketConnectionObject.hideFlags = HideFlags.HideInHierarchy;
            UnityEngine.Object.DontDestroyOnLoad(this.websocketConnectionObject);


            this.ConnectAddress += "&IPv6"; // this makes the Photon Server return a host name for the next server (NS points to MS and MS points to GS)


=======

        public override bool Connect()
        {
            this.State = PhotonSocketState.Connecting;


            if (!this.ConnectAddress.Contains("IPv6"))
            {
                this.ConnectAddress += "&IPv6"; // this makes the Photon Server return a host name for the next server (NS points to MS and MS points to GS)
            }

>>>>>>> Stashed changes
            // earlier, we read the proxy address/scheme and failed to connect entirely, if that wasn't successful...
            // it was either successful (using the resulting proxy address) or no connect at all...

            // we want:
            // WITH support: fail if the scheme is wrong or use it if possible
            // WITHOUT support: use proxy address, if it's a direct value (not a scheme we provide) or fail if it's a scheme

            string proxyServerAddress;
            if (!this.ReadProxyConfigScheme(this.ProxyServerAddress, this.ServerAddress, out proxyServerAddress))
            {
<<<<<<< Updated upstream
                this.Listener.DebugReturn(DebugLevel.INFO, "ReadProxyConfigScheme() failed. Using no proxy.");
=======
                this.Listener.DebugReturn(LogLevel.Info, "ReadProxyConfigScheme() failed. Using no proxy.");
>>>>>>> Stashed changes
            }


            try
            {
<<<<<<< Updated upstream
                this.sock = new WebSocket(new Uri(this.ConnectAddress), proxyServerAddress, this.SerializationProtocol);
                this.sock.DebugReturn = (DebugLevel l, string s) =>
=======
                this.sock = new WebSocket(new Uri(this.ConnectAddress), proxyServerAddress, this.OpenCallback, this.ReceiveCallback, this.ErrorCallback, this.CloseCallback, this.SerializationProtocol);
                this.sock.DebugReturn = (LogLevel l, string s) =>
>>>>>>> Stashed changes
                                        {
                                            if (this.State != PhotonSocketState.Disconnected)
                                            {
                                                this.Listener.DebugReturn(l, this.State + " " + s);
                                            }
                                        };

                this.sock.Connect();
<<<<<<< Updated upstream
                mb.StartCoroutine(this.ReceiveLoop());

=======
>>>>>>> Stashed changes
                return true;
            }
            catch (Exception e)
            {
<<<<<<< Updated upstream
                this.Listener.DebugReturn(DebugLevel.ERROR, "SocketWebTcp.Connect() caught exception: " + e);
=======
                this.Listener.DebugReturn(LogLevel.Error, "SocketWebTcp.Connect() caught exception: " + e);
>>>>>>> Stashed changes
                return false;
            }
        }

<<<<<<< Updated upstream
=======
        private void CloseCallback(int code, string reason)
        {
            if (this.State == PhotonSocketState.Connecting)
            {
                this.HandleException(StatusCode.ExceptionOnConnect); // sets state to Disconnecting
                return;
            }

            // passing-on close only if this socket is still used / expected to be connected
            if (this.State != PhotonSocketState.Disconnecting && this.State != PhotonSocketState.Disconnected)
            {
                this.Listener.DebugReturn(LogLevel.Error, "SocketWebTcp.CloseCallback(). Going to disconnect. Server: " + this.ServerAddress + " Error: " + code + " Reason: " + reason);
                this.HandleException(StatusCode.DisconnectByServerReasonUnknown); // sets state to Disconnecting
            }
        }

        // code can be from JsLib or WebSocket-Sharp, so it is not guaranteed to be the same in both cases
        private void ErrorCallback(int code, string message)
        {
            // passing-on errors only if this socket is still used / expected to be connected
            if (this.State != PhotonSocketState.Disconnecting && this.State != PhotonSocketState.Disconnected)
            {
                this.Listener.DebugReturn(LogLevel.Error, "SocketWebTcp.ErrorCallback(). Going to disconnect. Server: " + this.ServerAddress + " Error: " + code + " Message: " + message);
                this.HandleException(this.State != PhotonSocketState.Connected ? StatusCode.ExceptionOnConnect : StatusCode.ExceptionOnReceive); // sets state to Disconnecting
            }
        }

        private void OpenCallback()
        {
            if (State == PhotonSocketState.Connecting)
            {
                this.State = PhotonSocketState.Connected;
            }
        }

>>>>>>> Stashed changes

        /// <summary>
        /// Attempts to read a proxy configuration defined by a address prefix. Only available to Industries Circle members on demand.
        /// </summary>
        /// <remarks>
        /// Extended proxy support is available to Industries Circle members. Where available, proxy addresses may be defined as 'auto:', 'pac:' or 'system:'.
        /// In all other cases, the proxy address is used as is and fails to read configs (if one of the listed schemes is used).
        ///
        /// Requires file ProxyAutoConfig.cs and compile define: WEBSOCKET_PROXYCONFIG_SUPPORT.
        /// </remarks>
        /// <param name="proxyAddress">Proxy address from the server configuration.</param>
        /// <param name="url">Url to connect to (one of the Photon servers).</param>
        /// <param name="proxyUrl">Resulting proxy URL to use.</param>
        /// <returns>False if there is some error and the resulting proxy address should not be used.</returns>
        private bool ReadProxyConfigScheme(string proxyAddress, string url, out string proxyUrl)
        {
            proxyUrl = null;

            #if !WEBSOCKET_PROXYCONFIG

            if (!string.IsNullOrEmpty(proxyAddress))
            {
                if (proxyAddress.StartsWith("auto:") || proxyAddress.StartsWith("pac:") || proxyAddress.StartsWith("system:"))
                {
<<<<<<< Updated upstream
                    this.Listener.DebugReturn(DebugLevel.WARNING, "Proxy configuration via auto, pac or system is only supported with the WEBSOCKET_PROXYCONFIG define. Using no proxy instead.");
=======
                    this.Listener.DebugReturn(LogLevel.Warning, "Proxy configuration via auto, pac or system is only supported with the WEBSOCKET_PROXYCONFIG define. Using no proxy instead.");
>>>>>>> Stashed changes
                    return true;
                }
                proxyUrl = proxyAddress;
            }

            return true;

            #else

            if (!string.IsNullOrEmpty(proxyAddress))
            {
                var httpUrl = url.ToString().Replace("ws://", "http://").Replace("wss://", "https://"); // http(s) schema required in GetProxyForUrlUsingPac call
                bool auto = proxyAddress.StartsWith("auto:", StringComparison.InvariantCultureIgnoreCase);
                bool pac = proxyAddress.StartsWith("pac:", StringComparison.InvariantCultureIgnoreCase);

                if (auto || pac)
                {
                    string pacUrl = "";
                    if (pac)
                    {
                        pacUrl = proxyAddress.Substring(4);
                        if (pacUrl.IndexOf("://") == -1)
                        {
                            pacUrl = "http://" + pacUrl; //default to http
                        }
                    }

                    string processTypeStr = auto ? "auto detect" : "pac url " + pacUrl;

<<<<<<< Updated upstream
                    this.Listener.DebugReturn(DebugLevel.INFO, "WebSocket Proxy: " + url + " " + processTypeStr);
=======
                    this.Listener.DebugReturn(LogLevel.Info, "WebSocket Proxy: " + url + " " + processTypeStr);
>>>>>>> Stashed changes

                    string errDescr = "";
                    var err = ProxyAutoConfig.GetProxyForUrlUsingPac(httpUrl, pacUrl, out proxyUrl, out errDescr);

                    if (err != 0)
                    {
<<<<<<< Updated upstream
                        this.Listener.DebugReturn(DebugLevel.ERROR, "WebSocket Proxy: " + url + " " + processTypeStr + " ProxyAutoConfig.GetProxyForUrlUsingPac() error: " + err + " (" + errDescr + ")");
=======
                        this.Listener.DebugReturn(LogLevel.Error, "WebSocket Proxy: " + url + " " + processTypeStr + " ProxyAutoConfig.GetProxyForUrlUsingPac() error: " + err + " (" + errDescr + ")");
>>>>>>> Stashed changes
                        return false;
                    }
                }
                else if (proxyAddress.StartsWith("system:", StringComparison.InvariantCultureIgnoreCase))
                {
<<<<<<< Updated upstream
                    this.Listener.DebugReturn(DebugLevel.INFO, "WebSocket Proxy: " + url + " system settings");
=======
                    this.Listener.DebugReturn(LogLevel.Info, "WebSocket Proxy: " + url + " system settings");
>>>>>>> Stashed changes
                    string proxyAutoConfigPacUrl;
                    var err = ProxySystemSettings.GetProxy(out proxyUrl, out proxyAutoConfigPacUrl);
                    if (err != 0)
                    {
<<<<<<< Updated upstream
                        this.Listener.DebugReturn(DebugLevel.ERROR, "WebSocket Proxy: " + url + " system settings ProxySystemSettings.GetProxy() error: " + err);
=======
                        this.Listener.DebugReturn(LogLevel.Error, "WebSocket Proxy: " + url + " system settings ProxySystemSettings.GetProxy() error: " + err);
>>>>>>> Stashed changes
                        return false;
                    }
                    if (proxyAutoConfigPacUrl != null)
                    {
                        if (proxyAutoConfigPacUrl.IndexOf("://") == -1)
                        {
                            proxyAutoConfigPacUrl = "http://" + proxyAutoConfigPacUrl; //default to http
                        }
<<<<<<< Updated upstream
                        this.Listener.DebugReturn(DebugLevel.INFO, "WebSocket Proxy: " + url + " system settings AutoConfigURL: " + proxyAutoConfigPacUrl);
=======
                        this.Listener.DebugReturn(LogLevel.Info, "WebSocket Proxy: " + url + " system settings AutoConfigURL: " + proxyAutoConfigPacUrl);
>>>>>>> Stashed changes
                        string errDescr = "";
                        err = ProxyAutoConfig.GetProxyForUrlUsingPac(httpUrl, proxyAutoConfigPacUrl, out proxyUrl, out errDescr);

                        if (err != 0)
                        {
<<<<<<< Updated upstream
                            this.Listener.DebugReturn(DebugLevel.ERROR, "WebSocket Proxy: " + url + " system settings AutoConfigURLerror: " + err + " (" + errDescr + ")");
=======
                            this.Listener.DebugReturn(LogLevel.Error, "WebSocket Proxy: " + url + " system settings AutoConfigURLerror: " + err + " (" + errDescr + ")");
>>>>>>> Stashed changes
                            return false;
                        }
                    }
                }
                else
                {
                    proxyUrl = proxyAddress;
                }

<<<<<<< Updated upstream
                this.Listener.DebugReturn(DebugLevel.INFO, "WebSocket Proxy: " + url + " -> " + (string.IsNullOrEmpty(proxyUrl) ? "DIRECT" : "PROXY " + proxyUrl));
=======
                this.Listener.DebugReturn(LogLevel.Info, "WebSocket Proxy: " + url + " -> " + (string.IsNullOrEmpty(proxyUrl) ? "DIRECT" : "PROXY " + proxyUrl));
>>>>>>> Stashed changes
            }

            return true;
            #endif
        }


<<<<<<< Updated upstream

        public override bool Disconnect()
        {
            if (this.ReportDebugOfLevel(DebugLevel.INFO))
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "SocketWebTcp.Disconnect()");
=======
        public override bool Disconnect()
        {
            if (this.ReportDebugOfLevel(LogLevel.Info))
            {
                this.Listener.DebugReturn(LogLevel.Info, "SocketWebTcp.Disconnect()");
>>>>>>> Stashed changes
            }

            this.State = PhotonSocketState.Disconnecting;

            lock (this.syncer)
            {
                if (this.sock != null)
                {
                    try
                    {
                        this.sock.Close();
                    }
                    catch (Exception ex)
                    {
<<<<<<< Updated upstream
                        this.Listener.DebugReturn(DebugLevel.ERROR, "Exception in SocketWebTcp.Disconnect(): " + ex);
=======
                        this.Listener.DebugReturn(LogLevel.Error, "Exception in SocketWebTcp.Disconnect(): " + ex);
>>>>>>> Stashed changes
                    }

                    this.sock = null;
                }
            }

<<<<<<< Updated upstream
            if (this.websocketConnectionObject != null)
            {
                UnityEngine.Object.Destroy(this.websocketConnectionObject);
            }

=======
>>>>>>> Stashed changes
            this.State = PhotonSocketState.Disconnected;
            return true;
        }

<<<<<<< Updated upstream
        /// <summary>
        /// used by TPeer*
        /// </summary>
=======
        /// <summary>Used by TPeer</summary>
>>>>>>> Stashed changes
        public override PhotonSocketError Send(byte[] data, int length)
        {
            if (this.State != PhotonSocketState.Connected)
            {
                return PhotonSocketError.Skipped;
            }

            try
            {
                if (data.Length > length)
                {
                    byte[] trimmedData = new byte[length];
                    Buffer.BlockCopy(data, 0, trimmedData, 0, length);
                    data = trimmedData;
                }

<<<<<<< Updated upstream
                //if (this.ReportDebugOfLevel(DebugLevel.ALL))
                //{
                //    this.Listener.DebugReturn(DebugLevel.ALL, "Sending: " + SupportClassPun.ByteArrayToString(data));
                //}

=======
>>>>>>> Stashed changes
                if (this.sock != null)
                {
                    this.sock.Send(data);
                }
            }
            catch (Exception e)
            {
<<<<<<< Updated upstream
                this.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send to: " + this.ServerAddress + ". " + e.Message);
=======
                this.Listener.DebugReturn(LogLevel.Error, "Cannot send to: " + this.ServerAddress + ". " + e.Message);
>>>>>>> Stashed changes

                this.HandleException(StatusCode.Exception);
                return PhotonSocketError.Exception;
            }

            return PhotonSocketError.Success;
        }

<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
        public override PhotonSocketError Receive(out byte[] data)
        {
            data = null;
            return PhotonSocketError.NoData;
        }

<<<<<<< Updated upstream

        internal const int ALL_HEADER_BYTES = 9;
        internal const int TCP_HEADER_BYTES = 7;
        internal const int MSG_HEADER_BYTES = 2;

        public IEnumerator ReceiveLoop()
        {
            //this.Listener.DebugReturn(DebugLevel.INFO, "ReceiveLoop()");
            if (this.sock != null)
            {
                while (this.sock != null && !this.sock.Connected && this.sock.Error == null)
                {
                    yield return new WaitForRealSeconds(0.1f);
                }

                if (this.sock != null)
                {
                    if (this.sock.Error != null)
                    {
                        this.Listener.DebugReturn(DebugLevel.ERROR, "Exiting receive thread. Server: " + this.ServerAddress + " Error: " + this.sock.Error);
                        this.HandleException(StatusCode.ExceptionOnConnect);
                    }
                    else
                    {
                        // connected
                        if (this.ReportDebugOfLevel(DebugLevel.ALL))
                        {
                            this.Listener.DebugReturn(DebugLevel.ALL, "Receiving by websocket. this.State: " + this.State);
                        }

                        this.State = PhotonSocketState.Connected;
                        this.peerBase.OnConnect();

                        while (this.State == PhotonSocketState.Connected)
                        {
                            if (this.sock != null)
                            {
                                if (this.sock.Error != null)
                                {
                                    this.Listener.DebugReturn(DebugLevel.ERROR, "Exiting receive thread (inside loop). Server: " + this.ServerAddress + " Error: " + this.sock.Error);
                                    this.HandleException(StatusCode.ExceptionOnReceive);
                                    break;
                                }
                                else
                                {
                                    byte[] inBuff = this.sock.Recv();
                                    if (inBuff == null || inBuff.Length == 0)
                                    {
                                        // nothing received. wait a bit, try again
                                        yield return new WaitForRealSeconds(0.02f);
                                        continue;
                                    }

                                    //if (this.ReportDebugOfLevel(DebugLevel.ALL))
                                    //{
                                    //    this.Listener.DebugReturn(DebugLevel.ALL, "TCP << " + inBuff.Length + " = " + SupportClassPun.ByteArrayToString(inBuff));
                                    //}

                                    if (inBuff.Length > 0)
                                    {
                                        try
                                        {
                                            this.HandleReceivedDatagram(inBuff, inBuff.Length, false);
                                        }
                                        catch (Exception e)
                                        {
                                            if (this.State != PhotonSocketState.Disconnecting && this.State != PhotonSocketState.Disconnected)
                                            {
                                                if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                                                {
                                                    this.EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + this.State + ". Server: '" + this.ServerAddress + "' Exception: " + e);
                                                }

                                                this.HandleException(StatusCode.ExceptionOnReceive);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.Disconnect();
        }


        private class MonoBehaviourExt : MonoBehaviour
        {
=======
        public void ReceiveCallback(byte[] buf, int len)
        {
            // once the websocket is disconnecting / disconnected, it should not receive anything anymore
            if (State == PhotonSocketState.Disconnecting || State == PhotonSocketState.Disconnected)
            {
                return;
            }

            try
            {
                this.HandleReceivedDatagram(buf, len, false);
            }
            catch (Exception e)
            {
                if (this.State != PhotonSocketState.Disconnecting && this.State != PhotonSocketState.Disconnected)
                {
                    if (this.ReportDebugOfLevel(LogLevel.Error))
                    {
                        this.EnqueueDebugReturn(LogLevel.Error, "SocketWebTcp.ReceiveCallback() caught exception. Going to disconnect. State: " + this.State + ". Server: '" + this.ServerAddress + "' Exception: " + e);
                    }

                    this.HandleException(StatusCode.ExceptionOnReceive);
                }
            }
>>>>>>> Stashed changes
        }
    }
}

<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
#endif