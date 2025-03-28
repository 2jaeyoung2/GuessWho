using DG.Tweening;

using UnityEngine;

using UnityEngine.Events;

using ZL.Unity.Tweeners;

namespace ZL.Unity.UI
{
    [AddComponentMenu("ZL/UI/UGUI Screen")]

    [DisallowMultipleComponent]

    [RequireComponent(typeof(CanvasGroupAlphaTweener))]

    public class UGUIScreen : MonoBehaviour
    {
        [SerializeField]

        [UsingCustomProperty]

        [GetComponent]

        [Toggle(true)]

        private CanvasGroup canvasGroup;

        [SerializeField]

        [UsingCustomProperty]

        [GetComponent]

        [Toggle(true)]

        private CanvasGroupAlphaTweener alphaTweener;

        [SerializeField]

        [UsingCustomProperty]

        [GetComponent]

        [Toggle(true)]

        private UGUIScreenSwapper swapper;

        [Space]

        [SerializeField]

        [UsingCustomProperty]

        [PropertyField]

        [ReadOnlyWhenEditMode]

        [Button(nameof(ToggleFaded))]

        private bool isFadedIn = false;

        public bool IsFadedIn
        {
            get => isFadedIn;

            set
            {
                isFadedIn = value;

                if (isFadedIn == true)
                {
                    gameObject.SetActive(true);

                    canvasGroup.alpha = 1f;
                }

                else
                {
                    canvasGroup.alpha = 0f;

                    gameObject.SetActive(false);
                }
            }
        }

        [Space]

        [SerializeField]

        private UnityEvent eventOnFadingIn;

        [SerializeField]

        private UnityEvent eventOnFadedIn;

        [SerializeField]

        private UnityEvent eventOnFadingOut;

        [SerializeField]

        private UnityEvent eventOnFadedOut;

        private void Awake()
        {
            IsFadedIn = isFadedIn;
        }

        public void ToggleFaded()
        {
            SetFaded(!isFadedIn);
        }

        public void SetFaded(bool value)
        {
            SetFaded(value, alphaTweener.Tweener.Duration);
        }

        public void SetFaded(bool value, float duration)
        {
            isFadedIn = value;

            if (isFadedIn == true)
            {
                OnFadingIn();

                alphaTweener.Tween(1f, duration).OnComplete(OnFadedIn);
            }

            else
            {
                OnFadingOut();

                alphaTweener.Tween(0f, duration).OnComplete(OnFadedOut);
            }
        }

        protected virtual void OnFadingIn()
        {
            if (swapper != null)
            {
                swapper.SetCurrent(this);
            }

            gameObject.SetActive(true);

            eventOnFadingIn.Invoke();
        }

        protected virtual void OnFadedIn()
        {
            eventOnFadedIn.Invoke();
        }

        protected virtual void OnFadingOut()
        {
            if (swapper != null)
            {
                swapper.Current = null;
            }

            eventOnFadingOut.Invoke();
        }

        protected virtual void OnFadedOut()
        {
            eventOnFadedOut.Invoke();

            gameObject.SetActive(false);
        }
    }
}