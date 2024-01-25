using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FeedMoveController : MonoBehaviour
{
    [SerializeField] float _power = 5f;
    [SerializeField] float _deleteTime = 10f;
    Rigidbody _rigidbody;
    Ray _ray;

    private void Awake()
    {
        //　Rigidbodyを取得し速度を0に初期化
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {

        //　カメラからクリックした位置にレイを飛ばす
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //　弾が存在していればレイの方向に力を加える
        _rigidbody.AddForce(_ray.direction * _power, ForceMode.Impulse);

        //　弾を発射してから指定した時間が経過したら自動で削除
        Destroy(this.gameObject, _deleteTime);
    }

    private void FixedUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Enemyタグがついた敵に衝突したら自身と敵を削除
        if (collision.gameObject.tag == ("Carp"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
