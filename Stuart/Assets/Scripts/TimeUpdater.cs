using UnityEngine;

public class TimeUpdater : MonoBehaviour
{
    [SerializeField] private FloatValue timeValue;
    [SerializeField, Range(0.0f, 4.0f)]
    private float timeScale = 1;

    private void Update()
    {
        timeValue.ChangeValue(-Time.deltaTime * timeScale);

        if (timeValue.GetValue() < 0)
        {
            timeValue.SetValue(0);
        }
    }

    public void SetScale(float s)
    {
        timeScale = s;
    }
}
