using UnityEngine;

public class AgentEndPoint : MonoBehaviour
{
    private AgentGenerator controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<AgentGenerator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Background Character")
        {
            controller.RemoveAgent(other.GetComponent<MobileAgent>());
            Destroy(other.gameObject);
        }
    }
}
