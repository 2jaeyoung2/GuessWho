using Photon.Pun;

using System.Diagnostics;

using UnityEngine;

namespace ZL.Unity.Server.Photon
{
    [AddComponentMenu("ZL/Server/Photon/Photon Manager (Singleton)")]

    [DisallowMultipleComponent]

    [RequireComponent(typeof(PhotonView))]

    public sealed class PhotonManager : MonoBehaviour, IMonoSingleton<PhotonManager>
    {
        [SerializeField]

        [UsingCustomProperty]

        [ReadOnly(true)]

        [GetComponent]

        private PhotonView photonView;

        [Conditional("UNITY_EDITOR")]

        private void Awake()
        {
            ISingleton<PhotonManager>.TrySetInstance(this);
        }

        private void OnDestroy()
        {
            ISingleton<PhotonManager>.Release(this);
        }

        public void RPCLog(RpcTarget target, object message)
        {
            photonView.RPC("Log", target, message);
        }

        [Conditional("UNITY_EDITOR")]

        [PunRPC]

        private void Log(object message, Object context)
        {
            FixedDebug.Log(message, context);
        }
    }
}