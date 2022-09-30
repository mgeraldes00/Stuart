using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileAgent : MonoBehaviour
{
    [SerializeField] private Material foregroundMaterial;

    private SpriteRenderer sr;

    private bool isInverted = false, isOnForeground = false;
    public bool IsInverted => isInverted;
    public bool IsOnForeground => isOnForeground;

    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnForeground()
    {
        isOnForeground = true;

        sr.material = foregroundMaterial;
        sr.sortingOrder += 2;
    }

    public void CheckOrientation(bool facingRight)
    {
        if (!facingRight)
            isInverted = true;
    }
}
