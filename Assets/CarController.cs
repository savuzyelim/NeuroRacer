using UnityEngine;

public class CarController : MonoBehaviour
{
    public Wheel[] wheels;

    [Header("Input")]
    public float accel;
    public float steer;

    [Header("Steering")]
    public float maxSteerAngle = 30f;

    void Update()
    {
        accel = Input.GetAxis("Vertical");   // W/S
        steer = Input.GetAxis("Horizontal"); // A/D

    }

    void FixedUpdate()
    {
        foreach (Wheel w in wheels)
        {
            w.accelInput = accel;
        }

        // steer front wheels only
        ApplySteering();
    }

    void ApplySteering()
    {
        float steerAngle = steer * maxSteerAngle;

        foreach (Wheel w in wheels)
        {
            if (!w.isSteerWheel) continue;

            Vector3 euler = w.transform.localEulerAngles;
            euler.y = steerAngle;
            w.transform.localEulerAngles = euler;
        }
    }
}
