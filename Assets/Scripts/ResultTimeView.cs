using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultTimeView : MonoBehaviour
{
    [SerializeField] private Text _timeText;

    // Start is called before the first frame update
    void Start()
    {
        _timeText.text = $"{TimeManager.Instance.Timer.ToString("0.00")}m";
    }
}
