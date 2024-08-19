using UnityEngine;

public class DutherController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Renderer _renderer;
    float _distanse;

    private Material _material;
    
    // Start is called before the first frame update
    void Start()
    {
        _material = _renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        _distanse = Vector3.Distance(
            _mainCamera.gameObject.transform.position, this.gameObject.transform.position);
        _material.SetFloat("_DitherLevel", _distanse);
    }
}
