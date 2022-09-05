using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwap : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private float swapTime;

    private SpriteRenderer objectRenderer;
    private Sprite currentSprite;

    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(SwapLoop());
    }

    private IEnumerator SwapLoop()
    {
        int nextInLine = 0;

        do 
        {
            currentSprite = sprites[nextInLine];
            objectRenderer.sprite = currentSprite;
            yield return new WaitForSeconds(swapTime);
            nextInLine++;
            if (nextInLine >= sprites.Length)
                nextInLine = 0;
        }
        while (true);
    }
}
