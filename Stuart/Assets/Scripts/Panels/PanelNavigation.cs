using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelNavigation : MonoBehaviour
{
    private const float duration = 1.5f;

    [SerializeField] private int currentPanel;
    [SerializeField] private int lastPanelPlayed;
    [SerializeField] private int nextPanel;
    [SerializeField] private int maxPanelReached;

    private bool isLocked;

    [SerializeField] private GameObject[] panels;
    [SerializeField] private Panel[] panelGroups;

    [SerializeField] private Image coverMask;

    private Camera cam;

    private void Start()
    {
        isLocked = true;

        cam = GetComponent<Camera>();

        panelGroups = new Panel[panels.Length];

        for (int i = 0; i < panels.Length; i++)
            panelGroups[i] = panels[i].GetComponent<Panel>();

        maxPanelReached = PlayerPrefs.GetInt("MaxPanelReached");
        lastPanelPlayed = PlayerPrefs.GetInt("LastPanelPlayed");

        if (!PlayerPrefs.HasKey("CurrentPanel"))
        {
            currentPanel = 0;
            nextPanel = 1;
            PlayerPrefs.SetInt("CurrentPanel", 0);
            PlayerPrefs.SetInt("LastPanelPlayed", 0);
            PlayerPrefs.SetInt("IsLastPanelPlayed", 0);
            PlayerPrefs.SetInt("MaxPanelReached", 0);
        }
        else
        {
            currentPanel = PlayerPrefs.GetInt("CurrentPanel");
            
            nextPanel = maxPanelReached + 1;
            
            /*if (PlayerPrefs.GetInt("IsLastPanelPlayed") == 1)
            {
                
            }
            else if (PlayerPrefs.GetInt("IsLastPanelPlayed") == 0)
            {
                if (maxPanelReached >= lastPanelPlayed)
                {
                    nextPanel = maxPanelReached + 1;
                }
                else if (maxPanelReached < lastPanelPlayed)
                {
                    maxPanelReached--;
                    PlayerPrefs.SetInt("MaxPanelReached", maxPanelReached);
                    nextPanel = maxPanelReached + 1;
                }
            }*/

            // Modify image for all played panels
            if (currentPanel > 0)
            {
                for (int i = 0; i < maxPanelReached; i++)
                {
                    //panels[i].GetComponent<SpriteRenderer>().color = Color.grey;
                    panelGroups[i].SetImage(Color.grey);
                }

                if (PlayerPrefs.GetInt("IsLastPanelPlayed") == 0
                    && maxPanelReached < currentPanel)
                {
                    /*panels[currentPanel - 1].GetComponent<SpriteRenderer>().
                        color = Color.white;*/
                    panelGroups[currentPanel - 1].SetImage(Color.white);
                }
            }
            
            if (nextPanel > panels.Length)
                nextPanel = panels.Length;

            if (currentPanel > 0)
            {
                cam.orthographicSize = 2.5f;
                transform.position = new Vector3(
                    panels[currentPanel - 1].transform.position.x,
                    panels[currentPanel - 1].transform.position.y, -10);
            }
        }

        coverMask.enabled = true;
        StartCoroutine(AdjustCover(true, 1.0f));
    }

    private void Update()
    {
        if (Input.GetButtonUp("Select") && currentPanel == 0 && !isLocked)
        {
            Debug.Log("Move to first panel");
            isLocked = true;
            StartCoroutine(Move(0));
            currentPanel = 1;
            PlayerPrefs.SetInt("CurrentPanel", currentPanel);
        }

        if (Input.GetAxis("Horizontal") > 0 && currentPanel > 0
            && currentPanel < nextPanel && !isLocked)
        {
            Debug.Log("Move to next panel");
            isLocked = true;
            StartCoroutine(Move(PlayerPrefs.GetInt("CurrentPanel")));
            ++currentPanel;
            PlayerPrefs.SetInt("CurrentPanel", currentPanel);
        }

        if (Input.GetAxis("Horizontal") < 0 && currentPanel > 1
            && currentPanel <= nextPanel && !isLocked)
        {
            Debug.Log("Move to previous panel");
            isLocked = true;
            --currentPanel;
            PlayerPrefs.SetInt("CurrentPanel", currentPanel);
            StartCoroutine(Move(currentPanel - 1));
        }

        if (Input.GetButtonDown("Select") && currentPanel > 0 && !isLocked)
        {
            PlayerPrefs.SetInt("LastPanelPlayed", currentPanel);
            PlayerPrefs.SetInt("IsLastPanelPlayed", 0);
            isLocked = true;
            StartCoroutine(Zoom());
            StartCoroutine(AdjustCover(false));
        }
    }

    private IEnumerator Move(int index)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = panels[index].transform.position;

        do
        {
            if (cam.orthographicSize > 2.5f)
                cam.orthographicSize = 
                    cam.orthographicSize - 2f * Time.deltaTime;
            else
                cam.orthographicSize = 2.5f;
            transform.position = Vector3.Lerp(
                startPosition, new Vector3(targetPosition.x, targetPosition.y, -10), 
                timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        while (timeElapsed < duration);

        transform.position = new Vector3(targetPosition.x, targetPosition.y, -10);
        isLocked = false;
    }

    private IEnumerator Zoom()
    {
        float timeElapsed = 0;

        do
        {
            cam.orthographicSize = 
                cam.orthographicSize - 2f * Time.deltaTime;
            yield return null;
        }
        while(timeElapsed < duration);
    }

    private IEnumerator AdjustCover(bool reveal, float waitTime = 0)
    {
        float time = 0.0f, totalTime = 1.0f;
        Color c = coverMask.color;

        if (waitTime != 0)
            yield return new WaitForSeconds(waitTime);

        if (reveal)
        {
            do
            {
                time += Time.deltaTime;
                c.a = 1.0f - Mathf.Clamp01(time / totalTime);
                coverMask.color = c;
                yield return null;
            }
            while(time < totalTime);
        }
        else 
        {
            do
            {
                time += Time.deltaTime;
                c.a = 0.0f + Mathf.Clamp01(time / totalTime);
                coverMask.color = c;
                yield return null;
            }
            while(time < totalTime);

            SceneManager.LoadScene(currentPanel);
        }

        isLocked = false;
    }
}
