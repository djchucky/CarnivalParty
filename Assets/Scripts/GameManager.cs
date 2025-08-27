using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private Target[] _arrayTarget;
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
        FindInitialTargets();
        UIManager.Instance.UpdateScore(_currentScore);
        UIManager.Instance.UpdateTargets(_currentHitTarget, _totalTargets);
        
        if(_timerRoutine == null )
        _timerRoutine = StartCoroutine(TimerRoutine());
    }

    private void FindInitialTargets()
    {
        _arrayTarget = FindObjectsByType<Target>(FindObjectsSortMode.InstanceID);
            
        foreach (Target t in _arrayTarget)
        {
            _targets.Add(t);
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
}
