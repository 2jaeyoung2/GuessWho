using DG.Tweening;

using DG.Tweening.Core;

using DG.Tweening.Plugins.Options;

using UnityEngine;

namespace ZL.Unity.Tweeners
{
    public abstract class ComponentValueTweener<TValueTweener, T1, T2, TPlugOptions> : MonoBehaviour

        where TValueTweener : ValueTweener<T1, T2, TPlugOptions>

        where TPlugOptions : struct, IPlugOptions
    {
        [Space]

        [SerializeField]

        protected TValueTweener tweener;

        public TValueTweener Tweener => tweener;

        public float Duration
        {
            get => tweener.Duration;

            set => tweener.Duration = value;
        }

        public float Delay
        {
            get => tweener.Delay;

            set => tweener.Delay = value;
        }

        public Ease Ease
        {
            get => tweener.Ease;

            set => tweener.Ease = value;
        }

        public bool IsIndependentUpdate
        {
            get => tweener.IsIndependentUpdate;

            set => tweener.IsIndependentUpdate = value;
        }

        public bool Loop
        {
            get => tweener.Loop;

            set => tweener.Loop = value;
        }

        public int LoopCount
        {
            get => tweener.LoopCount;

            set => tweener.LoopCount = value;
        }

        public LoopType LoopType
        {
            get => tweener.LoopType;

            set => tweener.LoopType = value;
        }

        public TweenerCore<T1, T2, TPlugOptions> Current => tweener.Current;

        protected abstract T1 Value { get; set; }

        protected virtual void Awake()
        {
            tweener.Getter = () => Value;

            tweener.Setter = value => Value = value;
        }

        public void SetEase(int value)
        {
            Ease = (Ease)value;
        }

        public TweenerCore<T1, T2, TPlugOptions> Tween(T2 endValue)
        {
            return tweener.Tween(endValue);
        }

        public TweenerCore<T1, T2, TPlugOptions> Tween(T2 endValue, float duration)
        {
            return tweener.Tween(endValue, duration);
        }
    }
}