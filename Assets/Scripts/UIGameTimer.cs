using System.Diagnostics;
using TMPro;
using UnityEngine;

public class UIGameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text m_label;

    private Stopwatch _timer;
    
    private void Start()
    {
        _timer = ServiceLocator.Instance.Game.Timer;
    }

    private void Update()
    {
        m_label.SetText(_timer.Elapsed.ToString("hh':'mm':'ss':'fff"));
    }
}
