using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuControl : MonoBehaviour, IControls
{
    [SerializeField] private Image controlImg;
    [SerializeField] private TextMeshProUGUI controlTxt, imgTxt;

    [SerializeField] private int index;

    public IEnumerator RevealControl(bool onlyPart)
    {
        float startTime = 0.0f, time = 0.0f, totalTime = 0.25f;
        Color c, t = controlTxt.color, ct;

        if (onlyPart)
        {
            c = new Color (0, 0, 0, 0);
            ct = new Color (0, 0, 0, 0);
        }
        else
        {
            c = controlImg.color;
            ct = imgTxt.color;
        }
        
        do
        {
            startTime += Time.deltaTime;

            if (startTime > totalTime * 4f)
            {
                time += Time.deltaTime;

                t.a = Mathf.Clamp01(time / totalTime);
                controlTxt.color = t;

                if (!onlyPart)
                {
                    c.a = Mathf.Clamp01(time / totalTime);
                    controlImg.color = c;
                    ct.a = Mathf.Clamp01(time / totalTime);
                    imgTxt.color = ct;
                }
            }
            
            yield return null;
        }
        while(time < totalTime);
    }

    public IEnumerator HideControl(bool onlyPart)
    {
        float time = 0.0f, totalTime = 0.25f;
        Color c, t = controlTxt.color, ct;

        if (onlyPart)
        {
            c = new Color (0, 0, 0, 0);
            ct = new Color (0, 0, 0, 0);
        }
        else
        {
            c = controlImg.color;
            ct = imgTxt.color;
        }

        if (controlImg.color.a != 0)
        {
            do
            {
                time += Time.deltaTime;

                t.a = 1.0f - Mathf.Clamp01(time / totalTime);
                controlTxt.color = t;
                
                if (!onlyPart)
                {
                    c.a = 1.0f - Mathf.Clamp01(time / totalTime);
                    controlImg.color = c;
                    ct.a = 1.0f - Mathf.Clamp01(time / totalTime);
                    imgTxt.color = ct;
                }

                yield return null;
            }
            while(time < totalTime);
        }
    }
}
