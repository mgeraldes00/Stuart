using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    [SerializeField] private int index;
    public int Index => index;

    [SerializeField] private GameObject[] images;

    private Camera cam;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Change later to hide default image and replace with illustration
    public void SetImage(Color newColor)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].GetComponent<SpriteRenderer>().color = newColor;
        }
    }
}
