using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField] private SpacebarControl[] generalControls;
    [SerializeField] private ArrowControl[] navControls;

    public void UpdateUI(int showIndex = -1, int hideIndex = -1)
    {
        if (showIndex >= 0)
            StartCoroutine(generalControls[showIndex].RevealControl());

        if (hideIndex >= 0)
            StartCoroutine(generalControls[hideIndex].HideControl());
    }

    public void UpdateNavigationUI(int showIndex = -1, int hideIndex = -1)
    {
        if (showIndex >= 0)
            StartCoroutine(navControls[showIndex].RevealControl());

        if (hideIndex >= 0)
            StartCoroutine(navControls[hideIndex].HideControl());
    }
}
