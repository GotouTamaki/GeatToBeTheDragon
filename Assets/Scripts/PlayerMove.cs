using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 1f;
    Vector3 _dir = default;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 入力を受け取り、カメラを基準にした XZ 平面上に変換する
        _dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        _dir = Camera.main.transform.TransformDirection(_dir);
        _dir.y = 0;

        this.transform.Translate(_dir * _moveSpeed * Time.deltaTime);
    }
}
