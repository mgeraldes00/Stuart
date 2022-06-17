using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform sourceTransform;
    [SerializeField] private Vector2 moveScale = Vector2.one;

    private Vector3 offset;

    void Start()
    {
        sourceTransform = Camera.main.transform;

        offset = new Vector3(
            transform.position.x - sourceTransform.position.x * moveScale.x,
            transform.position.y - sourceTransform.position.y * moveScale.y,
            transform.position.z - sourceTransform.position.z);
    }

    void Update()
    {
        Vector3 currentPos = new Vector3();
        currentPos.x = sourceTransform.position.x * moveScale.x + offset.x;
        currentPos.y = sourceTransform.position.y * moveScale.y + offset.y;
        currentPos.z = sourceTransform.position.z + offset.z;
        transform.position = currentPos;
    }
}
