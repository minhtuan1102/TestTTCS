// ----------------------------------------------------------------------------
// <copyright file="Region.cs" company="Exit Games GmbH">
<<<<<<< Updated upstream
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Represents regions in the Photon Cloud.
=======
// Photon Realtime API - Copyright (C) 2022 Exit Games GmbH
// </copyright>
// <summary>
// Represents regions of the Photon Cloud.
>>>>>>> Stashed changes
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

<<<<<<< Updated upstream
#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
=======
#if UNITY_2017_4_OR_NEWER
>>>>>>> Stashed changes
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
<<<<<<< Updated upstream
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif


    public class Region
    {
=======
    using Photon.Client;

    #if SUPPORTED_UNITY
    using SupportClass = Photon.Client.SupportClass;
    #endif

    /// <summary>Summarizes a region's properties for Best Region selection and pinging.</summary>
    /// <remarks>Regions are usually representing geo locations (EU, US, etc) with a code, cluster, address.</remarks>
    /// <a href="https://doc.photonengine.com/en-us/realtime/current/connection-and-authentication/regions" target="_blank">Regions</a>
    public class Region
    {
        /// <summary>A region's code as string (e.g. US, EU).</summary>
>>>>>>> Stashed changes
        public string Code { get; private set; }

        /// <summary>Unlike the CloudRegionCode, this may contain cluster information.</summary>
        public string Cluster { get; private set; }

<<<<<<< Updated upstream
=======
        /// <summary>The address of this region.</summary>
>>>>>>> Stashed changes
        public string HostAndPort { get; protected internal set; }

        /// <summary>Weighted ping time.</summary>
        /// <remarks>
        /// Regions gets pinged 5 times (RegionPinger.Attempts).
        /// Out of those, the worst rtt is discarded and the best will be counted two times for a weighted average.
        /// </remarks>
        public int Ping { get; set; }

<<<<<<< Updated upstream
        public bool WasPinged { get { return this.Ping != int.MaxValue; } }

=======
        /// <summary>True if the region was pinged and Ping contains a valid value.</summary>
        public bool WasPinged { get { return this.Ping != int.MaxValue; } }

        /// <summary>Constructs a new Region instance from code and address.</summary>
>>>>>>> Stashed changes
        public Region(string code, string address)
        {
            this.SetCodeAndCluster(code);
            this.HostAndPort = address;
            this.Ping = int.MaxValue;
        }

<<<<<<< Updated upstream
        public Region(string code, int ping)
        {
            this.SetCodeAndCluster(code);
            this.Ping = ping;
        }

=======
        /// <summary>Reads code and cluster (combined as "code/cluster") into separate values.</summary>
>>>>>>> Stashed changes
        private void SetCodeAndCluster(string codeAsString)
        {
            if (codeAsString == null)
            {
                this.Code = "";
                this.Cluster = "";
                return;
            }

            codeAsString = codeAsString.ToLower();
            int slash = codeAsString.IndexOf('/');
            this.Code = slash <= 0 ? codeAsString : codeAsString.Substring(0, slash);
            this.Cluster = slash <= 0 ? "" : codeAsString.Substring(slash+1, codeAsString.Length-slash-1);
        }

<<<<<<< Updated upstream
=======
        /// <summary>Provides a string representation of the Region.</summary>
>>>>>>> Stashed changes
        public override string ToString()
        {
            return this.ToString(false);
        }

<<<<<<< Updated upstream
=======
        /// <summary>Provides a string representation of the Region, optionally including the address.</summary>
>>>>>>> Stashed changes
        public string ToString(bool compact = false)
        {
            string regionCluster = this.Code;
            if (!string.IsNullOrEmpty(this.Cluster))
            {
                regionCluster += "/" + this.Cluster;
            }

            if (compact)
            {
                return string.Format("{0}:{1}", regionCluster, this.Ping);
            }
            else
            {
                return string.Format("{0}[{2}]: {1}ms", regionCluster, this.Ping, this.HostAndPort);
            }
        }
    }
}