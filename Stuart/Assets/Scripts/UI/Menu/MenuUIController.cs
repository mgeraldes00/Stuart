using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private MenuControl[] generalControls;
    [SerializeField] private MenuNavigator[] navControls;

    public void UpdateUI(
        int[] showIndex, int[] hideIndex, bool onlyText = false)
    {
        for (int i = 0; i < showIndex.Length; i++)
            StartCoroutine(generalControls[showIndex[i]].RevealControl(onlyText));
        for (int i = 0; i < hideIndex.Length; i++)
            StartCoroutine(generalControls[hideIndex[i]].HideControl(onlyText));
    }

    public void UpdateNavigationlUI(
        int[] showIndex, int[] hideIndex)
    {
        for (int i = 0; i < showIndex.Length; i++)
            StartCoroutine(navControls[showIndex[i]].RevealControl());
        for (int i = 0; i < hideIndex.Length; i++)
            StartCoroutine(navControls[hideIndex[i]].HideControl());
    }
}
