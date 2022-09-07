using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private GameObject controllerObj;
    private IController controller;

    private Collider2D coinCollider;

    [SerializeField] private AudioSource coinAudio;

    [SerializeField] private float scale = 1.0f;

    [SerializeField] float updateValue = 1;

    private bool collected;

    // Start is called before the first frame update
    private void Start()
    {
        controllerObj = GameObject.Find("Controller");
        controller = controllerObj.GetComponent<IController>();

        coinCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (collected)
        {
            scale -= updateValue * Time.deltaTime;

            if (scale < 0) scale = 0;


            gameObject.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            collected = true;
            controller.CollectCoin();

            coinAudio.Play();

            coinCollider.enabled = false;

            Destroy(gameObject, 1.0f);
        }
    }
}
