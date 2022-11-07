using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField] private SpacebarControl[] generalControls;
    [SerializeField] private ArrowControl[] navControls;

    public void UpdateUI(int showIndex, int hideIndex = -1)
    {
        StartCoroutine(generalControls[showIndex].RevealControl());

        if (hideIndex >= 0)
            StartCoroutine(generalControls[hideIndex].HideControl());
    }

    public void UpdateNavigationUI(int showIndex, int hideIndex = -1)
    {
        StartCoroutine(navControls[showIndex].RevealControl());

        if (hideIndex >= 0)
            StartCoroutine(navControls[hideIndex].HideControl());
    }
}
