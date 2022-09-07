using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPoint : MonoBehaviour
{
    [SerializeField] private GameObject controllerObj;
    private IController controller;

    private BoxCollider2D eventCollider;

    [SerializeField] private int eventIndex;
    [SerializeField] private float resetTime = 0;

    [SerializeField] private string[] playerDialogue, followerDialogue, otherDialogue;

    [SerializeField] private bool hasDialogue, hasThought, repeatable;

    private bool interacted;

    // Start is called before the first frame update
    private void Start()
    {
        controllerObj = GameObject.Find("Controller");
        controller = controllerObj.GetComponent<IController>();

        eventCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!hasThought)
            {
                if (!interacted)
                {
                    if (hasDialogue)
                        controller.SetDialogue(
                            playerDialogue, followerDialogue, otherDialogue);

                    controller.BeginEvent(eventIndex);
                }

                interacted = true;
            }
            else
            {
                controller.SetThought(playerDialogue[0]);

                controller.BeginEvent(eventIndex);
            }

            
            if (!repeatable)
                gameObject.SetActive(false);
            else
            {
                eventCollider.enabled = false;
                StartCoroutine(ResetPoint());
            }
        }
    }

    private IEnumerator ResetPoint()
    {
        yield return new WaitForSeconds(resetTime);
        eventCollider.enabled = true;
    }
}
