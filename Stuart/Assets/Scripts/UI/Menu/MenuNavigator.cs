using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigator : MonoBehaviour, IControls
{
    [SerializeField] private Image controlImg;

    [SerializeField] private int index;

    public IEnumerator RevealControl(bool onlyPart = false)
    {
        float startTime = 0.0f, time = 0.0f, totalTime = 0.25f;
        Color c = controlImg.color;
        
        if (controlImg.color.a != 1)
        {
            do
            {
                startTime += Time.deltaTime;

                if (startTime > totalTime * 4f)
                {
                    time += Time.deltaTime;

                    c.a = Mathf.Clamp01(time / totalTime);
                    controlImg.color = c;
                }
                
                yield return null;
            }
            while(time < totalTime);
        }
        
    }

    public IEnumerator HideControl(bool onlyPart = false)
    {
        float time = 0.0f, totalTime = 0.25f;
        Color c = controlImg.color;

        if (controlImg.color.a != 0)
        {
            do
            {
                time += Time.deltaTime;

                c.a = 1.0f - Mathf.Clamp01(time / totalTime);
                controlImg.color = c;

                yield return null;
            }
            while(time < totalTime);
        }
    }
}
