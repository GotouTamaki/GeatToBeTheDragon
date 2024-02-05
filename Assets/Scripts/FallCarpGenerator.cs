using System.Collections.Generic;
using UnityEngine;

public class FallCarpGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] _fallObjects = null;
    [SerializeField] List<int> weights = new List<int>();   // 重み設定用変数
    [SerializeField] float _interval = 1.0f;
    [SerializeField] float _spawnPosiRange = 10f;
    [SerializeField] float _spawnNegaRange = -10f;
    [SerializeField] float _intervalMaxRange = 10f;
    [SerializeField] float _intervalMinRange = 1f;

    Vector3 _spawnPosi = Vector3.zero;
    float _timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;        
    }

    private void FixedUpdate()
    {
        if (_timer > _interval)
        {
            _spawnPosi = new Vector3(Random.Range(_spawnNegaRange, _spawnPosiRange), this.transform.position.y, 0);
            int num = Choose(weights);
            Instantiate(_fallObjects[num], _spawnPosi, _fallObjects[num].transform.rotation);
            _timer = 0;
            _interval = Random.Range(_intervalMinRange, _intervalMaxRange);
        }
    }
    
    /// <summary>抽選メソッド</summary>
    public int Choose(List<int> weight)
    {
        float total = 0f;
        //配列の要素をtotalに代入
        for (int i = 0;i < weight.Count; i++)
        {
            total += weight[i];
        }
        //Random.valueは0.1から1までの値を返す
        float random = UnityEngine.Random.value * total;
        //weightがrandomより大きいかを探す
        for (int i = 0;i < weight.Count ; i++)
        {
            if (random < weight[i])
            {
                //ランダムの値より重みが大きかったらその値を返す
                return i;
            }
            else
            {
                //次のweightが処理されるようにする
                random -= weight[i];
            }
        }
        //なかったら最後の値を返す
        return weight.Count -1;
    }
}
