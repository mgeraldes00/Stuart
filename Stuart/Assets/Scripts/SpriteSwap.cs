using System.Collections;
using UnityEngine;
using Rand = UnityEngine.Random;

public class SpriteSwap : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private float startTime, swapTime;
    [SerializeField] private int count;

    [SerializeField] private AudioSource[] animAudio;

    private SpriteRenderer objectRenderer;
    private Sprite currentSprite;

    private void Start()
    {
        objectRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(SwapLoop());
    }
    
    private void RandomizeWaitTime()
    {
        startTime = Rand.Range(1.0f, 3.0f);
    }

    private IEnumerator SwapLoop()
    {
        int nextInLine = 0, currentCount = 0;

        do 
        {
            if (count != 0 && currentCount > count)
            {
                yield return new WaitForSeconds(startTime);
                RandomizeWaitTime();
                currentCount = 0;
            }
            
            currentSprite = sprites[nextInLine];
            if (currentCount < count) animAudio[nextInLine].Play();
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
