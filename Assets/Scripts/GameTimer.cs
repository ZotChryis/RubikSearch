using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text m_label;

    private TimeSpan m_runningTime = TimeSpan.Zero;
    private bool m_running = false;
    
    public void ResetTimer()
    {
        m_runningTime = TimeSpan.Zero;
        m_running = false;
    }

    public void StartTimer()
    {
        m_running = true;
    }

    public void StopTimer()
    {
        m_running = false;
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
        if (m_running)
        {
            m_runningTime += TimeSpan.FromSeconds(Time.deltaTime);
        }

        m_label.SetText(m_runningTime.ToString());
    }
}
