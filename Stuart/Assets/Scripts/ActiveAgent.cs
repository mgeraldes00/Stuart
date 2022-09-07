using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAgent : MonoBehaviour
{
    [SerializeField] private SpeechBalloon speechBalloon;

    [SerializeField] private bool talking;

    public void Talk()
    {
        if (!talking)
        {
            speechBalloon.ShowDialogueBalloon();
        }
        
        StartCoroutine(ResetTalk());
    }

    public void Listen()
    {
        speechBalloon.HideBalloon();
    }

    private IEnumerator ResetTalk()
    {
        talking = true;
        yield return new WaitForEndOfFrame();
        talking = false;
    }
}
