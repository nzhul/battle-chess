﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private Faction _currentTurn = Faction.Human;

    public bool HasLevelFinished { get; set; }

    public bool HasLevelStarted { get; set; }

    public bool IsGameOver { get; private set; }

    public Faction Winner { get; set; }

    public string EndGameReason { get; set; }

    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;

    private void Start()
    {
        BoardManager.Instance.OnBoardInit += OnBoardInit; ;
        StartCoroutine(RunGameLoop());
    }

    private void OnBoardInit()
    {
        List<Piece> drones = EnemyManager.Instance.Pieces.Where(p => p.PieceType == typeof(Drone)).ToList();

        foreach (var drone in drones)
        {
            drone.OnTurnCompleted += CheckDroneVictoryCondition;
        }
    }

    private void CheckDroneVictoryCondition(Piece drone)
    {
        if (drone.CurrentY == 0)
        {
            string reason = string.Format("AI drone has reached human starting point at {0}:{1}", drone.CurrentX, drone.CurrentY);
            this.EndGame(Faction.AI, reason);
        }
    }

    private IEnumerator RunGameLoop()
    {
        yield return StartCoroutine(StartGameRoutine());
        yield return StartCoroutine(PlayGameRoutine());
        yield return StartCoroutine(EndGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }

        PlayerManager.Instance.InputEnabled = false;

        while (!this.HasLevelStarted)
        {
            yield return null;
        }

        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }


    IEnumerator PlayGameRoutine()
    {
        yield return new WaitForSeconds(1f);
        PlayerManager.Instance.InputEnabled = true;

        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }

        while (!this.IsGameOver)
        {
            yield return null;
        }
    }

    public void EndGame(Faction winner, string reason)
    {
        this.Winner = winner;
        this.IsGameOver = true;
        this.EndGameReason = reason;
    }

    IEnumerator EndGameRoutine()
    {
        PlayerManager.Instance.InputEnabled = false;

        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }

        while (!this.HasLevelFinished)
        {
            yield return null;
        }

        this.RestartLevel();
    }

    void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PlayLevel()
    {
        this.HasLevelStarted = true;
    }

    public void UpdateTurn()
    {
        if (_currentTurn == Faction.Human)
        {
            if (PlayerManager.Instance.IsTurnComplete)
            {
                PlayEnemyTurn();
            }

        }
        else if (_currentTurn == Faction.AI)
        {
            if (EnemyManager.Instance.IsEnemyTurnComplete())
            {
                PlayPlayerTurn();
            }
        }
    }

    private void PlayPlayerTurn()
    {
        _currentTurn = Faction.Human;
        PlayerManager.Instance.StartHumanTurn();
    }

    private void PlayEnemyTurn()
    {
        _currentTurn = Faction.AI;
        PlayerManager.Instance.InputEnabled = false;
        EnemyManager.Instance.StartAITurn();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}