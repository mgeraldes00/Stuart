using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurControl : MonoBehaviour
{
    private Player player;

    [SerializeField] private Material material;

    [SerializeField] private float blurAmount;

    [SerializeField] private float maxBlur;
    [SerializeField] private float blurSpeed = 3.5f;

    [SerializeField] private bool blurActive;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (blurActive)
            blurAmount += blurSpeed * Time.deltaTime;
        else
            blurAmount -= blurSpeed * Time.deltaTime;

        blurAmount = Mathf.Clamp(blurAmount, 0, maxBlur);
        material.SetFloat("_BlurAmount", blurAmount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            blurActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            blurActive = false;
        }
    }
}
