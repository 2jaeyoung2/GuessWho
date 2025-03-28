using System.Collections.Generic;

using System;

using UnityEngine;

namespace ZL.Unity.Collections
{
    [Serializable]

    public sealed class LoopList<T>
    {
        [SerializeField]

        private int index = 0;

        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        [SerializeField]

        private LoopPattern loopType = LoopPattern.Clamp;

        [Space]

        [SerializeField]

        private List<T> list = new();

        public int Count => list.Count;

        public bool TryGetCurrent(out T result)
        {
            if (list.Count == 0)
            {
                result = default;

                return false;
            }

            result = list[index.Loop(0, Count - 1, loopType)];

            return true;
        }
    }
}