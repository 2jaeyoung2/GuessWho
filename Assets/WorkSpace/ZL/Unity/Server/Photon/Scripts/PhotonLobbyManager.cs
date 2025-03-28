using Photon.Pun;

using Photon.Realtime;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Events;

using ZL.Unity.Collections;

namespace ZL.Unity.Server.Photon
{
    [AddComponentMenu("ZL/Server/Photon/Photon Lobby Manager (Singleton)")]

    [DisallowMultipleComponent]

    public sealed class PhotonLobbyManager : MonoBehaviourPunCallbacks, ISingleton<PhotonLobbyManager>
    {
        [Space]

        [SerializeField]

        [UsingCustomProperty]

        [ReadOnly(true)]

        private string currentLobbyName = string.Empty;

        [Space]

        [SerializeField]

        [UsingCustomProperty]

        [ReadOnlyWhenPlayMode]

        private Wrapper<TypedLobby[]> lobbies;

        [Space]

        [SerializeField]

        [UsingCustomProperty]

        [ReadOnly(true)]

        private Wrapper<List<RoomInfo>> roomList;

        public List<RoomInfo> RoomList
        {
            get => roomList.value;

            private set => roomList.value = value;
        }

        [Space]

        [SerializeField]

        private UnityEvent eventOnJoinedLobby;

        [Space]

        [SerializeField]

        private UnityEvent evenOnLeftLobby;

        [Space]

        [SerializeField]

        private UnityEvent eventOnRoomListUpdate;

        private Dictionary<string, TypedLobby> lobbyDictionary;

        private void Awake()
        {
            ISingleton<PhotonLobbyManager>.TrySetInstance(this);

            if (lobbies.value.Length != 0)
            {
                lobbyDictionary = new(lobbies.value.Length);

                foreach (var lobby in lobbies.value)
                {
                    lobbyDictionary.Add(lobby.Name, lobby);
                }
            }
        }

        private void OnDestroy()
        {
            ISingleton<PhotonLobbyManager>.Release(this);
        }

        public void JoinLobby(string name)
        {
            currentLobbyName = name;

            PhotonNetwork.JoinLobby(lobbyDictionary[name]);
        }

        public override void OnJoinedLobby()
        {
            FixedDebug.Log($"Joined lobby: {currentLobbyName}");

            eventOnJoinedLobby.Invoke();
        }

        public void LeaveLobby()
        {
            PhotonNetwork.LeaveLobby();
        }

        public override void OnLeftLobby()
        {
            FixedDebug.Log($"Left lobby: {currentLobbyName}");

            currentLobbyName = string.Empty;

            evenOnLeftLobby.Invoke();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            FixedDebug.Log($"Room list update.");

            RoomList = roomList;

            eventOnRoomListUpdate.Invoke();
        }
    }
}