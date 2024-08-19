using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Serialization;

public class SceneChangeAndFadeManager : MonoBehaviour
{
    /// <summary>フェードパネルのCanvasGroup</summary>
    [SerializeField] private CanvasGroup _fadePanelCanvasGroup;
    /// <summary>フェードのインターバル</summary>
    [SerializeField] float _interval = 5f;

    public static SceneChangeAndFadeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>引数に入力された名前のシーンに遷移します</summary>
    /// <param name="sceneName">遷移するシーンの名前</param>
    public void SceneChangeAndFade(string sceneName)
    {
        // フェードモード 0がフェードイン 1がフェードアウト
        _fadePanelCanvasGroup.interactable = true;
        _fadePanelCanvasGroup.blocksRaycasts = true;
        _fadePanelCanvasGroup.DOFade(1, _interval)
            .OnComplete(() =>
            {
                // シーンを再読み込みする
                SceneManager.LoadScene(sceneName);
                // フェードインさせる
                _fadePanelCanvasGroup.DOFade(0, _interval)
                    .OnComplete(() =>
                    {
                        _fadePanelCanvasGroup.interactable = false;
                        _fadePanelCanvasGroup.blocksRaycasts = false;
                        _fadePanelCanvasGroup.DOFade(0, _interval);
                    });
            })
            .SetLink(_fadePanelCanvasGroup.gameObject);
    }
}