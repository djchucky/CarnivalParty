using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if(_rb == null)
        {
            Debug.LogError("Rigidbody is NULL on ball");
        }
        
        _rb.useGravity = false;
    }

    public void Init(Vector3 velocity)
    {
        _rb.useGravity = enabled;
        _rb.AddForce(velocity, ForceMode.Impulse);
    }

}
