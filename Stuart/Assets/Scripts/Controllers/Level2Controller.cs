using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level2Controller : MonoBehaviour, IController
{
    private const int maxCoins = 0;

    [SerializeField] private CameraCtrl cam;
    [SerializeField] private Player player;
    [SerializeField] private Follower follower;
    [SerializeField] private SpeechBalloon playerSpeech, followerSpeech;

    [SerializeField] private Image coverMask;

    [SerializeField] private Material material;

    [SerializeField] private float blurAmount;

    [SerializeField] private GameObject[] bounds;

    [SerializeField] private GameObject[] refPoints;

    [SerializeField] private string[] playerDialogue, followerDialogue;
    [SerializeField] private string playerThought;

    [SerializeField] private int coinNum = 0;

    [SerializeField] private float fps;
    [SerializeField] private Text fpsText;

    private void Start()
    {
        Application.targetFrameRate = 300;

        cam = FindObjectOfType<CameraCtrl>();

        material.SetFloat("_BlurAmount", blurAmount);

        player = FindObjectOfType<Player>();
        playerSpeech = 
            player.gameObject.GetComponentInChildren<SpeechBalloon>();
        follower = FindObjectOfType<Follower>();
        followerSpeech = 
            follower.gameObject.GetComponentInChildren<SpeechBalloon>();

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

        //StartCoroutine(cam.Unlock());
    }

    public void SetDialogue(
        string [] playerLines, string[] followerLines, string[] otherLines)
    {
        playerDialogue = playerLines;
        followerDialogue = followerLines;
    }

    public void SetThought(string thought)
    {
        playerThought = thought;
    }

    public void BeginEvent(int i)
    {
        switch (i)
        {
            case 0:
                player.Lock();
                follower.Lock();

                playerSpeech.DefineDialogue(playerDialogue);
                followerSpeech.DefineDialogue(followerDialogue);

                StartCoroutine(follower.AdjustPosition(false));

                StartCoroutine(Dialogue(
                    new int[] { 0, 1, 0 },
                    3.0f));
                break;
            case 1:
                StartCoroutine(cam.Lock());
                player.Lock();
                follower.Lock();

                playerSpeech.DefineDialogue(playerDialogue);
                followerSpeech.DefineDialogue(followerDialogue);

                StartCoroutine(Dialogue(
                    new int[] { 0, 1 },
                    1.0f));
                
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

        Component[] speakers = new Component[] { player, follower };

        Component currentSpeaker, currentListener;

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
                currentListener = speakers[1];
            else
                currentListener = speakers[0];

            if (extraParams != null)
            {
                if (index == turningPoint)
                {
                    currentListener.SendMessage("Turn");
                }
                    
                if (index == returnPoint)
                {
                    currentSpeaker.SendMessage("Turn");
                } 
            }

            yield return new WaitForEndOfFrame();

            currentSpeaker.SendMessage("Talk");
            currentListener.SendMessage("Listen");

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetButtonDown("Select"));
            yield return null;

            index++;
        }
        while (index < dialogue.Length);

        currentSpeaker.SendMessage("Listen");
        
        yield return new WaitForSeconds(0.2f);
        follower.Unlock();
        yield return new WaitForSeconds(0.8f);
        player.Unlock();
        StartCoroutine(cam.Unlock());
    }
}
