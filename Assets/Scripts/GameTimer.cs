using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text m_label;

    private Stopwatch m_stopwatch = new Stopwatch();
    
    public void ResetTimer()
    {
        m_stopwatch.Stop();
        m_stopwatch.Reset();
    }

    public void StartTimer()
    {
        m_stopwatch.Start();
    }

    public void StopTimer()
    {
        m_stopwatch.Stop();
    }

    private void Start()
    {
        ServiceLocator.Instance.Game.OnGameStartEvent += OnGameStarted;
        ServiceLocator.Instance.Game.OnGameEndEvent += OnGameEnded;
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.Game.OnGameStartEvent -= OnGameStarted;
        ServiceLocator.Instance.Game.OnGameEndEvent -= OnGameEnded;
    }

    private void OnGameStarted()
    {
        ResetTimer();
        StartTimer();
    }

    private void OnGameEnded()
    {
        StopTimer();
    }

    private void Update()
    {
        m_label.SetText(m_stopwatch.Elapsed.ToString("hh':'mm':'ss':'fff"));
    }
}
