using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private string playerMask;

    [SerializeField] private bool isOnGoal;

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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isOnGoal && Input.GetButtonUp("Select"))
        {
            SceneManager.LoadScene("Panels");
        }
    }
}
