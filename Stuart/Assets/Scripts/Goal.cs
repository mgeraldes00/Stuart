using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private AudioLeveler audioCtrl;

    [SerializeField] private Animator sceneMask;

    [SerializeField] private string playerMask;

    [SerializeField] private bool isOnGoal;

    [SerializeField] private int levelIndex;

    private void Start()
    {
        audioCtrl = FindObjectOfType<AudioLeveler>();

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
            StartCoroutine(audioCtrl.AdjustVolume("Master Volume", 0, -60, 30));
            StartCoroutine(EndLevel());
        }
    }

    private IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(0.5f);
        sceneMask.SetTrigger("endScene");

        yield return new WaitForSeconds(2.5f);
        PlayerPrefs.SetInt("IsLastPanelPlayed", 1);
        if (PlayerPrefs.GetInt("MaxPanelReached") < levelIndex)
            PlayerPrefs.SetInt("MaxPanelReached", levelIndex);
        SceneManager.LoadScene("Panels");
    }
}
