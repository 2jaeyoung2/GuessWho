using DG.Tweening;

using DG.Tweening.Core;

using DG.Tweening.Plugins.Options;

using UnityEngine;

using UnityEngine.Events;

namespace ZL.Unity.Tweeners
{
    public abstract class ValueTweener<T1, T2, TPlugOptions>

        where TPlugOptions : struct, IPlugOptions
    {
        [SerializeField]

        private float duration = 0f;

        public float Duration
        {
            get => duration;

            set => duration = value;
        }

        [SerializeField]

        private float delay = 0f;

        public float Delay
        {
            get => delay;

            set => delay = value;
        }

        [SerializeField]

        private Ease ease = Ease.Unset;

        public Ease Ease
        {
            get => ease;

            set => ease = value;
        }

        [SerializeField]

        private bool isIndependentUpdate = true;

        public bool IsIndependentUpdate
        {
            get => isIndependentUpdate;

            set => isIndependentUpdate = value;
        }

        [SerializeField]

        private bool loop = false;

        public bool Loop
        {
            get => loop;

            set => loop = value;
        }

        [SerializeField]

        [UsingCustomProperty]

        [ToggleIf("loop", false)]

        [AddIndent]

        [Alias("Count")]

        private int loopCount = -1;

        public int LoopCount
        {
            get => loopCount;

            set => loopCount = value;
        }

        [SerializeField]

        [UsingCustomProperty]

        [ToggleIf("loop", false)]

        [AddIndent]

        [Alias("Type")]

        [PropertyField]

        private LoopType loopType = LoopType.Restart;

        public LoopType LoopType
        {
            get => loopType;

            set => loopType = value;
        }

        protected DOGetter<T1> getter = null;

        public DOGetter<T1> Getter
        {
            get => getter;

            set => getter = value;
        }

        protected DOSetter<T1> setter = null;

        public DOSetter<T1> Setter
        {
            get => setter;

            set => setter = value;
        }

        [SerializeField]

        private UnityEvent eventOnStart = null;

        [SerializeField]

        private UnityEvent eventOnComplete = null;

        public UnityEvent EventOnComplete
        {
            get => eventOnComplete;

            set => eventOnComplete = value;
        }

        public TweenerCore<T1, T2, TPlugOptions> Current { get; private set; }

        protected abstract TweenerCore<T1, T2, TPlugOptions> To

            (DOGetter<T1> getter, DOSetter<T1> setter, T2 endValue, float duration);

        public TweenerCore<T1, T2, TPlugOptions> Tween(T2 endValue)
        {
            return Tween(endValue, duration);
        }

        public TweenerCore<T1, T2, TPlugOptions> Tween(T2 endValue, float duration)
        {
            Current.Kill();

            Current = To(getter, setter, endValue, duration);

            Current.SetDelay(delay);

            Current.SetEase(ease);

            if (isIndependentUpdate == true)
            {
                Current.SetUpdate(isIndependentUpdate);
            }

            if (loop == true)
            {
                Current.SetLoops(loopCount, loopType);
            }

            if (eventOnStart != null)
            {
                Current.OnStart(eventOnStart.Invoke);
            }

            if (eventOnComplete != null)
            {
                Current.OnComplete(eventOnComplete.Invoke);
            }

            Current.SetAutoKill(false);

            Current.SetRecyclable(true);

            Current.Restart();

            return Current;
        }
    }
}