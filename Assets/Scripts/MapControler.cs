using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

public class MapControler : MonoBehaviour
{
    [FormerlySerializedAs("_mapPrefab")]
    [Header("マップのプレハブ")]
    [SerializeField] private GameObject[] _mapPrefabs;
    
    [Header("マップのスタート地点")]
    [SerializeField] private Transform _startPosition;
    
    [Header("マップの経過地点")]
    [SerializeField] private Transform _firstGoalPos;
    
    [Header("初期マップ")]
    [SerializeField] private List<GameObject> _firstMaps = new List<GameObject>();

    [Header("マップのロール速度")]
    [SerializeField] private float _moveSpeed = 5;

    [Header("確認用")]
    [SerializeField] private List<GameObject> _maps = new List<GameObject>();

    private bool _isEndGame = false;

    private Transform _goalPos;

    private Transform _playerT;

    private void Awake()
    {
        _playerT = GameObject.FindGameObjectWithTag("Player").transform;
        _goalPos = _firstGoalPos;

        foreach (var map in _firstMaps)
        {
            _maps.Add(map);
        }
    }

    public void EndGame()
    {
        _isEndGame = true;
    }
    private void Update()
    {
        Check();
        //SetSpeed();
        CheckDestroy();
    }

    private void FixedUpdate()
    {
        foreach (var map in _maps)
        {
            map.transform.position -= new Vector3(0, _moveSpeed, 0);
        }
    }

    public void SetSpeed()
    {
        
    }

    public void Check()
    {
        //float h = _goalPos.position.y - _playerT.position.y;

        if (_maps[0].transform.position.y < _goalPos.position.y)
        {
            SpownNewMap();
        }
    }

    public void CheckDestroy()
    {
        //if (_maps.Count <= 2) return;

        //float h = _maps[0].transform.position.y - _playerT.position.y;

        if (_maps[0].transform.position.y < _goalPos.position.y)
        {
            var go = _maps[0];
            _maps.RemoveAt(0);
            Destroy(go);
        }
    }

    public void SpownNewMap()
    {
        Vector3 pos = new Vector3(0, _startPosition.position.y, 0);
        var go = Instantiate(_mapPrefabs[Random.Range(0, _mapPrefabs.Length)]);
        go.transform.position = pos;

        if (!_isEndGame)
        {
            
        }

        _maps.Add(go);
        //_goalPos = _maps[1].transform;
    }
}
