using System;
using Unity.Barracuda;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// 分類
public class Classifier : MonoBehaviour
{
    // リソース
    public NNModel modelFile; // モデル
    public TextAsset labelsFile; // ラベル

    // パラメータ
    public const int IMAGE_SIZE = 224; // 画像サイズ
    private const int IMAGE_MEAN = 127; // MEAN

    private const float IMAGE_STD = 127.5f; // STD

    // private const string INPUT_NAME = "input"; // 入力名
    private const string INPUT_NAME = "sequential_1_input"; //★ 入力名
    private const string OUTPUT_NAME = "sequential_3"; //★出力名

    // 推論
    private IWorker _worker; // ワーカー
    private string[] _labels; // ラベル
    private int _waitIndex;

    // スタート時に呼ばれる
    void Start()
    {
        // ラベルとモデルの読み込み
        _labels = Regex.Split(labelsFile.text, "\n|\r|\r\n")
            .Where(s => !String.IsNullOrEmpty(s)).ToArray();
        var model = ModelLoader.Load(modelFile);

#if UNITY_EDITOR
        foreach (var label in _labels)
        {
            Debug.Log(label);
        }
#endif

        // ワーカーの生成
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    // 推論の実行
    public IEnumerator Predict(Color32[] picture, Action<List<KeyValuePair<string, float>>> callback)
    {
        // 結果
        var map = new List<KeyValuePair<string, float>>();

        // 入力テンソルの生成
        using (var tensor = TransformInput(picture, IMAGE_SIZE, IMAGE_SIZE))
        {
            // 入力の生成
            var inputs = new Dictionary<string, Tensor>();
            inputs.Add(INPUT_NAME, tensor);

            // 推論の実行
            var enumerator = _worker.ExecuteAsync(inputs);

            // 推論の実行の完了待ち
            while (enumerator.MoveNext())
            {
                _waitIndex++;
                if (_waitIndex >= 20)
                {
                    _waitIndex = 0;
                    yield return null;
                }
            }

            // 出力の生成
            var output = _worker.PeekOutput(OUTPUT_NAME);
            for (int i = 0; i < _labels.Length; i++)
            {
                map.Add(new KeyValuePair<string, float>(_labels[i], output[i] * 100));
            }
        }

        // ソートして結果を返す
        callback(map.OrderByDescending(x => x.Value).ToList());
    }

    // 入力テンソルの生成
    public static Tensor TransformInput(Color32[] pic, int width, int height)
    {
        float[] floatValues = new float[width * height * 3];
        for (int i = 0; i < pic.Length; ++i)
        {
            var color = pic[i];
            floatValues[i * 3 + 0] = (color.r - IMAGE_MEAN) / IMAGE_STD;
            floatValues[i * 3 + 1] = (color.g - IMAGE_MEAN) / IMAGE_STD;
            floatValues[i * 3 + 2] = (color.b - IMAGE_MEAN) / IMAGE_STD;
        }

        return new Tensor(1, height, width, 3, floatValues);
    }
}