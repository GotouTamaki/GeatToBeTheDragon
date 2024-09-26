using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class Discriminator : MonoBehaviour
{
    public bool IsWorking = true; // 処理中

    private const string INPUT_NAME = "sequential_1_input"; //★ 入力名
    private const string OUTPUT_NAME = "sequential_3"; //★ 出力名

    [SerializeField] private NNModel _modelAsset; // インポートしたモデル
    [SerializeField] private RenderTexture _inputTexture; // カメラからの画像
    [SerializeField] private TextAsset _labelsAsset; // ラベルが書かれているテキストファイル
    [SerializeField] private Text _uiText; // テキスト

    private Model _runtimeModel;
    private IWorker _worker;
    private string[] _labels;
    private int _waitIndex;
    private bool _isWorking; // 処理中

    private void Start()
    {
        // モデルのロード
        _runtimeModel = ModelLoader.Load(_modelAsset);
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Compute, _runtimeModel);

        // ラベルを単語ごとに配列に代入
        _labels = Regex.Split(_labelsAsset.text, "\n|\r|\r\n")
            .Where(s => !string.IsNullOrEmpty(s)).ToArray();

        StartCoroutine(Inference());
    }

    // private void Update()
    // {
    //     if (IsWorking ^ _isWorking && IsWorking)
    //     {
    //         StartCoroutine(Inference());
    //     }
    //     else
    //     {
    //         StopCoroutine(Inference());
    //     }
    // }

    public IEnumerator Inference()
    {
        // 画像認識が行われる
        while (IsWorking)
        {
            // カメラからの画像をモデルに渡している
            // 入力の生成
            Tensor tensorInput = new Tensor(_inputTexture, 3);
            _worker.Execute(tensorInput);
            var inputs = new Dictionary<string, Tensor>();
            inputs.Add(INPUT_NAME, tensorInput);

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
            Tensor output = _worker.PeekOutput(OUTPUT_NAME);
            _uiText.text = "";
            List<(string, float)> results = new List<(string, float)>();

            for (int i = 0; i < _labels.Length; i++)
            {
                results.Add((_labels[i], output[i] * 100));
            }

            foreach (var result in results /*.OrderByDescending(result => result.Item2)*/)
            {
                // 推論結果の表示
                _uiText.text += $"{result.Item1} : {string.Format("{0:0.000}%", result.Item2)}\n";
            }

            // 未使用のアセットをアンロード
            Resources.UnloadUnusedAssets();

            // TensorおよびWorkerは終了した時点で破棄する必要がある
            tensorInput.Dispose();
            output.Dispose();
            yield return null;
        }
    }

    private void OnDestroy()
    {
        // 終了時に破棄
        _worker.Dispose();
    }
}