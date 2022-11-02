using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuStart : MonoBehaviour, IControls
{
    [SerializeField] private Image controlImg;
    [SerializeField] private TextMeshProUGUI imgTxt;

    [SerializeField] private int index;

    public IEnumerator RevealControl(bool onlyPart = false)
    {
        Debug.Log("Reveal start control");

        float startTime = 0.0f, time = 0.0f, totalTime = 0.25f;
        Color c = controlImg.color, ct = imgTxt.color;
        
        do
        {
            startTime += Time.deltaTime;

            if (startTime > totalTime * 10f)
            {
                time += Time.deltaTime;

                c.a = Mathf.Clamp01(time / totalTime);
                controlImg.color = c;
                ct.a = Mathf.Clamp01(time / totalTime);
                imgTxt.color = ct;
            }
            
            yield return null;
        }
        while(time < totalTime);
    }

    public IEnumerator HideControl(bool onlyPart = false)
    {
        float time = 0.0f, totalTime = 0.25f;
        Color c = controlImg.color, ct = imgTxt.color;;

        if (controlImg.color.a != 0)
        {
            do
            {
                time += Time.deltaTime;

                c.a = 1.0f - Mathf.Clamp01(time / totalTime);
                controlImg.color = c;
                ct.a = 1.0f - Mathf.Clamp01(time / totalTime);
                imgTxt.color = ct;

                yield return null;
            }
            while(time < totalTime);
        }
    }
}
