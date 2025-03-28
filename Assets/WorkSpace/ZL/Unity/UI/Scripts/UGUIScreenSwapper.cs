using UnityEngine;

namespace ZL.Unity.UI
{
    [AddComponentMenu("ZL/UI/UGUI Screen Swapper")]

    [DisallowMultipleComponent]

    public class UGUIScreenSwapper : MonoBehaviour
    {
        [Space]

        [SerializeField]

        protected UGUIScreen main;

        public UGUIScreen Current { get; set; } = null;

        public UGUIScreen Last { get; set; } = null;

        public void SetCurrent(UGUIScreen newCurrent)
        {
            Current?.SetFaded(false);

            Current = newCurrent;

            Last = newCurrent;

            newCurrent.transform.SetAsLastSibling();
        }

        public virtual void OpenMainScreen()
        {
            main?.SetFaded(true);
        }

        public virtual void CloseCurrentScreen()
        {
            Current?.SetFaded(false);
        }

        public virtual void OpenLastScreen()
        {
            Last?.SetFaded(true);
        }
    }
}