using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour, IController
{
    private GameObject player, follower;

    [SerializeField] private GameObject[] bounds;
    [SerializeField] private Transform reunionPoint;

    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = 300;

        player = FindObjectOfType<Player>().gameObject;
        //follower = FindObjectOfType<Follower>().gameObject;

        InitializeScene();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void InitializeScene()
    {
        player.GetComponent<Player>().IsTutorial = true;
        //follower.GetComponent<Follower>().IsTutorial = true;

        StartCoroutine(player.GetComponent<Player>().EnterScene(2.0f));

        Invoke("ActivateBounds", 5.0f);
    }

    public void ActivateBounds()
    {
        for (int i = 0; i < bounds.Length; i++)
            bounds[i].SetActive(true);
    }
}
