using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null )
            {
                Debug.Log("Instance is null on Game Manager");
            }
            return _instance;
        }
    }

    //Variables 
    private bool _isGameOver;
    private Coroutine _timerRoutine;
    public static Action OnGameFinish;

    [Header("Game Settings")]
    [SerializeField] private float _timer;
    [SerializeField] private int _currentScore;
    [SerializeField] private int _currentHitTarget;
    [SerializeField] private int _totalTargets;

    [SerializeField] private List<Target> _targets = new List<Target>();

    //Subscribe to Target Event 
    private void OnEnable()
    {
        Target.OnHit += Target_OnHit;
    }

    private void OnDisable()
    {
        Target.OnHit -= Target_OnHit;    
    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    void Start()
    {
        //Set number of Targets in Scene and Update UI
        StartCoroutine(TargetsRoutine());
        
    }

    private void FindInitialTargets()
    {
        //Get Current Active scene
        Scene currentScene = SceneManager.GetActiveScene();

        //Get all Objects in Scene
        GameObject[] objectsInScene = currentScene.GetRootGameObjects();

        foreach (GameObject obj in objectsInScene)
        {
            // Prende tutti i Target nei figli (anche se disattivi)
            Target[] targetsInChildren = obj.GetComponentsInChildren<Target>(true);
            foreach (Target t in targetsInChildren)
            {
                _targets.Add(t);
            }
        }

        _totalTargets = _targets.Count;
    }

    public void AddScore(int targetPoint)
    {
        _currentScore += targetPoint;
    }

    private void UpdateTargetLeft()
    {
        _currentHitTarget++;
    }

    //Subscribed to delegate OnHit (Target)
    private void Target_OnHit(int targetPoint)
    {
        UpdateTargetLeft();
        AddScore(targetPoint);
        CheckScore();
        UIManager.Instance.UpdateScore(_currentScore);
        UIManager.Instance.UpdateTargets(_currentHitTarget, _totalTargets);
    }

    private void CheckScore()
    {
        if(_currentHitTarget == _totalTargets)
        {
            _isGameOver = true;
            UIManager.Instance.WinScreen();
            OnGameFinish?.Invoke();
            Debug.Log("You Win");
        }
    }

    IEnumerator TimerRoutine()
    {
        while (!_isGameOver)
        {
            _timer -= Time.deltaTime;
            _timer = (float) Math.Round(_timer,2);

            UIManager.Instance.UpdateTimer(_timer);
            if(_timer <= 0)
            {
                _isGameOver = true;
                UIManager.Instance.GameOverScreen();
                OnGameFinish?.Invoke();
                Debug.Log("You Lose");
                yield break;
            }

            yield return null;
        }  
        _timerRoutine = null;
    }

    IEnumerator TargetsRoutine()
    {
        yield return null;
        FindInitialTargets();
        UIManager.Instance.UpdateScore(_currentScore);
        UIManager.Instance.UpdateTargets(_currentHitTarget, _totalTargets);

        if (_timerRoutine == null)
            _timerRoutine = StartCoroutine(TimerRoutine());
    }
}
