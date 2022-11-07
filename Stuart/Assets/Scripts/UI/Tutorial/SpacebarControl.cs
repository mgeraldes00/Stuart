using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpacebarControl : MonoBehaviour, IControls
{
    [SerializeField] private Image controlImg;
    [SerializeField] private TextMeshProUGUI controlTxt;

    [SerializeField] private int index;

    public IEnumerator RevealControl(bool onlyPart = false)
    {
        float startTime = 0.0f, time = 0.0f, totalTime = 0.25f;
        Color c = controlImg.color, t = controlTxt.color;

        do
        {
            startTime += Time.deltaTime;

            if (startTime > totalTime * 4f)
            {
                time += Time.deltaTime;

                t.a = Mathf.Clamp01(time / totalTime);
                controlTxt.color = t;
                c.a = Mathf.Clamp01(time / totalTime);
                controlImg.color = c;
            }
            
            yield return null;
        }
        while(time < totalTime);
    }

    public IEnumerator HideControl(bool onlyPart = false)
    {
        float time = 0.0f, totalTime = 0.25f;
        Color c = controlImg.color, t = controlTxt.color;

        do
        {
            time += Time.deltaTime;

            t.a = 1.0f - Mathf.Clamp01(time / totalTime);
            controlTxt.color = t;
            c.a = 1.0f - Mathf.Clamp01(time / totalTime);
            controlImg.color = c;

            yield return null;
        }
        while(time < totalTime);
    }
}
