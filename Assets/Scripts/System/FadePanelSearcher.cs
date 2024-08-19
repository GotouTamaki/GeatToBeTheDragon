using DG.Tweening;
using UnityEngine;

public class FadePanelSearcher : MonoBehaviour
{
    public void SceneChangeAndFadeStart(string sceneName)
    {
        SceneChangeAndFadeManager.Instance.SceneChangeAndFade(sceneName);
    }
}