using UnityEngine;

namespace ZL.Unity
{
    [AddComponentMenu("ZL/Skybox Rotator")]

    [DisallowMultipleComponent]

    public sealed class SkyboxRotator : MonoBehaviour
    {
        [Space]

        [SerializeField]

        private float speed = 1f;

        private void Update()
        {
            RenderSettings.skybox.SetFloat("_Rotation", speed * Time.time);
        }
    }
}