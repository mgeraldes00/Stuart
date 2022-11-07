using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField] private SpacebarControl[] generalControls;
    [SerializeField] private ArrowControl[] navControls;

    public void UpdateUI(int[] showIndex, int[] hideIndex)
    {
        for (int i = 0; i < showIndex.Length; i++)
            StartCoroutine(generalControls[showIndex[i]].RevealControl());
        for (int i = 0; i < hideIndex.Length; i++)
            StartCoroutine(generalControls[hideIndex[i]].HideControl());
    }

    public void UpdateNavigationUI(int[] showIndex, int[] hideIndex)
    {
        for (int i = 0; i < showIndex.Length; i++)
            StartCoroutine(navControls[showIndex[i]].RevealControl());
        for (int i = 0; i < hideIndex.Length; i++)
            StartCoroutine(navControls[hideIndex[i]].HideControl());
    }
}
