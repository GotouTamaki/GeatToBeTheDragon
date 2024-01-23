using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedPitcher : MonoBehaviour
{
    /// <summary>弾のプレハブ</summary>
    [SerializeField] Camera _camera;
    /// <summary>弾のプレハブ</summary>
    [SerializeField] GameObject _bullet;
    /// <summary>レンズからのオフセット値</summary>
    [SerializeField] float _offset;

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            //　カメラのレンズの中心を求める
            var centerOfLens = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane + _offset));
            //　カメラのレンズの中心から弾を飛ばす
            var bulletObj = Instantiate(_bullet, centerOfLens, Quaternion.identity) as GameObject;
        }
    }
}
