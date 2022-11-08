using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioLeveler : MonoBehaviour
{
    [SerializeField] private AudioMixer level;

    [SerializeField] private Player player;

    [SerializeField] private float lowPos, highPos;

    private void Start()
    {
        StartCoroutine(AdjustVolume("Master Volume", -60, 0, 30));
    }

    private void Update()
    {
        if (player != null)
        {
            float playerPos = player.gameObject.transform.position.y;

            if (playerPos >= lowPos && playerPos <= highPos)
            {
                level.SetFloat("Low Volume", -playerPos + lowPos);
                level.SetFloat("High Volume", -highPos + playerPos);
            }
        }    
    }

    public IEnumerator AdjustVolume(
        string param, float startVolume, float endVolume, float adjustTime, float delay = 0f)
    {
        float volume = startVolume;

        if (delay != 0)
            yield return new WaitForSeconds(delay);

        if (volume > endVolume)
        {
            do
            {
                volume -= adjustTime * Time.deltaTime;
                level.SetFloat(param, volume);
                yield return null;
            }
            while(volume >= endVolume);
        }
        else
        {
            do
            {
                volume += adjustTime * Time.deltaTime;
                level.SetFloat(param, volume);
                yield return null;
            }
            while(volume <= endVolume);
        }
    }
}
