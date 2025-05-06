<<<<<<< Updated upstream
﻿// -----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
=======
// -----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Exit Games GmbH">
// Photon Realtime API - Copyright (C) 2022 Exit Games GmbH
>>>>>>> Stashed changes
// </copyright>
// <summary>Settings for Photon application(s) and the server to connect to.</summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_2017_4_OR_NEWER
#define SUPPORTED_UNITY
#endif

<<<<<<< Updated upstream
namespace Photon.Realtime
{
    using System;
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
=======

namespace Photon.Realtime
{
    using System;
    using Photon.Client;

    #if SUPPORTED_UNITY
    using SupportClass = Photon.Client.SupportClass;
>>>>>>> Stashed changes
    #endif


    /// <summary>
    /// Settings for Photon application(s) and the server to connect to.
    /// </summary>
    /// <remarks>
    /// This is Serializable for Unity, so it can be included in ScriptableObject instances.
    /// </remarks>
<<<<<<< Updated upstream
    #if !NETFX_CORE || SUPPORTED_UNITY
=======
    #if SUPPORTED_UNITY
>>>>>>> Stashed changes
    [Serializable]
    #endif
    public class AppSettings
    {
        /// <summary>AppId for Realtime or PUN.</summary>
        public string AppIdRealtime;

        /// <summary>AppId for Photon Fusion.</summary>
        public string AppIdFusion;

<<<<<<< Updated upstream
=======
        /// <summary>AppId for Photon Quantum.</summary>
        public string AppIdQuantum;

>>>>>>> Stashed changes
        /// <summary>AppId for Photon Chat.</summary>
        public string AppIdChat;

        /// <summary>AppId for Photon Voice.</summary>
        public string AppIdVoice;

        /// <summary>The AppVersion can be used to identify builds and will split the AppId distinct "Virtual AppIds" (important for matchmaking).</summary>
        public string AppVersion;


        /// <summary>If false, the app will attempt to connect to a Master Server (which is obsolete but sometimes still necessary).</summary>
        /// <remarks>if true, Server points to a NameServer (or is null, using the default), else it points to a MasterServer.</remarks>
        public bool UseNameServer = true;

        /// <summary>Can be set to any of the Photon Cloud's region names to directly connect to that region.</summary>
        /// <remarks>if this IsNullOrEmpty() AND UseNameServer == true, use BestRegion. else, use a server</remarks>
        public string FixedRegion;

        /// <summary>Set to a previous BestRegionSummary value before connecting.</summary>
        /// <remarks>
        /// This is a value used when the client connects to the "Best Region".<br/>
        /// If this is null or empty, all regions gets pinged. Providing a previous summary on connect,
        /// speeds up best region selection and makes the previously selected region "sticky".<br/>
        ///
        /// Unity clients should store the BestRegionSummary in the PlayerPrefs.
        /// You can store the new result by implementing <see cref="IConnectionCallbacks.OnConnectedToMaster"/>.
<<<<<<< Updated upstream
        /// If <see cref="LoadBalancingClient.SummaryToCache"/> is not null, store this string.
=======
        /// If <see cref="RealtimeClient.SummaryToCache"/> is not null, store this string.
>>>>>>> Stashed changes
        /// To avoid storing the value multiple times, you could set SummaryToCache to null.
        /// </remarks>
        #if SUPPORTED_UNITY
        [NonSerialized]
        #endif
        public string BestRegionSummaryFromStorage;

        /// <summary>The address (hostname or IP) of the server to connect to.</summary>
        public string Server;

        /// <summary>If not null, this sets the port of the first Photon server to connect to (that will "forward" the client as needed).</summary>
<<<<<<< Updated upstream
        public int Port;

        /// <summary>The address (hostname or IP and port) of the proxy server.</summary>
=======
        public ushort Port;

