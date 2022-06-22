using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour, IController
{
    [SerializeField] private Player player;
    [SerializeField] private Follower follower;

    [SerializeField] private GameObject[] bounds;
    [SerializeField] private Transform reunionPoint;

    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = 300;

        player = FindObjectOfType<Player>();
        follower = FindObjectOfType<Follower>();

        InitializeScene();
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
                StartCoroutine(follower.EnterScene(2.0f));

                StartCoroutine(Dialogue(new int[] { 0, 1, 0, 1, 0 }, 5.0f));
                break;
            case 2:

                break;
        }
    }

    private IEnumerator Dialogue(int[] dialogue, float timeToStart)
    {
        int index = 0;

        Component[] speakers = new Component[] { player, follower };

        Component currentSpeaker, currentListener;

        yield return new WaitForSeconds(timeToStart);
        do
        {
            currentSpeaker = speakers[dialogue[index]];
            
            if (currentSpeaker == speakers[0])
                currentListener = speakers[1];
            else
                currentListener = speakers[0];

            currentSpeaker.SendMessage("Talk");
            currentListener.SendMessage("Listen");

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetButtonDown("Select"));
            yield return null;

            index++;
        }
        while (index < dialogue.Length);
    }
}
