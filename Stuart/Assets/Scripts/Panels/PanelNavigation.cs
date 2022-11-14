using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelNavigation : MonoBehaviour
{
    private const int numOfLevels = 4;
    private const float duration = 1.5f, turnDuration = 0.5f, switchTime = 0.1f;

    [SerializeField] private AudioLeveler audioCtrl;
    [SerializeField] private MenuUIController menuUI;
    [SerializeField] private MenuStart startControl;

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

    [SerializeField] private AudioSource pageTurn;

    private Camera cam;

    private void Start()
    {
        // ENDGAME
        /*currentPanel = numOfLevels - 1;
        currentPage = 0;
        nextPanel = numOfLevels - 1;
        PlayerPrefs.SetInt("CurrentPanel", numOfLevels - 1);
        PlayerPrefs.SetInt("CurrentPage", 1);
        PlayerPrefs.SetInt("LastPanelPlayed", numOfLevels - 1);
        PlayerPrefs.SetInt("IsLastPanelPlayed", 1);
        PlayerPrefs.SetInt("MaxPanelReached", numOfLevels - 1);*/

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
            PlayerPrefs.SetInt("TransitionFromLevel", 0);

            pages[0].SetActive(true);

            StartCoroutine(SimpleLock(2.5f));
        }
        else
        {
            currentPanel = PlayerPrefs.GetInt("CurrentPanel");
            currentPage = PlayerPrefs.GetInt("CurrentPage");
            nextPanel = maxPanelReached + 1;

            if (currentPanel > 0)
            {
                for (int i = 0; i < maxPanelReached; i++)
                {
                    panelGroups[i].SetImage(true);
                }

                if (PlayerPrefs.GetInt("IsLastPanelPlayed") == 0
                    && maxPanelReached < currentPanel)
                {
                    panelGroups[currentPanel - 1].SetImage(false);
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

                if (currentPanel % 2 != 0)
                {
                    menuUI.UpdateUI(new int[] { 0 }, new int[] { });
                    if (currentPanel < nextPanel
                        || currentPanel == numOfLevels)
                        menuUI.UpdateUI(new int[] { 1 }, new int[] { });
                }
                else
                {
                    menuUI.UpdateUI(new int[] { 2 }, new int[] { });
                    if (currentPanel < nextPanel
                        || currentPanel == maxPanelReached 
                        && maxPanelReached == numOfLevels)
                        menuUI.UpdateUI(new int[] { 3 }, new int[] { });
                } 

                if (currentPanel > 1)
                    menuUI.UpdateNavigationUI(new int[] { 1 }, new int[] { });

                if (nextPanel > currentPanel)
                    menuUI.UpdateNavigationUI(new int[] { 0 }, new int[] { });
                
                    
            }

            for (int i = 0; i < pages.Length; i++)
                pages[i].SetActive(false);

            pages[currentPage].SetActive(true);

            if (PlayerPrefs.GetInt("TransitionFromLevel") != 0)
            {
                coverMask.enabled = true;
                StartCoroutine(AdjustCover(true, 1.0f));
            }
            else StartCoroutine(SimpleLock(1.0f));
        }

        if (currentPanel == 0) StartCoroutine(startControl.RevealControl());
    }

    private void Update()
    {
        if (Input.GetButtonUp("Select") && currentPanel == 0 && !isLocked)
        {
            isLocked = true;
            isFocusLocked = true;
            StartCoroutine(Move(0));
            currentPanel = 1;
            PlayerPrefs.SetInt("CurrentPanel", currentPanel);
            menuUI.UpdateUI(new int[] { 0 }, new int[] { });
            StartCoroutine(startControl.HideControl());
        }

        if (Input.GetAxis("Horizontal") > 0 && currentPanel > 0
            && currentPanel < nextPanel && !isLocked && !zoomIn)
        {
            isLocked = true;
            isFocusLocked = true;

            if (currentPanel % 2 == 0)
            {
                StartCoroutine(SwitchPage(true, "turnRight"));
                StartCoroutine(
                    Move(PlayerPrefs.GetInt("CurrentPanel"), true));
                menuUI.UpdateUI(new int[] { 0 }, new int[] { 2, 3 });
                if (currentPanel + 1 < nextPanel
                    || currentPanel + 1 == maxPanelReached 
                    && maxPanelReached == numOfLevels)
                    menuUI.UpdateUI(new int[] { 1 }, new int[] { });
            }
            else
            {
                StartCoroutine(Move(PlayerPrefs.GetInt("CurrentPanel")));
                menuUI.UpdateUI(new int[] { 2 }, new int[] { 0, 1 });
                if (currentPanel + 1 < nextPanel
                    || currentPanel + 1 == maxPanelReached 
                    && maxPanelReached == numOfLevels)
                    menuUI.UpdateUI(new int[] { 3 }, new int[] { });
            }

            ++currentPanel;
            PlayerPrefs.SetInt("CurrentPanel", currentPanel);

            if (currentPanel == 2)
                menuUI.UpdateNavigationUI(new int[] { 1 }, new int[] { });

            if (currentPanel == nextPanel)
                menuUI.UpdateNavigationUI(new int[] { }, new int[] { 0 });
        }

        if (Input.GetAxis("Horizontal") < 0 && currentPanel > 1
            && currentPanel <= nextPanel && !isLocked && !zoomIn)
        {
            isLocked = true;
            isFocusLocked = true;

            if (currentPanel % 2 != 0)
            {
                --currentPanel;
                PlayerPrefs.SetInt("CurrentPanel", currentPanel);
                StartCoroutine(SwitchPage(false, "turnLeft"));
                StartCoroutine(Move(currentPanel - 1, true));
                menuUI.UpdateUI(new int[] { 2, 3 }, new int[] { 0 });
                if (currentPanel + 1 < nextPanel
                    || currentPanel + 1 == maxPanelReached 
                    && maxPanelReached == numOfLevels)
                    menuUI.UpdateUI(new int[] { }, new int[] { 1 });
            }
            else
            {
                --currentPanel;
                PlayerPrefs.SetInt("CurrentPanel", currentPanel);
                StartCoroutine(Move(currentPanel - 1));
                menuUI.UpdateUI(new int[] { 0, 1 }, new int[] { 2 });
                if (currentPanel + 1 < nextPanel
                    || currentPanel + 1 == maxPanelReached 
                    && maxPanelReached == numOfLevels)
                    menuUI.UpdateUI(new int[] { }, new int[] { 3 });
            }

            if (currentPanel == nextPanel - 1)
                menuUI.UpdateNavigationUI(new int[] { 0 }, new int[] { });

            if (currentPanel == 1)
                menuUI.UpdateNavigationUI(new int[] { }, new int[] { 1 });
        }

        if (Input.GetAxis("VerticalAlt") > 0 && currentPanel > 0
            && panelGroups[currentPanel - 1].CurrentPanel <
            panelGroups[currentPanel - 1].Images.Length - 1
            && !isLocked && !isFocusLocked && zoomIn)
        {
            isLocked = true;
            isFocusLocked = true;

            StartCoroutine(Move(
                currentPanel, false, true, true, false, 5.5f, 3.5f));

            if (panelGroups[currentPanel - 1].CurrentPanel > 0)
                menuUI.UpdateNavigationUI(new int[] { 2 }, new int[] { });

            if (panelGroups[currentPanel - 1].CurrentPanel ==
                panelGroups[currentPanel - 1].Images.Length - 1)
                menuUI.UpdateNavigationUI(new int[] { }, new int[] { 3 });
                
        }

        if (Input.GetAxis("VerticalAlt") < 0 && currentPanel > 0
            && panelGroups[currentPanel - 1].CurrentPanel > 0
            && !isLocked && !isFocusLocked && zoomIn)
        {
            isLocked = true;
            isFocusLocked = true;

            StartCoroutine(Move(
                currentPanel, false, true, true, true, 5.5f, 3.5f));

            if (panelGroups[currentPanel - 1].CurrentPanel == 0)
                menuUI.UpdateNavigationUI(new int[] { }, new int[] { 2 });
            
            if (panelGroups[currentPanel - 1].CurrentPanel ==
                panelGroups[currentPanel - 1].Images.Length - 2)
                menuUI.UpdateNavigationUI(new int[] { 3 }, new int[] { });
        }

        if (Input.GetButtonDown("Select") && currentPanel > 0 && !isLocked)
        {
            PlayerPrefs.SetInt("LastPanelPlayed", currentPanel);
            PlayerPrefs.SetInt("IsLastPanelPlayed", 0);
            PlayerPrefs.SetInt("TransitionFromLevel", 0);
            isLocked = true;
            isFocusLocked = true;
            StartCoroutine(audioCtrl.AdjustVolume("Master Volume", 0, -60, 30));
            StartCoroutine(Zoom(2f));
            StartCoroutine(AdjustCover(false));
        }

        if (Input.GetButtonDown("Focus") && !isFocusLocked
            && currentPanel > 0 && currentPanel < nextPanel
            || Input.GetButtonDown("Focus") && !isFocusLocked 
            && currentPanel == maxPanelReached && maxPanelReached == numOfLevels)
        {
            if (!zoomIn)
            {
                isLocked = true;
                isFocusLocked = true;
                StartCoroutine(Move(
                    currentPanel, false, true, true, false, 5.5f, 3.5f));
                if (currentPanel % 2 != 0)
                    menuUI.UpdateUI(new int[] { }, new int[] { 0, 1 }, true);
                else
                    menuUI.UpdateUI(new int[] { }, new int[] { 2, 3 }, true);

                menuUI.UpdateNavigationUI(new int[] { 3 }, new int[] { 0, 1 });
            }
            else
            {
                isLocked = true;
                isFocusLocked = true;
                panelGroups[currentPanel - 1].ResetCurrentPanel();
                StartCoroutine(Move(
                    currentPanel, false, true, false, false, -5.5f, 9f));
                if (currentPanel % 2 != 0)
                    menuUI.UpdateUI(new int[] { 0, 1 }, new int[] { }, true);
                else
                    menuUI.UpdateUI(new int[] { 2, 3 }, new int[] { }, true);

                if (currentPanel > 1)
                    menuUI.UpdateNavigationUI(new int[] { 1 }, new int[] { 2, 3 });

                if (nextPanel > currentPanel)
                    menuUI.UpdateNavigationUI(new int[] { 0 }, new int[] { 2, 3 });
            }   
        }
    }

    private IEnumerator SwitchPage(bool increase, string turnDirection)
    {
        coverAnim.SetTrigger(turnDirection);

        Color c = coverMask.color;
        c.a = 1;
        coverMask.color = c;

        yield return new WaitForSeconds(0.15f);
        pageTurn.Play();
 
        yield return new WaitForSeconds(turnDuration - 0.15f);
        pages[currentPage].SetActive(false);

        if (increase) currentPage++;
        else currentPage--;

        pages[currentPage].SetActive(true);
        PlayerPrefs.SetInt("CurrentPage", currentPage);

        yield return new WaitForSeconds(turnDuration);
        c.a = 0;
        coverMask.color = c;

        yield return new WaitForSeconds(turnDuration);
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

    private IEnumerator SimpleLock(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isLocked = false;
        isFocusLocked = false;
    }

    private IEnumerator AdjustCover(bool reveal, float waitTime = 0)
    {
        float time = 0.0f, totalTime = 1.0f;
        Color c = coverMask.color;

        if (coverMask.enabled != true) coverMask.enabled = true;

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
            menuUI.UpdateUI(new int[] { }, new int[] { 0, 1, 2, 3 });
            menuUI.UpdateNavigationUI(new int[] { }, new int[] { 0, 1, 2, 3 });

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

        yield return new WaitForSeconds(waitTime);
        isLocked = false;
        isFocusLocked = false;
    }

    private void OnApplicationQuit() 
        => PlayerPrefs.SetInt("TransitionFromLevel", 0);
}
