using UnityEngine;

/// <summary>アウトラインのオンオフを切り替えます(テスト用)</summary>
public class TestOutLineController : MonoBehaviour
{
    [SerializeField] private Material _material;

    [SerializeField] private bool _enableOutLine;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _enableOutLine = !_enableOutLine;
            EnableOutLine();
        }
    }

    public void EnableOutLine()
    {
        if (_enableOutLine)
        {
            _material.EnableKeyword("_OUTLINE");
        }
        else
        {
            _material.DisableKeyword("_OUTLINE");
        }
    }
}