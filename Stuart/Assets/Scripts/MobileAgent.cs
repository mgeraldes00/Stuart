using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileAgent : MonoBehaviour
{
    [SerializeField] private Material foregroundMaterial;

    private SpriteRenderer sr;

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

        sr.material = foregroundMaterial;
        sr.sortingOrder += 2;
    }
}
