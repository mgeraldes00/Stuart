using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private MenuControl[] generalControls;
    [SerializeField] private MenuControl[] navControls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI(int[] showIndex, int[] hideIndex)
    {
        for (int i = 0; i < showIndex.Length; i++)
            StartCoroutine(generalControls[showIndex[i]].RevealControl());
        for (int i = 0; i < hideIndex.Length; i++)
            StartCoroutine(generalControls[hideIndex[i]].HideControl());
    }
}
