using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level5Controller : MonoBehaviour, IController
{
    private const int maxCoins = 0;

    [SerializeField] private CameraCtrl cam;
    [SerializeField] private Player player;
    [SerializeField] private Follower follower;
    [SerializeField] private ActiveAgent activeAgent;
    [SerializeField] private SpeechBalloon playerSpeech, followerSpeech, otherSpeech;

    [SerializeField] private Image coverMask;

    [SerializeField] private GameObject[] bounds;

    [SerializeField] private GameObject[] refPoints;

    [SerializeField] private GameObject goal;

    [SerializeField] private string[] playerDialogue, followerDialogue, otherDialogue;
    [SerializeField] private string playerThought;

    [SerializeField] private int coinNum = 0;

    [SerializeField] private float fps;
    [SerializeField] private Text fpsText;

    private void Start()
    {
        Application.targetFrameRate = 300;

        cam = FindObjectOfType<CameraCtrl>();

        player = FindObjectOfType<Player>();
        playerSpeech = 
            player.gameObject.GetComponentInChildren<SpeechBalloon>();
        follower = FindObjectOfType<Follower>();
        followerSpeech = 
            follower.gameObject.GetComponentInChildren<SpeechBalloon>();
        activeAgent = FindObjectOfType<ActiveAgent>();
        otherSpeech = 
            activeAgent.gameObject.GetComponentInChildren<SpeechBalloon>();

        coverMask.enabled = true;
        InitializeScene();

        //InvokeRepeating(nameof(GetFPS), 1, 1);
    }

    private void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = fps + " fps";
    }

    public void InitializeScene()
    {
        StartCoroutine(player.EnterScene(2.0f));
        StartCoroutine(follower.EnterScene(2.0f));

        Invoke("ActivateBounds", 5.0f);
    }

    public void ActivateBounds()
    {
        for (int i = 0; i < bounds.Length; i++)
            bounds[i].SetActive(true);

        StartCoroutine(cam.Unlock());
    }

    public void SetDialogue(
        string [] playerLines, string[] followerLines, string[] otherLines)
    {
        playerDialogue = playerLines;
        followerDialogue = followerLines;
        otherDialogue = otherLines;
    }

    public void SetThought(string thought)
    {
        playerThought = thought;
    }

    public void BeginEvent(int i)
    {
        switch (i)
        {
            case 1:
                StartCoroutine(cam.Lock());
                player.Lock();
                follower.Lock();

                otherSpeech.DefineDialogue(otherDialogue);
                playerSpeech.DefineDialogue(playerDialogue);
                followerSpeech.DefineDialogue(followerDialogue);

                StartCoroutine(Dialogue(
                    new int[] { 2, 0, 1, 0, 2, 0, 1, 0},
                    1.0f, new int[] { 3, 4 }));
                
                bounds[1].SetActive(false);
                break;
            case 2:
                StartCoroutine(player.LeaveScene());
                StartCoroutine(follower.LeaveScene());
                break;
        }
    }

    public void CollectCoin()
    {
        coinNum++;
    }

    public IEnumerator Dialogue(
        int[] dialogue, float timeToStart, int[] extraParams = null)
    {
        int index = 0;

        Component[] speakers = new Component[] { player, follower, activeAgent };

        Component currentSpeaker;
        Component[] currentListener = new Component[2];

        int turningPoint = -1, returnPoint = -1;

        if (extraParams != null)
        {
            turningPoint = extraParams[0];
            returnPoint = extraParams[1];
        }

        yield return new WaitForSeconds(timeToStart);
        do
        {
            currentSpeaker = speakers[dialogue[index]];

            if (currentSpeaker == speakers[0])
            {
                currentListener[0] = speakers[1];
                currentListener[1] = speakers[2];
            }
            else if (currentSpeaker == speakers[1])
            {
                currentListener[0] = speakers[0];
                currentListener[1] = speakers[2];
            }
            else if (currentSpeaker == speakers[2])
            {
                currentListener[0] = speakers[0];
                currentListener[1] = speakers[1];
            }

            if (extraParams != null)
            {
                if (index == turningPoint)
                {
                    currentSpeaker.SendMessage("Turn");
                }
                    
                if (index == returnPoint)
                {
                    currentListener[0].SendMessage("Turn");
                } 
            }

            yield return new WaitForEndOfFrame();

            currentSpeaker.SendMessage("Talk");
            currentListener[0].SendMessage("Listen");
            currentListener[1].SendMessage("Listen");

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetButtonDown("Select"));
            yield return null;

            index++;
        }
        while (index < dialogue.Length);

        currentSpeaker.SendMessage("Listen");
        
        yield return new WaitForSeconds(0.2f);
        goal.SetActive(true);
        //follower.Unlock();
        //yield return new WaitForSeconds(0.8f);
        //player.Unlock();
        //StartCoroutine(cam.Unlock());
    }
}
