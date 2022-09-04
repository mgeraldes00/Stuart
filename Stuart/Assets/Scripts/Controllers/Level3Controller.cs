using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level3Controller : MonoBehaviour, IController
{
    private const int maxCoins = 3;

    [SerializeField] private CameraCtrl cam;
    [SerializeField] private Player player;
    [SerializeField] private Follower follower;
    [SerializeField] private SpeechBalloon playerSpeech, followerSpeech;

    [SerializeField] private Image coverMask;
    [SerializeField] private Image coinImage;
    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] private bool uiRevealed;

    [SerializeField] private GameObject[] bounds;

    [SerializeField] private GameObject[] refPoints;

    [SerializeField] private GameObject[] props;

    [SerializeField] private string[] playerDialogue, followerDialogue;
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

    public void SetDialogue(string [] playerLines, string[] followerLines)
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
                follower.Lock();

                playerSpeech.DefineDialogue(playerDialogue);
                followerSpeech.DefineDialogue(followerDialogue);

                StartCoroutine(player.AdjustPosition());
                StartCoroutine(follower.AdjustPosition(false));

                StartCoroutine(Dialogue(
                    new int[] { 0, 1 },
                    2.0f));
                break;
            // Event : separation point
            case 2:
                StartCoroutine(cam.Lock());
                player.Lock();
                follower.Lock();

                playerSpeech.DefineDialogue(playerDialogue);
                followerSpeech.DefineDialogue(followerDialogue);

                StartCoroutine(player.AdjustPosition(true, 5, 3.5f));
                StartCoroutine(follower.AdjustPosition(false, 7));

                StartCoroutine(Dialogue(
                    new int[] { 0, 1 },
                    4.5f, new int[] {-1, -1}));
                
                //bounds[1].SetActive(false);
                break;
            // Event : final
            case 3:

                break;
            // Event : leave scene
            case 4:
                StartCoroutine(player.LeaveScene());
                break;
            // Event : liberate exit
            case 5:

                break;
            // Event : not enough coins
            case 6:
                player.Lock();
                player.Turn();
                StartCoroutine(player.AdjustPosition(false, -2f, 2.0f));

                if (!uiRevealed)
                    StartCoroutine(ShowCoinUI());

                playerSpeech.DefineThought(playerThought);
                player.Think();

                player.Invoke(nameof(player.Unlock), 2.5f);
                player.Invoke(nameof(player.Listen), 3.0f);
                break;
        }
    }

    public void CollectCoin()
    {
        coinNum++;

        coinText.text = $"{coinNum}/{maxCoins}";

        if (coinNum == maxCoins)
        {
            refPoints[0].SetActive(false);
        }
    }

    private IEnumerator ShowCoinUI()
    {
        float fadeTime = 1;
        float elapsedTime = 0;

        Color cImg = coinImage.color;
        Color cTxt = coinText.color;

        do
        {
            elapsedTime += Time.deltaTime;

            cImg.a = Mathf.Clamp01(elapsedTime / fadeTime);
            cTxt.a = Mathf.Clamp01(elapsedTime / fadeTime);

            coinImage.color = cImg;
            coinText.color = cTxt;
            yield return null;
        }
        while (elapsedTime < fadeTime);

        uiRevealed = true;
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
        if (extraParams == null)
            follower.Unlock();
        else
        {
            yield return new WaitForSeconds(0.2f);
            follower.Turn();
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(follower.LeaveScene());
            props[0].SetActive(true);
            // Play prop sound
        }
        yield return new WaitForSeconds(0.8f);
        player.Unlock();
        StartCoroutine(cam.Unlock());
    }
}
