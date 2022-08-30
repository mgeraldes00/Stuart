using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEffect : MonoBehaviour
{
    private CameraCtrl cam;
    private Player player;

    [SerializeField] private float offsetValue = 0;

    [SerializeField] private bool edge;

    [SerializeField] private int side;

    private void Start()
    {
        if (offsetValue != 0)
            cam = FindObjectOfType<CameraCtrl>();

        if (edge)
            player = FindObjectOfType<Player>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (offsetValue != 0)
                cam.OffsetY = offsetValue;
        }
    }

    /*private void OnTriggerStay2D(Collider2D other)
    {   
        if (edge)
        {
            if (other.tag == "Player")
                player.SetBalancing(side);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (edge)
        {
            if (other.tag == "Player")
                player.SetBalancing();
        }
    }*/
}
