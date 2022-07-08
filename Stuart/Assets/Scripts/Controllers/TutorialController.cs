using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour, IController
{
    [SerializeField] private Player player;
    [SerializeField] private Follower follower;

    [SerializeField] private GameObject[] bounds;
    [SerializeField] private Transform reunionPoint;
    [SerializeField] private GameObject fallPoint;

    [SerializeField] private int coinNum = 0, maxCoins = 1;

    [SerializeField] private float fps;
    [SerializeField] private Text fpsText;

    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = 300;

        player = FindObjectOfType<Player>();
        follower = FindObjectOfType<Follower>();

        InitializeScene();

        InvokeRepeating(nameof(GetFPS), 1, 1);
    }

    private void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = fps + " fps";
    }

    // Update is called once per frame
    private void Update()
    {
        
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
    }

    public void BeginEvent(int i)
    {
        switch (i)
        {
            case 1:
                player.Lock();
                StartCoroutine(follower.EnterScene(1.5f));

                StartCoroutine(Dialogue(
                    new int[] { 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1 }, 
                    4.5f, new int[] { 5, 8 }));
                break;
            case 2:
                fallPoint.SetActive(true);
                break;
            case 3:
                player.Lock();
                follower.Lock();

                StartCoroutine(player.AdjustPosition());
                StartCoroutine(follower.AdjustPosition());

                StartCoroutine(Dialogue(
                    new int[] { 1, 0, 1, 0 },
                    2.0f));

                bounds[0].SetActive(false);
                break;
            case 4:
                StartCoroutine(player.LeaveScene());
                StartCoroutine(follower.LeaveScene());
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
                    currentListener.SendMessage("Turn");
                if (index == returnPoint)
                    currentSpeaker.SendMessage("Turn");
            }

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
    }
}
