using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeechBalloon : MonoBehaviour
{
    [SerializeField] private Transform agent;

    [SerializeField] private Animator mask, spriteAnim;

    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private AudioSource sound;
    
    [SerializeField] private string[] content;

    [SerializeField] private int nextLine, currentIndex;

    [SerializeField] private float delay = 0.01f;

    [SerializeField] private bool speaking;

    // Start is called before the first frame update
    void Start()
    {
        mask = GetComponent<Animator>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void DefineDialogue(string[] lines)
    {
        nextLine = 0;

        content = lines;
    }

    public void ShowBalloon()
    {
        mask.SetBool("Talking", true);

        text.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        text.text = "";

        nextLine++;
        currentIndex = nextLine - 1;
        
        StartCoroutine(ShowText());
        spriteAnim.SetBool("talking", true);
    }

    public void HideBalloon()
    {
        mask.SetBool("Talking", false);

        if (speaking)
        {
            sound.Stop();
            speaking = false;
            StopAllCoroutines();
            spriteAnim.SetBool("talking", false);
        }
    }

    private IEnumerator ShowText()
    {
        string currentLine = "";
        
        yield return new WaitForSeconds(0.2f);

        sound.Play();
        speaking = true;

        for (int i = 0; i < content[currentIndex].Length; i++)
        {
            currentLine = content[currentIndex].Substring(0, i);
            text.text = currentLine;
            yield return new WaitForSeconds(delay);
        }

        sound.Stop();
        speaking = false;
        spriteAnim.SetBool("talking", false);
    }
}
