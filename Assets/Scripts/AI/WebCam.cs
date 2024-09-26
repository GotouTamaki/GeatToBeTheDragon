using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Webカメラ
public class WebCam : MonoBehaviour
{
    // 推論
    [SerializeField] private RenderTexture _inputTexture; //カメラからの画像
    [SerializeField] private Classifier _classifier; // 分類
    [SerializeField] private Text _uiText; // テキスト

    private bool _isWorking; // 処理中

    // スタート時に呼ばれる
    void Start()
    {
    }

    // フレーム毎に呼ばれる
    private void Update()
    {
        // 画像分類
        //TFClassify();
    }

    //画像分類
    private void TFClassify()
    {
        if (_isWorking)
        {
            return;
        }
    
        _isWorking = true;
    
        // 画像の前処理
        StartCoroutine(ProcessImage(result =>
        {
            // 推論の実行
            // StartCoroutine(_classifier.Predict(_inputTexture, probabilities =>
            // {
            //     // 推論結果の表示
            //     _uiText.text = "";
            //     for (int i = 0; i < 2; i++)
            //     {
            //         _uiText.text += probabilities[i].Key + ": " +
            //                              string.Format("{0:0.000}%", probabilities[i].Value) + "\n";
            //     }
            //
            //     // 未使用のアセットをアンロード
            //     Resources.UnloadUnusedAssets();
            //     _isWorking = false;
            // }));
        }));
    }

    //画像の前処理
    private IEnumerator ProcessImage(System.Action<Color32[]> callback)
    {
        // 画像のクロップ（WebCamTexture → Texture2D）
        yield return StartCoroutine(CropSquare(_inputTexture, texture =>
        {
            // 画像のスケール（Texture2D → Texture2D）
            var scaled = Scaled(texture,
                Classifier.IMAGE_SIZE,
                Classifier.IMAGE_SIZE);
    
            // コールバックを返す
            callback(scaled.GetPixels32());
        }));
    }
    
    // 画像のクロップ（WebCamTexture → Texture2D）
    public static IEnumerator CropSquare(RenderTexture _inputTexture, System.Action<Texture2D> callback)
    {
        // Texture2Dの準備
        var smallest = _inputTexture.width < _inputTexture.height ? _inputTexture.width : _inputTexture.height;
        var rect = new Rect(0, 0, smallest, smallest);
        Texture2D result = new Texture2D((int)rect.width, (int)rect.height);
        result.Apply();
    
        // 画像のクロップ
        // if (rect.width != 0 && rect.height != 0)
        // {
        //     result.SetPixels(_inputTexture.GetPixels(
        //         Mathf.FloorToInt((_inputTexture.width - rect.width) / 2),
        //         Mathf.FloorToInt((_inputTexture.height - rect.height) / 2),
        //         Mathf.FloorToInt(rect.width),
        //         Mathf.FloorToInt(rect.height)));
        //     yield return null;
        //     result.Apply();
        // }
    
        yield return null;
        callback(result);
    }

    // 画像のスケール（Texture2D → Texture2D）
    public static Texture2D Scaled(Texture2D texture, int width, int height)
    {
        // リサイズ後のRenderTextureの生成
        var rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(texture, rt);
    
        // リサイズ後のTexture2Dの生成
        var preRT = RenderTexture.active;
        RenderTexture.active = rt;
        var ret = new Texture2D(width, height);
        ret.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        ret.Apply();
        RenderTexture.active = preRT;
        RenderTexture.ReleaseTemporary(rt);
        return ret;
    }
}