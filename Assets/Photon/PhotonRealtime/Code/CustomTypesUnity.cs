// ----------------------------------------------------------------------------
<<<<<<< Updated upstream
// <copyright file="CustomTypes.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Sets up support for Unity-specific types. Can be a blueprint how to register your own Custom Types for sending.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif

=======
// <copyright file="CustomTypesUnity.cs" company="Exit Games GmbH">
// Photon Realtime API - Copyright (C) 2022 Exit Games GmbH
// </copyright>
// <summary>Sets up support for Unity-specific types.</summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#if UNITY_2017_4_OR_NEWER
#define SUPPORTED_UNITY
#endif


>>>>>>> Stashed changes
#if SUPPORTED_UNITY
namespace Photon.Realtime
{
    using Photon.Realtime;
<<<<<<< Updated upstream
    using ExitGames.Client.Photon;
=======
    using Photon.Client;
>>>>>>> Stashed changes
    using UnityEngine;
    using Debug = UnityEngine.Debug;


    /// <summary>
    /// Internally used class, containing de/serialization methods for various Unity-specific classes.
    /// Adding those to the Photon serialization protocol allows you to send them in events, etc.
    /// </summary>
    internal static class CustomTypesUnity
    {
        private const int SizeV2 = 2 * 4;
        private const int SizeV3 = 3 * 4;
        private const int SizeQuat = 4 * 4;


        /// <summary>Register de/serializer methods for Unity specific types. Makes the types usable in RaiseEvent and PUN.</summary>
        internal static void Register()
        {
            PhotonPeer.RegisterType(typeof(Vector2), (byte) 'W', SerializeVector2, DeserializeVector2);
            PhotonPeer.RegisterType(typeof(Vector3), (byte) 'V', SerializeVector3, DeserializeVector3);
            PhotonPeer.RegisterType(typeof(Quaternion), (byte) 'Q', SerializeQuaternion, DeserializeQuaternion);
        }


        #region Custom De/Serializer Methods

        public static readonly byte[] memVector3 = new byte[SizeV3];

        private static short SerializeVector3(StreamBuffer outStream, object customobject)
        {
            Vector3 vo = (Vector3) customobject;

            int index = 0;
            lock (memVector3)
            {
                byte[] bytes = memVector3;
<<<<<<< Updated upstream
                Protocol.Serialize(vo.x, bytes, ref index);
                Protocol.Serialize(vo.y, bytes, ref index);
                Protocol.Serialize(vo.z, bytes, ref index);
=======
                MessageProtocol.Serialize(vo.x, bytes, ref index);
                MessageProtocol.Serialize(vo.y, bytes, ref index);
                MessageProtocol.Serialize(vo.z, bytes, ref index);
>>>>>>> Stashed changes
                outStream.Write(bytes, 0, SizeV3);
            }

            return SizeV3;
        }

        private static object DeserializeVector3(StreamBuffer inStream, short length)
        {
            Vector3 vo = new Vector3();
            if (length != SizeV3)
            {
                return vo;
            }

            lock (memVector3)
            {
                inStream.Read(memVector3, 0, SizeV3);
                int index = 0;
<<<<<<< Updated upstream
                Protocol.Deserialize(out vo.x, memVector3, ref index);
                Protocol.Deserialize(out vo.y, memVector3, ref index);
                Protocol.Deserialize(out vo.z, memVector3, ref index);
=======
                MessageProtocol.Deserialize(out vo.x, memVector3, ref index);
                MessageProtocol.Deserialize(out vo.y, memVector3, ref index);
                MessageProtocol.Deserialize(out vo.z, memVector3, ref index);
>>>>>>> Stashed changes
            }

            return vo;
        }


        public static readonly byte[] memVector2 = new byte[SizeV2];

        private static short SerializeVector2(StreamBuffer outStream, object customobject)
        {
            Vector2 vo = (Vector2) customobject;
            lock (memVector2)
            {
                byte[] bytes = memVector2;
                int index = 0;
<<<<<<< Updated upstream
                Protocol.Serialize(vo.x, bytes, ref index);
                Protocol.Serialize(vo.y, bytes, ref index);
=======
                MessageProtocol.Serialize(vo.x, bytes, ref index);
                MessageProtocol.Serialize(vo.y, bytes, ref index);
>>>>>>> Stashed changes
                outStream.Write(bytes, 0, SizeV2);
            }

            return SizeV2;
        }

        private static object DeserializeVector2(StreamBuffer inStream, short length)
        {
            Vector2 vo = new Vector2();
            if (length != SizeV2)
            {
                return vo;
            }

            lock (memVector2)
            {
                inStream.Read(memVector2, 0, SizeV2);
                int index = 0;
<<<<<<< Updated upstream
                Protocol.Deserialize(out vo.x, memVector2, ref index);
                Protocol.Deserialize(out vo.y, memVector2, ref index);
=======
                MessageProtocol.Deserialize(out vo.x, memVector2, ref index);
                MessageProtocol.Deserialize(out vo.y, memVector2, ref index);
>>>>>>> Stashed changes
            }

            return vo;
        }


        public static readonly byte[] memQuarternion = new byte[SizeQuat];

        private static short SerializeQuaternion(StreamBuffer outStream, object customobject)
        {
            Quaternion o = (Quaternion) customobject;

            lock (memQuarternion)
            {
                byte[] bytes = memQuarternion;
                int index = 0;
<<<<<<< Updated upstream
                Protocol.Serialize(o.w, bytes, ref index);
                Protocol.Serialize(o.x, bytes, ref index);
                Protocol.Serialize(o.y, bytes, ref index);
                Protocol.Serialize(o.z, bytes, ref index);
=======
                MessageProtocol.Serialize(o.w, bytes, ref index);
                MessageProtocol.Serialize(o.x, bytes, ref index);
                MessageProtocol.Serialize(o.y, bytes, ref index);
                MessageProtocol.Serialize(o.z, bytes, ref index);
>>>>>>> Stashed changes
                outStream.Write(bytes, 0, SizeQuat);
            }

            return 4 * 4;
        }

        private static object DeserializeQuaternion(StreamBuffer inStream, short length)
        {
            Quaternion o = Quaternion.identity;
            if (length != SizeQuat)
            {
                return o;
            }

            lock (memQuarternion)
            {
                inStream.Read(memQuarternion, 0, SizeQuat);
                int index = 0;
<<<<<<< Updated upstream
                Protocol.Deserialize(out o.w, memQuarternion, ref index);
                Protocol.Deserialize(out o.x, memQuarternion, ref index);
                Protocol.Deserialize(out o.y, memQuarternion, ref index);
                Protocol.Deserialize(out o.z, memQuarternion, ref index);
=======
                MessageProtocol.Deserialize(out o.w, memQuarternion, ref index);
                MessageProtocol.Deserialize(out o.x, memQuarternion, ref index);
                MessageProtocol.Deserialize(out o.y, memQuarternion, ref index);
                MessageProtocol.Deserialize(out o.z, memQuarternion, ref index);
>>>>>>> Stashed changes
            }

            return o;
        }

        #endregion
    }
}
#endif