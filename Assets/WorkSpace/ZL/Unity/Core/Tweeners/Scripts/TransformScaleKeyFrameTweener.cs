using DG.Tweening.Plugins.Options;

using UnityEngine;

namespace ZL.Unity.Tweeners
{
    [AddComponentMenu("ZL/Tweeners/Transform Scale Key Frame Tweener")]

    [RequireComponent(typeof(TransformScaleTweener))]

    public sealed class TransformScaleKeyFrameTweener :
        
        KeyFrameTweener<TransformScaleTweener, Vector3Tweener, Vector3, Vector3, VectorOptions>
    {
        public override void SetKeyFrame()
        {
            if (keyFrames.TryGetCurrent(out var result) == true)
            {
                transform.localScale = result;
            }
        }
    }
}