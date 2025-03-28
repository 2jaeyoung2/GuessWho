using DG.Tweening;

using DG.Tweening.Plugins.Options;

using UnityEngine;

namespace ZL.Unity.Tweeners
{
    [AddComponentMenu("ZL/Tweeners/Rotation Key Frame Tweener")]

    [RequireComponent(typeof(TransformRotationTweener))]

    public sealed class RotationKeyFrameTweener :
        
        KeyFrameTweener<TransformRotationTweener, QuaternionTweener, Quaternion, Vector3, QuaternionOptions>
    {
        [Space]

        [SerializeField]

        private RotateMode rotateMode;

        public override void SetKeyFrame()
        {
            if (keyFrames.TryGetCurrent(out var result) == true)
            {
                transform.rotation = Quaternion.Euler(result);
            }
        }

        protected override void TweenKeyFrame()
        {
            if (keyFrames.TryGetCurrent(out var result) == true)
            {
                componentTweener.Tween(result).SetRotateMode(rotateMode);
            }
        }
    }
}