using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurControl : MonoBehaviour
{
    private Player player;

    [SerializeField] private Material material;

    [SerializeField] private float blurAmount;
    [SerializeField] private float blurSpeed = 3.5f;

    [SerializeField] private bool blurActive;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (player.transform.position.y < 21.5f)
            blurActive = true;
        else
            blurActive = false;

        if (blurActive)
            blurAmount += blurSpeed * Time.deltaTime;
        else
            blurAmount -= blurSpeed * Time.deltaTime;

        blurAmount = Mathf.Clamp(blurAmount, 0, 0.2f);
        material.SetFloat("_BlurAmount", blurAmount);
    }
}
