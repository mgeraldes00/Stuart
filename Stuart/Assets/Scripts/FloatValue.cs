using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatValue", menuName = "Custom/Float Value")]
public class FloatValue : ScriptableObject
{
    [SerializeField] private float value;

    public void SetValue(float v)
    {
        value = v;
    }

    public void ChangeValue(float deltaValue)
    {
        value += deltaValue;
    }

    public float GetValue()
    {
        return value;
    }
}
