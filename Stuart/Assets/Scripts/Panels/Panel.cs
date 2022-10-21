using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel : MonoBehaviour
{
    [SerializeField] private int index;
    public int Index => index;

    [SerializeField] private int currentPanel;
    public int CurrentPanel => currentPanel;

    [SerializeField] private GameObject[] images;
    public GameObject[] Images => images;

    [SerializeField] private TextMeshProUGUI[] texts;

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
    public void SetImage(bool reveal)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (reveal)
                images[i].GetComponent<SpriteRenderer>().color =
                    new Color(255, 255, 255, 255);
            else
                images[i].GetComponent<SpriteRenderer>().color =
                    new Color(255, 255, 255, 0);
        }

        for (int i = 0; i < texts.Length; i++)
        {   
            if (reveal)
                texts[i].color = new Color(0, 0, 0, 255);
            else
                texts[i].color = new Color(0, 0, 0, 0);
        }
    }
}
