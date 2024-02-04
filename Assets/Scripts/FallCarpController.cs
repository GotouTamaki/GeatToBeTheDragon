using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallCarpController : MonoBehaviour
{
    [SerializeField] float _deleteTime = 10f;
    
    private void OnEnable()
    {
        // 時間経過でDestroyする
        Destroy(this.gameObject, _deleteTime);
    }
}