using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Text _timerText;

    private float _timer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        _timerText.text = $"{_timer.ToString("00000.00")}m";
    }
}
