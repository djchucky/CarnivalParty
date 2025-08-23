using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    private PlayerInputAction _input;
    private Vector2 _move;
    [SerializeField] private SimulatedScene _simulatedScene;

    [Header("Settings")]
    [SerializeField] private float _rotateSpeed = 100f;
    [SerializeField] private float _minValueX = -30f; // angle min X
    [SerializeField] private float _maxValueX = 30f;  // angle max X
    [SerializeField] private float _minValueY = -60f; // angle min Y
    [SerializeField] private float _maxValueY = 60f;  // angle max Y

    [Header("Prefab settings")]
    [SerializeField] private Ball _ball;
    [SerializeField] private GameObject _ballContainer;
    [SerializeField] [Range(1,20)] private float _power;
    [SerializeField] private Vector3 _offset;
    
    private float _rotationX;
    private float _rotationY;

    void Start()
    {
        _input = new PlayerInputAction();
        _input.Player.Enable();
        _input.Player.Fire.performed += Fire_performed;

        // Store initial rotation
        Vector3 startRot = transform.localEulerAngles;
        _rotationX = startRot.x;
        _rotationY = startRot.y;
    }

    private void Fire_performed(InputAction.CallbackContext obj)
    {
        //Throw ball
        var ballClone = Instantiate(_ball,transform.position,Quaternion.identity);
        ballClone.transform.parent = _ballContainer.transform;
        ballClone.Init(transform.forward * _power);
    }

    void Update()
    {
        Movement();    
    }

    private void FixedUpdate()
    {
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
}