        /// <summary>
        /// Defines a proxy URL for WebSocket connections. Can be the proxy or point to a .pac file.
        /// </summary>
        /// <remarks>
        /// This URL supports various definitions:
        ///
        /// "user:pass@proxyaddress:port"<br/>
        /// "proxyaddress:port"<br/>
        /// "system:"<br/>
        /// "pac:"<br/>
        /// "pac:http://host/path/pacfile.pac"<br/>
        ///
        /// Important: Don't define a protocol, except to point to a pac file. The proxy address should not begin with http:// or https://.
        /// </remarks>
>>>>>>> Stashed changes
        public string ProxyServer;

        /// <summary>The network level protocol to use.</summary>
        public ConnectionProtocol Protocol = ConnectionProtocol.Udp;

<<<<<<< Updated upstream
        /// <summary>Enables a fallback to another protocol in case a connect to the Name Server fails.</summary>
        /// <remarks>See: LoadBalancingClient.EnableProtocolFallback.</remarks>
        public bool EnableProtocolFallback = true;

        /// <summary>Defines how authentication is done. On each system, once or once via a WSS connection (safe).</summary>
        public AuthModeOption AuthMode = AuthModeOption.Auth;

        /// <summary>If true, the client will request the list of currently available lobbies.</summary>
        public bool EnableLobbyStatistics;

        /// <summary>Log level for the network lib.</summary>
        public DebugLevel NetworkLogging = DebugLevel.ERROR;
=======
        /// <summary>Enables the fallback to WSS, should the initial connect to the Name Server fail. Some exceptions apply.</summary>
        /// <remarks>
        /// For security reasons, a fallback to another protocol is not done when using WSS or AuthMode.AuthOnceWss.
        /// That would compromise the expected security.
        ///
        /// If the fallback is impossible or if that connection also fails, the app logic must handle the case.
        /// It might even make sense to just try the same connection settings once more (or ask the user to do something about
        /// the network connectivity, firewalls, etc).
        /// 
        /// The fallback will use the default Name Server port as defined by ProtocolToNameServerPort.
        /// </remarks>
        public bool EnableProtocolFallback = true;

        /// <summary>Defines how authentication is done. On each system, once or once via a WSS connection (safe).</summary>
        public AuthModeOption AuthMode = AuthModeOption.AuthOnceWss;

        /// <summary>If true, the Master Server will send statistics for currently used lobbies. Defaults to false.</summary>
        /// <remarks>
        /// The lobby statistics can be useful if your title dynamically uses lobbies, depending (e.g.)
        /// on current player activity or such. The provided list of stats is capped to 500.
        ///
        /// Changing the value while being connected has no immediate effect. Set this before connecting.
        ///
        /// Implement ILobbyCallbacks.OnLobbyStatisticsUpdate to get the list of used lobbies.
        /// </remarks>
        public bool EnableLobbyStatistics;

        /// <summary>Log level for the PhotonPeer and connection. Useful to debug connection related issues.</summary>
        public LogLevel NetworkLogging = LogLevel.Error;
        
        /// <summary>Log level for the RealtimeClient and callbacks. Useful to get info about the client state, servers it uses and operations called.</summary>
        public LogLevel ClientLogging = LogLevel.Warning;

>>>>>>> Stashed changes

        /// <summary>If true, the Server field contains a Master Server address (if any address at all).</summary>
        public bool IsMasterServerAddress
        {
            get { return !this.UseNameServer; }
        }

        /// <summary>If true, the client should fetch the region list from the Name Server and find the one with best ping.</summary>
        /// <remarks>See "Best Region" in the online docs.</remarks>
        public bool IsBestRegion
        {
            get { return this.UseNameServer && string.IsNullOrEmpty(this.FixedRegion); }
        }

        /// <summary>If true, the default nameserver address for the Photon Cloud should be used.</summary>
        public bool IsDefaultNameServer
        {
            get { return this.UseNameServer && string.IsNullOrEmpty(this.Server); }
        }

        /// <summary>If true, the default ports for a protocol will be used.</summary>
        public bool IsDefaultPort
        {
            get { return this.Port <= 0; }
        }

<<<<<<< Updated upstream
=======

        /// <summary>Creates an AppSettings instance with default values.</summary>
        public AppSettings()
        {
        }

