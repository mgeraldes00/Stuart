using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    [SerializeField] private int index;
    public int Index => index;

    [SerializeField] private int currentPanel;
    public int CurrentPanel => currentPanel;

    [SerializeField] private GameObject[] images;
    public GameObject[] Images => images;

    private Camera cam;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void ResetCurrentPanel() { currentPanel = 0; }

    public Vector3 SelectImage(bool goingUp)
    {
        if (goingUp) currentPanel--;
        else currentPanel++;

        return images[currentPanel].transform.position;
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
