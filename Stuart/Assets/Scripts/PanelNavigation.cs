using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelNavigation : MonoBehaviour
{
    private const float duration = 1.5f;

    [SerializeField] private int currentPanel;

    private bool isLocked;

    [SerializeField] private GameObject[] panels;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        currentPanel = 0;
        PlayerPrefs.SetInt("CurrentPanel", 0);

        if (!PlayerPrefs.HasKey("CurrentPanel"))
        {
            currentPanel = 0;
            PlayerPrefs.SetInt("CurrentPanel", 0);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentPanel < panels.Length 
            && !isLocked)
        {
            Debug.Log("Move to panel");
            isLocked = true;
            StartCoroutine(Move(PlayerPrefs.GetInt("CurrentPanel")));
            currentPanel++;
            PlayerPrefs.SetInt("CurrentPanel", currentPanel);
        }
    }

    private IEnumerator Move(int index)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = panels[index].transform.position;

        do
        {
            if (cam.orthographicSize > 2.1f)
                cam.orthographicSize = 
                    cam.orthographicSize - 2f * Time.deltaTime;
            else
                cam.orthographicSize = 2.1f;
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
}