        /// <summary>
        /// Initializes the AppSettings with default values or the provided original.
        /// </summary>
        /// <param name="original">If non-null, all values are copied from the original.</param>
        public AppSettings(AppSettings original = null)
        {
            if (original != null)
            {
                original.CopyTo(this);
            }
        }


        /// <summary>Gets the AppId for a specific type of client.</summary>
        public string GetAppId(ClientAppType ct)
        {
            switch (ct)
            {
                case ClientAppType.Realtime:
                    return this.AppIdRealtime;
                case ClientAppType.Fusion:
                    return this.AppIdFusion;
                case ClientAppType.Quantum:
                    return this.AppIdQuantum;
                case ClientAppType.Voice:
                    return this.AppIdVoice;
                case ClientAppType.Chat:
                    return this.AppIdChat;
                default:
                    return null;
            }
        }


        /// <summary>Tries to detect the ClientAppType, based on which AppId values are present. Can detect Realtime, Fusion or Quantum. Used when the RealtimeClient.ClientType is set to detect.</summary>
        /// <returns>Most likely to be used ClientAppType or ClientAppType.Detect in conflicts.</returns>
        public ClientAppType ClientTypeDetect()
        {
            bool ra = !string.IsNullOrEmpty(this.AppIdRealtime);
            bool fa = !string.IsNullOrEmpty(this.AppIdFusion);
            bool qa = !string.IsNullOrEmpty(this.AppIdQuantum);

            if (ra && !fa && !qa)
            {
                return ClientAppType.Realtime;
            }
            if (fa && !ra && !qa)
            {
                return ClientAppType.Fusion;
            }
            if (qa && !ra && !fa)
            {
                return ClientAppType.Quantum;
            }

            Log.Error("ConnectUsingSettings requires that the AppSettings contain exactly one value set out of AppIdRealtime, AppIdFusion or AppIdQuantum.");
            return ClientAppType.Detect;
        }


>>>>>>> Stashed changes
        /// <summary>ToString but with more details.</summary>
        public string ToStringFull()
        {
            return string.Format(
                                 "appId {0}{1}{2}{3}" +
                                 "use ns: {4}, reg: {5}, {9}, " +
                                 "{6}{7}{8}" +
                                 "auth: {10}",
<<<<<<< Updated upstream
                                 String.IsNullOrEmpty(this.AppIdRealtime) ? string.Empty : "Realtime/PUN: " + this.HideAppId(this.AppIdRealtime) + ", ",
                                 String.IsNullOrEmpty(this.AppIdFusion) ? string.Empty : "Fusion: " + this.HideAppId(this.AppIdFusion) + ", ",
                                 String.IsNullOrEmpty(this.AppIdChat) ? string.Empty : "Chat: " + this.HideAppId(this.AppIdChat) + ", ",
                                 String.IsNullOrEmpty(this.AppIdVoice) ? string.Empty : "Voice: " + this.HideAppId(this.AppIdVoice) + ", ",
                                 String.IsNullOrEmpty(this.AppVersion) ? string.Empty : "AppVersion: " + this.AppVersion + ", ",
                                 "UseNameServer: " + this.UseNameServer + ", ",
                                 "Fixed Region: " + this.FixedRegion + ", ",
                                 //this.BestRegionSummaryFromStorage,
                                 String.IsNullOrEmpty(this.Server) ? string.Empty : "Server: " + this.Server + ", ",
                                 this.IsDefaultPort ? string.Empty : "Port: " + this.Port + ", ",
                                 String.IsNullOrEmpty(ProxyServer) ? string.Empty : "Proxy: " + this.ProxyServer + ", ",
=======
                                 string.IsNullOrEmpty(this.AppIdRealtime) ? string.Empty : "Realtime/PUN: " + this.HideAppId(this.AppIdRealtime) + ", ",
                                 string.IsNullOrEmpty(this.AppIdFusion) ? string.Empty : "Fusion: " + this.HideAppId(this.AppIdFusion) + ", ",
                                 string.IsNullOrEmpty(this.AppIdQuantum) ? string.Empty : "Quantum: " + this.HideAppId(this.AppIdQuantum) + ", ",
                                 string.IsNullOrEmpty(this.AppIdChat) ? string.Empty : "Chat: " + this.HideAppId(this.AppIdChat) + ", ",
                                 string.IsNullOrEmpty(this.AppIdVoice) ? string.Empty : "Voice: " + this.HideAppId(this.AppIdVoice) + ", ",
                                 string.IsNullOrEmpty(this.AppVersion) ? string.Empty : "AppVersion: " + this.AppVersion + ", ",
                                 "UseNameServer: " + this.UseNameServer + ", ",
                                 "Fixed Region: " + this.FixedRegion + ", ",
                                 //this.BestRegionSummaryFromStorage,
                                 string.IsNullOrEmpty(this.Server) ? string.Empty : "Server: " + this.Server + ", ",
                                 this.IsDefaultPort ? string.Empty : "Port: " + this.Port + ", ",
                                 string.IsNullOrEmpty(this.ProxyServer) ? string.Empty : "Proxy: " + this.ProxyServer + ", ",
>>>>>>> Stashed changes
                                 this.Protocol,
                                 this.AuthMode
                                 //this.EnableLobbyStatistics,
                                 //this.NetworkLogging,
                                );
        }


