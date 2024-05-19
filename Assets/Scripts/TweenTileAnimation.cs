using System;
using DG.Tweening;
using UnityEngine;

public class TweenTileAnimation : MonoBehaviour
{
    private void Start()
    {
        transform.DOPunchScale(Vector3.right, 0.5f, 10, 1f);
        transform.DOPunchPosition(Vector3.right * 40, 0.5f, 10, 0.2f);
    }
}
