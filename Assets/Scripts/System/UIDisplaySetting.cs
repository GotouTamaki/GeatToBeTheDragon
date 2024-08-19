using UnityEngine;

public class UIDisplaySetting : MonoBehaviour
{
    public void SetSettingUIDisplay(bool isDisplay)
    {
        CanvasGroup canvasGroup = GameObject.FindGameObjectWithTag("Setting").GetComponent<CanvasGroup>();
        canvasGroup.alpha = isDisplay ? 1 : 0;
        canvasGroup.interactable = isDisplay;
        canvasGroup.blocksRaycasts = isDisplay;
    }
}