        /// <summary>Checks if a string is a Guid by attempting to create one.</summary>
        /// <param name="val">The potential guid to check.</param>
        /// <returns>True if new Guid(val) did not fail.</returns>
        public static bool IsAppId(string val)
        {
            try
            {
                new Guid(val);
            }
            catch
            {
                return false;
            }

            return true;
        }


        private string HideAppId(string appId)
        {
            return string.IsNullOrEmpty(appId) || appId.Length < 8
                       ? appId
                       : string.Concat(appId.Substring(0, 8), "***");
        }

<<<<<<< Updated upstream
        public AppSettings CopyTo(AppSettings d)
        {
            d.AppIdRealtime = this.AppIdRealtime;
            d.AppIdFusion = this.AppIdFusion;
            d.AppIdChat = this.AppIdChat;
            d.AppIdVoice = this.AppIdVoice;
            d.AppVersion = this.AppVersion;
            d.UseNameServer = this.UseNameServer;
            d.FixedRegion = this.FixedRegion;
            d.BestRegionSummaryFromStorage = this.BestRegionSummaryFromStorage;
            d.Server = this.Server;
            d.Port = this.Port;
            d.ProxyServer = this.ProxyServer;
            d.Protocol = this.Protocol;
            d.AuthMode = this.AuthMode;
            d.EnableLobbyStatistics = this.EnableLobbyStatistics;
            d.NetworkLogging = this.NetworkLogging;
            d.EnableProtocolFallback = this.EnableProtocolFallback;
            return d;
=======
        /// <summary>Copies values of this instance to the target.</summary>
        /// <param name="target">Target instance.</param>
        /// <returns>The target.</returns>
        public AppSettings CopyTo(AppSettings target)
        {
            target.AppIdRealtime = this.AppIdRealtime;
            target.AppIdFusion = this.AppIdFusion;
            target.AppIdQuantum = this.AppIdQuantum;
            target.AppIdChat = this.AppIdChat;
            target.AppIdVoice = this.AppIdVoice;
            target.AppVersion = this.AppVersion;
            target.UseNameServer = this.UseNameServer;
            target.FixedRegion = this.FixedRegion;
            target.BestRegionSummaryFromStorage = this.BestRegionSummaryFromStorage;
            target.Server = this.Server;
            target.Port = this.Port;
            target.ProxyServer = this.ProxyServer;
            target.Protocol = this.Protocol;
            target.AuthMode = this.AuthMode;
            target.EnableLobbyStatistics = this.EnableLobbyStatistics;
            target.ClientLogging = this.ClientLogging;
            target.NetworkLogging = this.NetworkLogging;
            target.EnableProtocolFallback = this.EnableProtocolFallback;
            return target;
>>>>>>> Stashed changes
        }
    }
}
