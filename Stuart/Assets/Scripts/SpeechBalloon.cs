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
    
    [SerializeField] private string[] content;

    [SerializeField] private int nextLine;

    [SerializeField] private float delay = 0.01f;

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
        text.text = "";
        
        StartCoroutine(ShowText());
    }

    public void HideBalloon()
    {
        mask.SetBool("Talking", false);
    }

    private IEnumerator ShowText()
    {
        string currentLine = "";
        
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < content[nextLine].Length; i++)
        {
            currentLine = content[nextLine].Substring(0, i);
            text.text = currentLine;
            yield return new WaitForSeconds(delay);
        }

        nextLine++;
    }
}
