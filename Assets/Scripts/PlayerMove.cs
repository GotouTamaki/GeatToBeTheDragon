using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] float _moveSpeed = 1f;
    Vector3 _dir = default;
    
    bool _canMove = false;
    // Start is called before the first frame update
    void Start()
    {
        _canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_canMove)
        {
            // 入力を受け取り、カメラを基準にした XZ 平面上に変換する
            _dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
            _dir = Camera.main.transform.TransformDirection(_dir);
            _dir.y = 0;

            this.transform.Translate(_dir * _moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
        {
            _rigidbody.WakeUp();
            _canMove = false;
            _rigidbody.useGravity = true;
        }
        else
        {
            _rigidbody.Sleep();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (!other.gameObject.CompareTag("Stage"))
        {
            _rigidbody.WakeUp();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!other.gameObject.CompareTag("Stage") && _canMove)
        {
            _rigidbody.Sleep();
        }
    }
}
