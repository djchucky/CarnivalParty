using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    private PlayerInputAction _input;

    private Vector2 _move;

    private bool _canMove = true;
    private bool _isCharging = true;

    private float _rotationX;
    private float _rotationY;

    public static Action<float,float,float> OnChargeBar;
    [SerializeField] private SimulatedScene _simulatedScene;

    [Header("Settings")]
    [SerializeField] private float _rotateSpeed = 100f;
    [SerializeField] private float _minValueX = -30f; // angle min X
    [SerializeField] private float _maxValueX = 30f;  // angle max X
    [SerializeField] private float _minValueY = -60f; // angle min Y
    [SerializeField] private float _maxValueY = 60f;  // angle max Y

    [Header("Fire Settings")]
    private float _nextFire = 0f;
    [SerializeField] private float _fireRate = 1.5f;
    private const int _minPower = 100;
    private const int _maxPower = 150;
    [SerializeField] private float _chargeSpeed = 5f;
    [SerializeField] [Range(_minPower,_maxPower)] private float _power;

    [Header("Prefab settings")]
    [SerializeField] private Ball _ball;
    [SerializeField] private GameObject _ballContainer;


    private void OnEnable()
    {
        GameManager.OnGameFinish += DisablePlayer;
    }

    private void OnDisable()
    {
        _input.Player.Fire.performed -= Fire_performed;
        GameManager.OnGameFinish -= DisablePlayer;
    }

    void Start()
    {
        _input = new PlayerInputAction();
        _input.Player.Enable();
        _input.Player.Fire.performed += Fire_performed;
        StartCoroutine(ChargeBarRoutine());
        // Store initial rotation
        Vector3 startRot = transform.localEulerAngles;
        _rotationX = startRot.x;
        _rotationY = startRot.y;
    }

    private void Fire_performed(InputAction.CallbackContext obj)
    {
        //Throw ball
        if (!_canMove) return;

        //Check if can Fire
        if(Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            GameObject ballClone = PoolManager.Instance.RequestBall();
            Vector3 shootPos = transform.position;
            Vector3 shootDirection = transform.forward * _power;
            //var ballClone = Instantiate(_ball,transform.position,Quaternion.identity);
            //ballClone.transform.parent = _ballContainer.transform;
            ballClone.GetComponent<Ball>().Init(shootDirection,shootPos);
        }
    }

    void Update()
    {
        if (!_canMove) return;
        Movement();    
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        _simulatedScene.SimulateTrajectory(_ball, transform.position, transform.forward * _power);
    }

    private void Movement()
    {
        _move = _input.Player.Movement.ReadValue<Vector2>();

        // Update Angles based on Inputs
        _rotationX -= _move.y * _rotateSpeed * Time.deltaTime; // su/giù
        _rotationY += _move.x * _rotateSpeed * Time.deltaTime; // sx/dx

        // Clamp the valuees
        _rotationX = Mathf.Clamp(_rotationX, _minValueX, _maxValueX);
        _rotationY = Mathf.Clamp(_rotationY, _minValueY, _maxValueY);

        // Apply the rotation
        transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
    }

    private void DisablePlayer()
    {
        _canMove = false;
    }

    IEnumerator ChargeBarRoutine()
    {
        while(_canMove)
        {
            while(_isCharging)
            {
                _power += (1.0f/_chargeSpeed) * Time.deltaTime;
                if(_power >= _maxPower)
                {
                    _isCharging = false;
                    break;
                }
                OnChargeBar?.Invoke(_power,_minPower,_maxPower);
                yield return null;
            }

            while(!_isCharging)
            {
                _power -= (1.0f/ _chargeSpeed) * Time.deltaTime;
                if (_power <= _minPower)
                {
                    _power = _minPower;
                    _isCharging = true;
                    break;
                }
                OnChargeBar?.Invoke(_power, _minPower, _maxPower);
                yield return null;
            }
        }
    }

}
