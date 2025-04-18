using Photon.Pun;

using System.Collections;

using UnityEngine;

using ZL.Unity.Server.Photon;

namespace ZL.Unity.GuessWho
{
    [AddComponentMenu("ZL/Guess Who/Ingame SceneDirector (Singleton)")]

    public sealed class IngameSceneDirector : PhotonSceneDirector<IngameSceneDirector>
    {
        public bool IsReady { get; set; } = false;

        protected override IEnumerator Start()
        {
            if (PhotonNetwork.IsConnected == true)
            {
                IsReady = true;
            }

            else
            {
                ISingleton<PhotonServerManager>.Instance.ConnectToMaster();
            }

            while (IsReady == false)
            {
                yield return null;
            }

            ISingleton<NPCManager>.Instance.InitialSetBySpawnPoint();

            ISingleton<PlayerSpawnManager>.Instance.SpawnRandom();

            FadeIn();
        }
    }
}