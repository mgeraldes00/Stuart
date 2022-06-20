using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPoint : MonoBehaviour
{
    [SerializeField] private GameObject controller;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GameObject.Find("Controller");
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
