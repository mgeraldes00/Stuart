using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand = System.Random;
using URand = UnityEngine.Random;

public class AgentGenerator : MonoBehaviour
{
    private const int maxAgents = 5;

    [SerializeField] private GameObject agentPrefab;

    private List<MobileAgent> sceneAgents = new List<MobileAgent>();

    [SerializeField] private GameObject[] startPoints;
    [SerializeField] private Vector2[] startPositions;

    // Start is called before the first frame update
    void Start()
    {
        startPositions = new Vector2[startPoints.Length];

        for (int i = 0; i < startPoints.Length; i++)
            startPositions[i] = new Vector2(
                startPoints[i].transform.position.x,
                startPoints[i].transform.position.y);

        StartCoroutine(Generate());
    }

    private void Update()
    {
        startPositions = new Vector2[startPoints.Length];

        for (int i = 0; i < startPoints.Length; i++)
            startPositions[i] = new Vector2(
                startPoints[i].transform.position.x,
                startPoints[i].transform.position.y);
    }

    private IEnumerator Generate()
    {
        while(true)
        {
            if (sceneAgents.Count <= maxAgents)
                DefineAgent();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void DefineAgent()
    {
        Rand rnd = new Rand();

        int spawnPoint = rnd.Next(0, 11), side = rnd.Next(0, 2);

        SpawnAgent(spawnPoint, side);
    }

    private void SpawnAgent(int pointGen, int side)
    {
        GameObject currentAgentObj;
        MobileAgent thisAgent;

        int spawnPoint = 0;
        float size = 0;
        bool facingRight = true, onForeground = false;

        switch (pointGen)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                size = URand.Range(1.0f, 1.3f);
                if (side == 0)
                {
                    spawnPoint = 0;
                }
                else
                {
                    spawnPoint = 1;
                    facingRight = false;
                }
                break;
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                size = URand.Range(1.1f, 1.4f);
                if (side == 0)
                {
                    spawnPoint = 2;
                }
                else
                {
                    spawnPoint = 3;
                    facingRight = false;
                }
                break;
            case 10:
                onForeground = true;
                size = 5;
                if (side == 0)
                {
                    spawnPoint = 4;
                }
                else
                {
                    spawnPoint = 5;
                    facingRight = false;
                }
                break;
        }
        
        if (facingRight)
            currentAgentObj = Instantiate(
                agentPrefab, startPositions[spawnPoint], Quaternion.identity);
        else
            currentAgentObj = Instantiate(
                agentPrefab, startPositions[spawnPoint],
                Quaternion.Euler(0, 180, 0));

        thisAgent = currentAgentObj.GetComponent<MobileAgent>();

        thisAgent.CheckOrientation(facingRight);
        
        sceneAgents.Add(thisAgent);

        if (onForeground)
            thisAgent.OnForeground();

        currentAgentObj.transform.localScale = new Vector3(size, size, 1);
    }
    
    public void RemoveAgent(MobileAgent agentToDestroy)
        => sceneAgents.Remove(agentToDestroy);
}
