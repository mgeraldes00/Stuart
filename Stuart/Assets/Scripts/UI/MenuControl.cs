using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuControl : MonoBehaviour, IControls
{
    [SerializeField] private Image controlImg;
    [SerializeField] private TextMeshProUGUI controlTxt, imgTxt;

    [SerializeField] private int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator RevealControl()
    {
        Debug.Log($"Reveal icon {index}");

        float startTime = 0.0f, time = 0.0f, totalTime = 0.25f;
        Color c = controlImg.color, t = controlTxt.color, ct = imgTxt.color;

        do
        {
            startTime += Time.deltaTime;

            if (startTime > totalTime * 4f)
            {
                time += Time.deltaTime;
                c.a = Mathf.Clamp01(time / totalTime);
                t.a = Mathf.Clamp01(time / totalTime);
                ct.a = Mathf.Clamp01(time / totalTime);
                controlImg.color = c;
                controlTxt.color = t;
                imgTxt.color = ct;
            }
            
            yield return null;
        }
        while(time < totalTime);
    }

    public IEnumerator HideControl()
    {
        Debug.Log($"Hide icon {index}");

        float time = 0.0f, totalTime = 0.25f;
        Color c = controlImg.color, t = controlTxt.color, ct = imgTxt.color;

        do
        {
            time += Time.deltaTime;
            c.a = 1.0f - Mathf.Clamp01(time / totalTime);
            t.a = 1.0f - Mathf.Clamp01(time / totalTime);
            ct.a = 1.0f - Mathf.Clamp01(time / totalTime);
            controlImg.color = c;
            controlTxt.color = t;
            imgTxt.color = ct;
            yield return null;
        }
        while(time < totalTime);
    }
}
