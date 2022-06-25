using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private string playerMask;

    [SerializeField] private bool isOnGoal;

    [SerializeField] private int levelIndex;

    private void Start()
    {
        levelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerMask))
        {
            Debug.Log("Reached goal");
            isOnGoal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerMask))
        {
            Debug.Log("Moved away from goal");
            isOnGoal = false;
        }
    }

    private void Update()
    {
        if (isOnGoal)
        {
            PlayerPrefs.SetInt("IsLastPanelPlayed", 1);
            if (PlayerPrefs.GetInt("MaxPanelReached") < levelIndex)
                PlayerPrefs.SetInt("MaxPanelReached", levelIndex);
            SceneManager.LoadScene("Panels");
        }
    }
}
