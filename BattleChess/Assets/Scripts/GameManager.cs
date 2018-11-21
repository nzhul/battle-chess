using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[Serializable]
public enum Turn
{
    Human,
    AI
}

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

    Turn _currentTurn = Turn.Human;
    public Turn CurrentTurn { get { return _currentTurn; } }

    // has the user pressed start?
    bool _hasLevelStarted = false;
    public bool HasLevelStarted { get { return _hasLevelStarted; } set { _hasLevelStarted = value; } }

    // have we begun gamePlay?
    bool _isGamePlaying = false;
    public bool IsGamePlaying { get { return _isGamePlaying; } set { _isGamePlaying = value; } }

    // have we met the game over condition?
    bool _isGameOver = false;
    public bool IsGameOver { get { return _isGameOver; } set { _isGameOver = value; } }

    // have the end level graphics finished playing?
    bool _hasLevelFinished = false;
    public bool HasLevelFinished { get { return _hasLevelFinished; } set { _hasLevelFinished = value; } }

    // delay in between game stages
    public float delay = 1f;

    // events invoked for StartLevel/PlayLevel/EndLevel coroutines
    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;

    private void Start()
    {
        // TODO: null check
        StartCoroutine(RunGameLoop());
    }

    private IEnumerator RunGameLoop()
    {
        yield return StartCoroutine(StartLevelRoutine());
        yield return StartCoroutine(PlayLevelRoutine());
        yield return StartCoroutine(EndLevelRoutine());
    }

    IEnumerator StartLevelRoutine()
    {
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }

        PlayerManager.Instance.InputEnabled = false;

        while (!_hasLevelStarted)
        {
            yield return null;
        }

        // trigger events when we press the StartButton
        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }


    IEnumerator PlayLevelRoutine()
    {
        _isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        PlayerManager.Instance.InputEnabled = true;

        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }

        while (!_isGameOver)
        {
            yield return null;

            _isGameOver = IsWinner();
        }
    }

    private bool IsWinner()
    {
        // TODO: do a check here whether human or AI has 0 pieces or 
        // _droneReachedFinal = true;
        return false;
    }

    IEnumerator EndLevelRoutine()
    {
        Debug.Log("END LEVEL");
        PlayerManager.Instance.InputEnabled = false;

        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }

        while (!_hasLevelFinished)
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
        _hasLevelStarted = true;
    }

    public void UpdateTurn()
    {
        if (_currentTurn == Turn.Human)
        {
            if (PlayerManager.Instance.IsTurnComplete && !EnemyManager.Instance.AreEnemiesAllDead())
            {
                PlayEnemyTurn();
            }

        }
        else if (_currentTurn == Turn.AI)
        {
            if (EnemyManager.Instance.IsEnemyTurnComplete())
            {
                PlayPlayerTurn();
            }
        }
    }

    private void PlayPlayerTurn()
    {
        _currentTurn = Turn.Human;
        PlayerManager.Instance.StartHumanTurn();
    }

    private void PlayEnemyTurn()
    {
        _currentTurn = Turn.AI;
        PlayerManager.Instance.InputEnabled = false;
        EnemyManager.Instance.StartAITurn();
    }
}
