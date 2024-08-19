using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField, Header("マップのプレハブ")] private GameObject[] _mapPrefabs;

    [SerializeField, Header("マップのスタート地点")] private Transform _startPosition;

    [SerializeField, Header("マップの経過地点")] private Transform _firstGoalPosition;

    [SerializeField, Header("初期マップ")] private List<GameObject> _firstMaps = new List<GameObject>();

    [SerializeField, Header("マップのロール速度")] private float _moveSpeed = 5;

    [SerializeField, Header("確認用")] private List<GameObject> _maps = new List<GameObject>();

    private bool _isEndGame;

    private Transform _goalPosition;

    //private Transform _playerPosition;

    private void Awake()
    {
        //_playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        _goalPosition = _firstGoalPosition;

        foreach (var map in _firstMaps)
        {
            _maps.Add(map);
        }
    }

    // public void EndGame()
    // {
    //     _isEndGame = true;
    // }

    private void Update()
    {
        Check();
        //SetSpeed(_moveSpeed);
        CheckDestroy();
    }

    private void FixedUpdate()
    {
        foreach (var map in _maps)
        {
            map.transform.position -= new Vector3(0, _moveSpeed, 0);
        }
    }

    public void SetSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    private void Check()
    {
        //float h = _goalPos.position.y - _playerT.position.y;

        if (_maps[0].transform.position.y < _goalPosition.position.y)
        {
            SpawnNewMap();
        }
    }

    private void CheckDestroy()
    {
        //if (_maps.Count <= 2) return;

        //float h = _maps[0].transform.position.y - _playerT.position.y;

        if (_maps[0].transform.position.y < _goalPosition.position.y)
        {
            var go = _maps[0];
            _maps.RemoveAt(0);
            Destroy(go);
        }
    }

    private void SpawnNewMap()
    {
        Vector3 pos = new Vector3(0, _startPosition.position.y, 0);
        var go = Instantiate(_mapPrefabs[Random.Range(0, _mapPrefabs.Length)]);
        go.transform.position = pos;

        // if (!_isEndGame)
        // {
        //     
        // }

        _maps.Add(go);
        //_goalPos = _maps[1].transform;
    }
}