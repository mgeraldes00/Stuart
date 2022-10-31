using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuControl : MonoBehaviour, IControls
{
    [SerializeField] private Image controlImg;
    [SerializeField] private TextMeshProUGUI controlTxt;

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
        float time = 0.0f, totalTime = 1.0f;
        Color c = controlImg.color, t = controlTxt.color;

        do
        {
            time += Time.deltaTime;
            c.a = 1.0f - Mathf.Clamp01(time / totalTime);
            t.a = 1.0f - Mathf.Clamp01(time / totalTime);
            controlImg.color = c;
            controlTxt.color = t;
            yield return null;
        }
        while(time < totalTime);
    }

    public IEnumerator HideControl()
    {
        float time = 0.0f, totalTime = 1.0f;
        Color c = controlImg.color, t = controlTxt.color;

        do
        {
            time += Time.deltaTime;
            c.a = 0.0f - Mathf.Clamp01(time / totalTime);
            t.a = 0.0f - Mathf.Clamp01(time / totalTime);
            controlImg.color = c;
            controlTxt.color = t;
            yield return null;
        }
        while(time < totalTime);
    }
}
