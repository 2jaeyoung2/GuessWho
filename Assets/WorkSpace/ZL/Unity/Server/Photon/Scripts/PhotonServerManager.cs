using Photon.Pun;

using Photon.Realtime;

using System;

using System.Collections;

using System.Diagnostics;

using UnityEngine;

using UnityEngine.Events;

using ZL.Unity.Collections;

namespace ZL.Unity.Server.Photon
{
    [AddComponentMenu("ZL/Server/Photon/Photon Server Manager (Singleton)")]

    [DisallowMultipleComponent]

    public sealed class PhotonServerManager : MonoBehaviourPunCallbacks, ISingleton<PhotonServerManager>
    {
        [Space]

        [SerializeField]

        private float fakeLoadingTime = 0f;

        [Space]

        [SerializeField]

        private UnityEvent eventOnConnectingToMaster;

        public UnityEvent EventOnConnectingToMaster => eventOnConnectedToMaster;

        [Space]

        [SerializeField]

        private UnityEvent eventOnConnectedToMaster;

        public UnityEvent EventOnConnectedToMaster => eventOnConnectedToMaster;

        [Space]

        [SerializeField]

        private UnityEvent eventOnDisconnected;

        public UnityEvent EventOnDisconnected => eventOnConnectedToMaster;

        private readonly Stopwatch loadingStopwatch = new();

        private void Awake()
        {
            ISingleton<PhotonServerManager>.TrySetInstance(this);
        }

        private void OnDestroy()
        {
            ISingleton<PhotonServerManager>.Release(this);
        }

        public void ConnectToMaster()
        {
            OnConnectingToMaster();

            loadingStopwatch.Restart();

            if (PhotonNetwork.IsConnected == false)
            {
                PhotonNetwork.GameVersion = Application.unityVersion;

                PhotonNetwork.ConnectUsingSettings();
            }

            else
            {
                OnConnectedToMaster();
            }
        }

        public void OnConnectingToMaster()
        {
            FixedDebug.Log("Connecting To Master...");

            eventOnConnectingToMaster.Invoke();
        }

        public override void OnConnectedToMaster()
        {
            loadingStopwatch.Stop();

            float loadingTime = (float)loadingStopwatch.Elapsed.TotalSeconds;

            StartCoroutine(FakeLoading(loadingTime, () =>
            {
                FixedDebug.Log("Connected To Master.");

                eventOnConnectedToMaster.Invoke();
            }));
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            loadingStopwatch.Stop();

            float loadingTime = (float)loadingStopwatch.Elapsed.TotalSeconds;

            StartCoroutine(FakeLoading(loadingTime, () =>
            {
                FixedDebug.LogWarning($"Disconnected: {cause}");

                eventOnDisconnected.Invoke();
            }));
        }

        private IEnumerator FakeLoading(float loadingTime, Action callback)
        {
            loadingTime = fakeLoadingTime - loadingTime;

            if (loadingTime > 0f)
            {
                yield return WaitFor.Seconds(loadingTime);
            }

            callback.Invoke();
        }
    }
}