using UnityEngine;
using UnityEngine.UIElements;

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

    public void Init(Vector3 velocity, Vector3 position)
    {
        transform.position = position; // Setta la posizione al momento del lancio
        _rb.linearVelocity = Vector3.zero;   // Reset forza precedente (importante nel pooling!)
        _rb.useGravity = enabled;
        _rb.AddForce(velocity, ForceMode.Impulse);
    }

}
