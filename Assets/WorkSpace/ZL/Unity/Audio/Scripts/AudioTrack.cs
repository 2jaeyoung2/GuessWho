using UnityEngine;

namespace ZL.Unity.Audio
{
    [AddComponentMenu("ZL/Audio/Audio Track (Singleton)")]

    [DisallowMultipleComponent]

    [RequireComponent(typeof(AudioSource))]

    public sealed class AudioTrack : MonoBehaviour, IMonoSingleton<AudioTrack>
    {
        [Space]

        [SerializeField]

        [UsingCustomProperty]

        [ReadOnly(true)]
        
        [GetComponent]

        private AudioSource audioSource;

        [Space]

        [SerializeField]

        private string trackName = string.Empty;

        [Space]

        [SerializeField]

        private bool playOnAwake = true;

        [SerializeField]

        [UsingCustomProperty]

        [PropertyField]

        [Button(nameof(Play))]

        [Button(nameof(Pause))]

        [Button(nameof(Resume))]

        private AudioTrackPlayMode playMode = AudioTrackPlayMode.RepeatOne;

        public AudioTrackPlayMode PlayMode { set => playMode = value; }

        [Space]

        [SerializeField]

        [UsingCustomProperty]

        [ToggleIf("playMode", AudioTrackPlayMode.Shuffle, true)]

        private int playlistIndex = 0;

        [SerializeField]

        private AudioClip[] playlist;

        private bool isLooping = false;

#if UNITY_EDITOR

        [HideInInspector]

        public bool isPlayModeShuffle;

        private void OnValidate()
        {
            isPlayModeShuffle = playMode == AudioTrackPlayMode.Shuffle;
        }

#endif

        private void Awake()
        {
            ISingleton<AudioTrack>.TrySetInstance(this);
        }

        private void Start()
        {
            if (playOnAwake == true)
            {
                PlayLoop();
            }
        }

        private void Update()
        {
            if (isLooping == true && audioSource.isPlaying == false)
            {
                Play();
            }
        }

        private void OnDestroy()
        {
            ISingleton<AudioTrack>.Release(this);
        }

        bool ISingleton<AudioTrack>.IsDuplicated()
        {
            if (ISingleton<AudioTrack>.Instance != null)
            {
                if (ISingleton<AudioTrack>.Instance.trackName == trackName)
                {
                    return true;
                }

                DestroyImmediate(ISingleton<AudioTrack>.Instance.gameObject);
            }

            return false;
        }

        public void PlayLoop(int index)
        {
            playlistIndex = index;

            PlayLoop();
        }

        public void PlayLoop()
        {
            isLooping = true;
        }

        public void Pause()
        {
            isLooping = false;

            audioSource.Pause();
        }

        public void Resume()
        {
            isLooping = true;

            audioSource.Play();
        }

        public void Play()
        {
            switch (playMode)
            {
                case AudioTrackPlayMode.RepeatAll:

                    ++playlistIndex;

                    if (playlistIndex > playlist.Length - 1)
                    {
                        playlistIndex = 0;
                    }

                    break;

                case AudioTrackPlayMode.Reverse:

                    --playlistIndex;

                    if (playlistIndex < 0)
                    {
                        playlistIndex = playlist.Length - 1;
                    }

                    break;

                case AudioTrackPlayMode.Shuffle:

                    playlistIndex = Random.Range(0, playlist.Length);

                    break;
            }

            audioSource.clip = playlist[playlistIndex];

            audioSource.Play();
        }

        public enum AudioTrackPlayMode
        {
            RepeatOne,

            RepeatAll,

            Reverse,

            Shuffle,
        }
    }
}