using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;
    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Instance is null on POOL MANAGER");
            }
            return _instance;
        }
    }

    //Variables
    [Header("Pool settings")]
    [SerializeField] private int _initialAmount;
    [SerializeField] private GameObject _ball;
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private GameObject _poolContainer;
    [SerializeField] private List<GameObject> _ballsList = new List<GameObject>();

    

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        GenerateBalls(_initialAmount);
    }

    private List<GameObject> GenerateBalls(int amountOfBalls)
    {
        //Generate amount of balls and add it them to the list

        for (int i = 0; i < amountOfBalls; i++)
        {
            GameObject ballClone = Instantiate(_ball, _spawnPos.position, Quaternion.identity);
            ballClone.transform.parent = _poolContainer.transform;
            ballClone.SetActive(false);
            _ballsList.Add(ballClone);
        }

        return null;
    }

    public GameObject RequestBall()
    {
        for(int i = 0; i < _ballsList.Count;i++)
        {
            if (_ballsList[i].activeInHierarchy == false)
            {
                _ballsList[i].SetActive(true);
                return _ballsList[i];
            }
        }

        GameObject newBall = Instantiate(_ball, _spawnPos.position, Quaternion.identity);
        newBall.transform.parent = _poolContainer.transform;
        _ballsList.Add(newBall);

        return newBall;
    }

    public Transform ReturnPosition()
    {
        return _spawnPos;
    }
}
