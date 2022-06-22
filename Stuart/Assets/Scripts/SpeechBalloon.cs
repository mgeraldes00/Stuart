using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeechBalloon : MonoBehaviour
{
    [SerializeField] private Transform agent;

    [SerializeField] private Animator mask;

    [SerializeField] private TextMeshProUGUI text;
    

    // Start is called before the first frame update
    void Start()
    {
        mask = GetComponent<Animator>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowBalloon()
    {
        mask.SetBool("Talking", true);

        text.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void HideBalloon()
    {
        mask.SetBool("Talking", false);
    }
}
