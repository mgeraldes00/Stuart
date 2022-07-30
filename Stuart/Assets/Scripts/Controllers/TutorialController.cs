using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour, IController
{
    [SerializeField] private CameraCtrl cam;
    [SerializeField] private Player player;
    [SerializeField] private Follower follower;
    [SerializeField] private SpeechBalloon playerSpeech, followerSpeech;

    [SerializeField] private Image coverMask;

    [SerializeField] private GameObject[] bounds;

    [SerializeField] private GameObject[] refPoints;

    [SerializeField] private string[] playerDialogue, followerDialogue;
    [SerializeField] private string playerThought;

    [SerializeField] private int coinNum = 0, maxCoins = 1;

    [SerializeField] private float fps;
    [SerializeField] private Text fpsText;

    // Start is called before the first frame update
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

        coverMask.enabled = true;
        InitializeScene();

        InvokeRepeating(nameof(GetFPS), 1, 1);
    }

    private void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = fps + " fps";
    }

    public void InitializeScene()
    {
        follower.Lock();
        StartCoroutine(player.EnterScene(2.0f));

        Invoke("ActivateBounds", 5.0f);
    }

    public void ActivateBounds()
    {
        for (int i = 0; i < bounds.Length; i++)
            bounds[i].SetActive(true);

        StartCoroutine(cam.Unlock());
    }

    public void SetDialogue(string[] playerLines, string[] followerLines)
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
            case 1:
                StartCoroutine(cam.Lock());
                player.Lock();
                StartCoroutine(follower.EnterScene(1.5f));

                playerSpeech.DefineDialogue(playerDialogue);
                followerSpeech.DefineDialogue(followerDialogue);
                StartCoroutine(Dialogue(
                    new int[] { 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1 }, 
                    4.5f, new int[] { 5, 8 }));
                break;
            case 2:
                refPoints[0].SetActive(true);
                refPoints[1].SetActive(false);
                refPoints[2].SetActive(true);
                break;
            case 3:
                StartCoroutine(cam.Lock());
                player.Lock();
                follower.Lock();

                StartCoroutine(player.AdjustPosition());
                StartCoroutine(follower.AdjustPosition());

                playerSpeech.DefineDialogue(playerDialogue);
                followerSpeech.DefineDialogue(followerDialogue);
                StartCoroutine(Dialogue(
                    new int[] { 1, 0, 1, 0 },
                    2.0f));

                bounds[0].SetActive(false);
                refPoints[2].SetActive(false);
                break;
            case 4:
                StartCoroutine(player.LeaveScene());
                StartCoroutine(follower.LeaveScene());
                break;
            case 5:
                player.Lock();
                player.Turn();
                StartCoroutine(player.AdjustPosition(false, -2f, 2.0f));

                playerSpeech.DefineThought(playerThought);
                player.Think();

                player.Invoke(nameof(player.Unlock), 2.2f);
                player.Invoke(nameof(player.Listen), 2.0f);
                break;
            case 6:
                playerSpeech.DefineThought(playerThought);
                player.Think();

                player.Invoke(nameof(player.Listen), 2.0f);
                break;
        }
    }

    public void CollectCoin()
    {
        coinNum++;

        if (coinNum == maxCoins)
        {
            BeginEvent(2);
        }
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
