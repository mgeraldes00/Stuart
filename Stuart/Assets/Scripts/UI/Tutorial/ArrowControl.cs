using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArrowControl : MonoBehaviour, IControls
{
    [SerializeField] private Image[] controlImg, keyImg;
    [SerializeField] private TextMeshProUGUI controlTxt;

    [SerializeField] private int index;

    public IEnumerator RevealControl(bool onlyPart = false)
    {
        float startTime = 0.0f, time = 0.0f, totalTime = 0.25f;
        Color[] c = new Color[controlImg.Length];
        Color[] ct = new Color[controlImg.Length];
        Color t = controlTxt.color;

        for (int i = 0; i < controlImg.Length; i++)
        {
            c[i] = controlImg[i].color;
            ct[i] = keyImg[i].color;
        }
        
        do
        {
            startTime += Time.deltaTime;

            if (startTime > totalTime * 4f)
            {
                time += Time.deltaTime;

                t.a = Mathf.Clamp01(time / totalTime);
                controlTxt.color = t;

                for (int i = 0; i < controlImg.Length; i++)
                {
                    c[i].a = Mathf.Clamp01(time / totalTime);
                    controlImg[i].color = c[i];
                    ct[i].a = Mathf.Clamp01(time / totalTime);
                    keyImg[i].color = ct[i];
                }
            }
            
            yield return null;
        }
        while(time < totalTime);
    }

    public IEnumerator HideControl(bool onlyPart = false)
    {
        float time = 0.0f, totalTime = 0.25f;
        Color[] c = new Color[controlImg.Length];
        Color[] ct = new Color[controlImg.Length];
        Color t = controlTxt.color;

        for (int i = 0; i < controlImg.Length; i++)
        {
            c[i] = controlImg[i].color;
            ct[i] = keyImg[i].color;
        }

        do
        {
            time += Time.deltaTime;

            t.a = 1.0f - Mathf.Clamp01(time / totalTime);
            controlTxt.color = t;
            
            for (int i = 0; i < controlImg.Length; i++)
            {
                c[i].a = 1.0f - Mathf.Clamp01(time / totalTime);
                controlImg[i].color = c[i];
                ct[i].a = 1.0f - Mathf.Clamp01(time / totalTime);
                keyImg[i].color = ct[i];
            }

            yield return null;
        }
        while(time < totalTime);
    }
}
