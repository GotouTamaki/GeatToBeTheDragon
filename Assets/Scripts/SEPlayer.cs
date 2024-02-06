using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public void SEPlaySelect()
    {
        AudioManager.Instance.PlaySE(SESoundData.SE.Select);
    }
    public void SEPlaySplash()
    {
        AudioManager.Instance.PlaySE(SESoundData.SE.Splash);
    }
    
    public void SEPlayCollision()
    {
        AudioManager.Instance.PlaySE(SESoundData.SE.Collision);
    }
}
