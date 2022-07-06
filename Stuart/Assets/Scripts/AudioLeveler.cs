using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioLeveler : MonoBehaviour
{
    [SerializeField] private AudioMixer level;

    [SerializeField] private Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float playerPos = player.gameObject.transform.position.y;

        if (playerPos >= 0 && playerPos <= 20)
        {
            level.SetFloat("Low Volume", -playerPos);
            level.SetFloat("High Volume", -20 + playerPos);
        }
    }
}
