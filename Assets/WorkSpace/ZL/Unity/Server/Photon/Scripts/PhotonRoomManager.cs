using Photon.Pun;

using Photon.Realtime;

using UnityEngine;

using UnityEngine.Events;

namespace ZL.Unity.Server.Photon
{
    [AddComponentMenu("ZL/Server/Photon/Photon Room Manager (Singleton)")]

    [DisallowMultipleComponent]

    public sealed class PhotonRoomManager : MonoBehaviourPunCallbacks, ISingleton<PhotonRoomManager>
    {
        [Space]

        [SerializeField]

        [UsingCustomProperty]

        [ReadOnly(true)]

        private string currentRoomName = string.Empty;

        [Space]

        [SerializeField]

        private UnityEvent eventOnCreatedRoom;

        [Space]

        [SerializeField]

        private UnityEvent eventOnCreateRoomFailed;

        [Space]

        [SerializeField]

        private UnityEvent eventOnJoinedRoom;

        [Space]

        [SerializeField]

        private UnityEvent evenOnJoinRoomFailed;
        
        [Space]

        [SerializeField]

        private UnityEvent eventOnLeftRoom;

        [Space]

        [SerializeField]

        private UnityEvent eventOnMasterClientLeftRoom;

        [Space]

        [SerializeField]

        private UnityEvent eventOnMasterClientSwitched;

        private void Awake()
        {
            ISingleton<PhotonRoomManager>.TrySetInstance(this);
        }

        private void OnDestroy()
        {
            ISingleton<PhotonRoomManager>.Release(this);
        }

        public void CreateRoom(string roomName)
        {
            currentRoomName = roomName;

            PhotonNetwork.CreateRoom(roomName, null);
        }

        public void CreateRoom(string roomName, RoomOptions roomOptions)
        {
            currentRoomName = roomName;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public override void OnCreatedRoom()
        {
            FixedDebug.Log($"Created room: {currentRoomName}");

            eventOnCreatedRoom.Invoke();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            FixedDebug.Log($"Create room failed ({returnCode}): {message}");

            eventOnCreateRoomFailed.Invoke();
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void JoinRandomOrCreateRoom()
        {
            PhotonNetwork.JoinRandomOrCreateRoom();
        }

        public override void OnJoinedRoom()
        {
            currentRoomName = PhotonNetwork.CurrentRoom.Name;

            FixedDebug.Log($"Joined room: {currentRoomName}");

            eventOnJoinedRoom.Invoke();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            FixedDebug.Log($"Join room failed ({returnCode}): {message}");

            evenOnJoinRoomFailed.Invoke();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            FixedDebug.Log($"Join random failed ({returnCode}): {message}");

            evenOnJoinRoomFailed.Invoke();
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            FixedDebug.Log($"Left room: {currentRoomName}");

            if (PhotonNetwork.IsMasterClient == true)
            {
                OnMasterClientLeftRoom();
            }

            eventOnLeftRoom.Invoke();
        }

        public void OnMasterClientLeftRoom()
        {
            ISingleton<PhotonManager>.Instance.RPCLog(RpcTarget.All, "Master client left room.");

            eventOnMasterClientLeftRoom.Invoke();
        }

        public void SetMasterClientInOrder()
        {
            var playerList = PhotonNetwork.PlayerList;

            foreach (var player in playerList)
            {
                if (player.IsMasterClient == false)
                {
                    PhotonNetwork.SetMasterClient(player);
                }
            }
        }

        public void SetMasterClientRandom()
        {
            var playerList = PhotonNetwork.PlayerList;

            if (playerList.Length != 0)
            {
                var newMasterClient = playerList[Random.Range(0, playerList.Length)];

                PhotonNetwork.SetMasterClient(newMasterClient);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            FixedDebug.Log($"Master client switched: {newMasterClient.NickName}");

            eventOnMasterClientSwitched.Invoke();
        }
    }
}