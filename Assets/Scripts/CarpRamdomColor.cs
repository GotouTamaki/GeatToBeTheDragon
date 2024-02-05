using UnityEngine;

public class CarpRamdomColor : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField, Range(0, 1.0f)] float _maxPatternOffsetX = 1;
    [SerializeField, Range(0, 1.0f)] float _maxPatternOffsetY = 1;
    [SerializeField] float _maxPatternScale = 4.5f;
    
    private Material _material;
    
    void OnEnable()
    {
        _material = _renderer.material;
        _material.SetFloat("_PatternOffsetX", Random.Range(0, _maxPatternOffsetX));
        _material.SetFloat("_PatternOffsetY", Random.Range(0, _maxPatternOffsetY));
        _material.SetFloat("_PatternScale", Random.Range(0, _maxPatternScale));
    }
}
