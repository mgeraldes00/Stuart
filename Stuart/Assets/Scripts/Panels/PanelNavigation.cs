using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelNavigation : MonoBehaviour
{
    private const float duration = 1.5f, turnDuration = 0.5f, switchTime = 0.1f;

    [SerializeField] private int currentPanel;
    [SerializeField] private int currentPage;
    [SerializeField] private int lastPanelPlayed;
    [SerializeField] private int nextPanel;
    [SerializeField] private int maxPanelReached;

    [SerializeField] private bool isLocked, isFocusLocked, zoomIn;

    [SerializeField] private GameObject[] panels, pages;
    [SerializeField] private Panel[] panelGroups;

    [SerializeField] private Animator coverAnim;
    [SerializeField] private Image coverMask;

    private Camera cam;

    private void Start()
    {
        isLocked = true;
        isFocusLocked = true;

        cam = GetComponent<Camera>();

        panelGroups = new Panel[panels.Length];

        for (int i = 0; i < panels.Length; i++)
            panelGroups[i] = panels[i].GetComponent<Panel>();

        maxPanelReached = PlayerPrefs.GetInt("MaxPanelReached");
        lastPanelPlayed = PlayerPrefs.GetInt("LastPanelPlayed");

        if (!PlayerPrefs.HasKey("CurrentPanel"))
        {
            currentPanel = 0;
            currentPage = 0;
            nextPanel = 1;
            PlayerPrefs.SetInt("CurrentPanel", 0);
            PlayerPrefs.SetInt("CurrentPage", 0);
            PlayerPrefs.SetInt("LastPanelPlayed", 0);
            PlayerPrefs.SetInt("IsLastPanelPlayed", 0);
            PlayerPrefs.SetInt("MaxPanelReached", 0);

            pages[0].SetActive(true);
        }
        else
        {
            currentPanel = PlayerPrefs.GetInt("CurrentPanel");
            currentPage = PlayerPrefs.GetInt("CurrentPage");
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
                cam.orthographicSize = 9f;
                transform.position = new Vector3(
                    panels[currentPanel - 1].transform.position.x,
                    panels[currentPanel - 1].transform.position.y, -10);
            }

            // Activate current page
            for (int i = 0; i < pages.Length; i++)
                pages[i].SetActive(false);

            pages[currentPage].SetActive(true);
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
            isFocusLocked = true;
            StartCoroutine(Move(0));
            currentPanel = 1;
            PlayerPrefs.SetInt("CurrentPanel", currentPanel);
        }

        if (Input.GetAxis("Horizontal") > 0 && currentPanel > 0
            && currentPanel < nextPanel && !isLocked && !zoomIn)
        {
            Debug.Log("Move to next panel");
            isLocked = true;
            isFocusLocked = true;

            if (currentPanel % 2 == 0)
            {
                StartCoroutine(SwitchPage(true, "turnRight"));
                StartCoroutine(
                    Move(PlayerPrefs.GetInt("CurrentPanel"), true));
            }

            else StartCoroutine(Move(PlayerPrefs.GetInt("CurrentPanel")));

            ++currentPanel;
            PlayerPrefs.SetInt("CurrentPanel", currentPanel);
        }

        if (Input.GetAxis("Horizontal") < 0 && currentPanel > 1
            && currentPanel <= nextPanel && !isLocked && !zoomIn)
        {
            Debug.Log("Move to previous panel");
            isLocked = true;
            isFocusLocked = true;

            if (currentPanel % 2 != 0)
            {
                --currentPanel;
                PlayerPrefs.SetInt("CurrentPanel", currentPanel);
                StartCoroutine(SwitchPage(false, "turnLeft"));
                StartCoroutine(Move(currentPanel - 1, true));
            }
            else
            {
                --currentPanel;
                PlayerPrefs.SetInt("CurrentPanel", currentPanel);
                StartCoroutine(Move(currentPanel - 1));
            }
        }

        //TODO
        if (Input.GetAxis("VerticalAlt") > 0 
            && panelGroups[currentPanel - 1].CurrentPanel <
            panelGroups[currentPanel - 1].Images.Length - 1
            && !isLocked && !isFocusLocked && zoomIn)
        {
            Debug.Log("Move to next image");
            isLocked = true;
            isFocusLocked = true;

            StartCoroutine(Move(
                currentPanel, false, true, true, false, 5.5f, 3.5f));
        }

        if (Input.GetAxis("VerticalAlt") < 0
            && panelGroups[currentPanel - 1].CurrentPanel > 0
            && !isLocked && !isFocusLocked && zoomIn)
        {
            Debug.Log("Move to previous image");
            isLocked = true;
            isFocusLocked = true;

            StartCoroutine(Move(
                currentPanel, false, true, true, true, 5.5f, 3.5f));
        }

        if (Input.GetButtonDown("Select") && currentPanel > 0 && !isLocked)
        {
            PlayerPrefs.SetInt("LastPanelPlayed", currentPanel);
            PlayerPrefs.SetInt("IsLastPanelPlayed", 0);
            isLocked = true;
            isFocusLocked = true;
            StartCoroutine(Zoom(2f));
            StartCoroutine(AdjustCover(false));
        }

        if (Input.GetButtonDown("Focus") && currentPanel > 0 && !isFocusLocked)
        {
            if (!zoomIn)
            {
                isLocked = true;
                isFocusLocked = true;
                StartCoroutine(Move(
                    currentPanel, false, true, true, false, 5.5f, 3.5f));
            }
            else
            {
                isLocked = true;
                isFocusLocked = true;
                panelGroups[currentPanel - 1].ResetCurrentPanel();
                StartCoroutine(Move(
                    currentPanel, false, true, false, false, -5.5f, 9f));
            }
        }
    }

    private IEnumerator SwitchPage(bool increase, string turnDirection)
    {
        coverAnim.SetTrigger(turnDirection);

        Color c = coverMask.color;
        c.a = 1;
        coverMask.color = c;
 
        yield return new WaitForSeconds(turnDuration);
        pages[currentPage].SetActive(false);

        if (increase) currentPage++;
        else currentPage--;

        pages[currentPage].SetActive(true);
        PlayerPrefs.SetInt("CurrentPage", currentPage);

        yield return new WaitForSeconds(turnDuration);
        c.a = 0;
        coverMask.color = c;
        isLocked = false;
        isFocusLocked = false;
    }

    private IEnumerator Move(int index, bool switchPage = false, 
        bool isFocus = false, bool focusing = false, bool goingUp = false,
        float zoomAmount = 2f, float maxSize = 9f)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition;

        float finalDuration = duration;

        if (isFocus && focusing)
            if (cam.orthographicSize > maxSize)
                targetPosition = panelGroups[index - 1].Images[0].transform.position;
            else
                targetPosition = panelGroups[index - 1].SelectImage(goingUp);
        else if (isFocus && !focusing)
            targetPosition = panels[index - 1].transform.position;
        else targetPosition = panels[index].transform.position;

        if (switchPage)
        {
            finalDuration = switchTime;
            yield return new WaitForSeconds(turnDuration);
        }

        do
        {
            if (cam.orthographicSize > maxSize && zoomAmount >= 0
                || cam.orthographicSize < maxSize && isFocus && !focusing)
                cam.orthographicSize = 
                    cam.orthographicSize - zoomAmount * Time.deltaTime;
            else
                cam.orthographicSize = maxSize;

            transform.position = Vector3.Lerp(
                startPosition, new Vector3(targetPosition.x, targetPosition.y, -10), 
                timeElapsed / finalDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        while (timeElapsed < finalDuration);

        transform.position = new Vector3(targetPosition.x, targetPosition.y, -10);
        if (!switchPage)
        {
            isLocked = false;
            isFocusLocked = false;
        }

        if (isFocus) isFocusLocked = false;
        
        if(focusing) zoomIn = true; 
        else if (isFocus && !focusing)
        {
            zoomIn = false;
            isLocked = false;
        } 
    }

    private IEnumerator Zoom(float value)
    {
        do
        {
            cam.orthographicSize = 
                cam.orthographicSize - value * Time.deltaTime;
            yield return null;
        }
        while(true);
    }

    private IEnumerator AdjustCover(bool reveal, float waitTime = 0)
    {
        float time = 0.0f, totalTime = 1.0f;
        Color c = coverMask.color;

        if (waitTime != 0) yield return new WaitForSeconds(waitTime);

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
        isFocusLocked = false;
    }
}
