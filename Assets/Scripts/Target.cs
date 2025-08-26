using UnityEngine;
using System;

public class Target : MonoBehaviour
{
    public static event Action<int> OnHit;

    [SerializeField] private int _targetPoints = 10;
    private bool _hasHit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Untagged"))
        {
            Debug.Log("Collided with: " + collision.collider.name);
            GetComponent<Renderer>().material.color = Color.red;
            
            if(OnHit != null && !_hasHit)
            {
                _hasHit = true;
                OnHit?.Invoke(_targetPoints);
            }
        }
    }
}
