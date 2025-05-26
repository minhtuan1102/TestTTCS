using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle2D : MonoBehaviour
{
    public Light2D globalLight;
    public Gradient lightColorOverTime;
    public AnimationCurve intensityOverTime;

    static public int currentDay = 1;
    static public int hour = 0;
    static public int minute = 0;

    [Range(0f, 24f)] public float timeOfDay = 6f;
    public float dayDuration = 60f; // 1 ngày kéo dài 60 giây

    void Update()
    {
        timeOfDay += (24f / dayDuration) * Time.deltaTime;
        if (timeOfDay >= 24f)
        {
            timeOfDay = 0f;
            currentDay++;
        }

        hour = Mathf.FloorToInt(timeOfDay);
        minute = Mathf.FloorToInt((timeOfDay - hour) * 60f);

        float percent = timeOfDay / 24f;

        // Đổi màu ánh sáng
        globalLight.color = lightColorOverTime.Evaluate(percent);
        globalLight.intensity = intensityOverTime.Evaluate(percent);
    }
}
