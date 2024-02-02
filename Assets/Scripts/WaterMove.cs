using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMove : MonoBehaviour
{
    [SerializeField] private Transform _waterPosition;
    [SerializeField] private Vector3 _moveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _waterPosition.position -= _moveSpeed;
    }
}
