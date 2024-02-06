using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Text _timerText;

    private float _timer = 0;
    private bool _isDuringMeasurement = false;
    
    public static TimeManager Instance { get; private set; }
    public float Timer => _timer;
    public bool SetIsDuringMeasurement(bool value) => _isDuringMeasurement = value;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;
        _isDuringMeasurement = true;
    }
    
    private void FixedUpdate()
    {
        if (_isDuringMeasurement)
        {
            _timer += Time.deltaTime;
            _timerText.text = $"{_timer.ToString("0000.00")}m";
        }
    }
}