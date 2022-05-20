using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelNavigation : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Move to panel");
        }
    }
}
