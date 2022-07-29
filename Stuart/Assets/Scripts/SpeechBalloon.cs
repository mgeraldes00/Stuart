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

    [SerializeField] private bool active, speaking;

    // Start is called before the first frame update
    private void Start()
    {
        mask = GetComponent<Animator>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (active)
            text.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void DefineDialogue(string[] lines)
    {
        nextLine = 0;

        content = lines;
    }

    public void DefineThought(string line)
    {
        nextLine = 0;

        content = new string[1];
        content[0] = line;
    }

    public void ShowThoughtBalloon()
    {
        mask.SetBool("Talking", true);
        active = true;

        //text.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        text.text = "";

        nextLine++;
        currentIndex = nextLine - 1;

        StartCoroutine(ShowThought());
    }

    public void ShowDialogueBalloon()
    {
        mask.SetBool("Talking", true);
        active = true;

        //text.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        text.text = "";

        nextLine++;
        currentIndex = nextLine - 1;
        
        StartCoroutine(ShowSpeech());
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

        active = false;
    }

    private IEnumerator ShowThought()
    {
        string currentLine = "";
        
        yield return new WaitForSeconds(0.2f);

        speaking = true;

        for (int i = 0; i < content[currentIndex].Length; i++)
        {
            currentLine = content[currentIndex].Substring(0, i);
            text.text = currentLine;
            yield return new WaitForSeconds(delay);
        }

        speaking = false;
    }

    private IEnumerator ShowSpeech()
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
