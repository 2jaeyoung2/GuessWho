using UnityEngine;

using UnityEngine.Audio;

#if UNITY_EDITOR

using UnityEditor;

#endif

using ZL.Unity.Collections;

using ZL.Unity.IO;

namespace ZL.Unity.Audio
{
    [AddComponentMenu("ZL/Audio/Audio Mixer Manager (Singleton)")]

    [DisallowMultipleComponent]

    public sealed class AudioMixerManager : MonoBehaviour, IMonoSingleton<AudioMixerManager>
    {
        [Space]

        [SerializeField]

        [UsingCustomProperty]

        [PropertyField]

        [Essential]

        [Button("LoadAudioMixerParameters", "Load Parameters")]

        private AudioMixer audioMixer = null;

        [SerializeField]

        [UsingCustomProperty]

        [PropertyField]

        [Button(nameof(LoadVolumes))]

        [Button(nameof(SaveVolumes))]

        private SerializableDictionary<string, float, FloatPrefs> parameters;

#if UNITY_EDITOR

        private void Reset()
        {
            LoadAudioMixerParameters();
        }

        private void OnValidate()
        {
            foreach (var parameter in parameters)
            {
                parameter.Value = Mathf.Clamp01(parameter.Value);

                if (Application.isPlaying == true)
                {
                    SetVolume(parameter.Key, parameter.Value);
                }
            }
        }

        public void LoadAudioMixerParameters()
        {
            parameters.Clear();

            if (audioMixer != null)
            {
                foreach (var audioMixerGroup in audioMixer.FindMatchingGroups(string.Empty))
                {
                    var key = audioMixerGroup.name;

                    audioMixer.GetFloat(key, out float value);

                    FloatPrefs volumePref = new(key, value);

                    volumePref.TryLoadValue();

                    parameters.Add(volumePref);
                }
            }

            EditorUtility.SetDirty(this);
        }

#endif

        private void Awake()
        {
            if (ISingleton<AudioMixerManager>.TrySetInstance(this) == true)
            {
                LoadVolumes();
            }
        }

        private void OnDestroy()
        {
            ISingleton<AudioMixerManager>.Release(this);
        }

        public void SaveVolumes()
        {
            foreach (var volumePref in parameters)
            {
                volumePref.SaveValue();
            }
        }

        public void LoadVolumes()
        {
            foreach (var volumePref in parameters)
            {
                volumePref.TryLoadValue();
            }
        }

        public float GetVolume(string key)
        {
            return parameters[key];
        }

        public void SetVolume(string key, float value)
        {
            parameters[key] = value;

            audioMixer.SetVolume(key, value);
        }
    }
}