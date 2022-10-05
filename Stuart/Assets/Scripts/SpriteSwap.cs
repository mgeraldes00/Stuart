using System.Collections;
using UnityEngine;

public class SpriteSwap : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private float startTime, swapTime;
    [SerializeField] int count;

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
        int nextInLine = 0, currentCount = 0;

        do 
        {
            if (count != 0 && currentCount > count)
            {
                yield return new WaitForSeconds(startTime);
                currentCount = 0;
            }

            currentSprite = sprites[nextInLine];
            objectRenderer.sprite = currentSprite;
            yield return new WaitForSeconds(swapTime);
            nextInLine++;
            if (count != 0) currentCount++;
            if (nextInLine >= sprites.Length)
                nextInLine = 0;
        }
        while (true);
    }
}
