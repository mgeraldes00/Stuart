using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEffect : MonoBehaviour
{
    private CameraCtrl cam;

    [SerializeField] private float offsetValue;

    private void Start()
    {
        cam = FindObjectOfType<CameraCtrl>();        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            cam.OffsetY = offsetValue;
        }
    }
}
