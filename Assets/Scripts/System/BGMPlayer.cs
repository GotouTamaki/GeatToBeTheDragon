using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                AudioManager.Instance.PlayBGM(BGMSoundData.BGM.Title);
                break;
            case 1:
                AudioManager.Instance.PlayBGM(BGMSoundData.BGM.InGame);
                    break;
            case 2:
                AudioManager.Instance.PlayBGM(BGMSoundData.BGM.Result);
                break;
        }
    }
}
