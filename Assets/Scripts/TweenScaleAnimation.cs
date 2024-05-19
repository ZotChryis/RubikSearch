using System;
using DG.Tweening;
using UnityEngine;

public class TweenScaleAnimation : MonoBehaviour
{
    [Serializable]
    private struct AnimateParams
    {
        public bool Animate;
        public Vector3 EndValue;
        public float Time;
        public Ease Ease;
        public int Loops;
        public LoopType LoopType;
    }

    [SerializeField] private AnimateParams Scale;

    private void Start()
    {
        if (Scale.Animate)
        {
            transform
                .DOScale(Scale.EndValue, Scale.Time)
                .SetEase(Scale.Ease)
                .SetLoops(Scale.Loops, Scale.LoopType);
        }
    }
}
