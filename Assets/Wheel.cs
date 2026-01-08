using UnityEngine;

public class Wheel : MonoBehaviour
{
    Transform tireTransform;

    [Header("References")]
    public Rigidbody carRigidBody;
    public Transform carTransform;

    [Header("Suspension")]
    public float springStrength = 4000f;
    public float springDamper = 600f;
    public float suspensionRestDist = 0.5f;
    public float maxSpringForce = 8000f;

    [Header("Grip")]
    public float tireGripFactor = 0.7f;
    public float rearGripMultiplier = 1.4f;
    public float tireMass = 20f;
    public float maxGripForce = 6000f;

    [Header("Engine")]
    public float accelInput;
    public float carTopSpeed = 60f;
    public AnimationCurve powerCurve;
    public float enginePower = 30000f;

    [Header("Wheel Type")]
    public bool isSteerWheel = false;

    void Start()
    {
        tireTransform = transform;
    }

    void FixedUpdate()
    {
        Vector3 springDir = tireTransform.up;

        // ---------- RAYCAST ----------
        RaycastHit hit;
        bool grounded = Physics.Raycast(
            tireTransform.position,
            -springDir,
            out hit,
            suspensionRestDist
        );

        if (!grounded) return;

        Vector3 tireWorldVel =
            carRigidBody.GetPointVelocity(tireTransform.position);

        // ---------- SUSPENSION ----------

        float offset = suspensionRestDist - hit.distance;

        float vel =
            Vector3.Dot(springDir, tireWorldVel);

        float springForce =
            (offset * springStrength) -
            (vel * springDamper);

        springForce = Mathf.Clamp(
            springForce,
            -maxSpringForce,
            maxSpringForce
        );

        carRigidBody.AddForceAtPosition(
            springDir * springForce,
            tireTransform.position
        );

        // ---------- LATERAL GRIP ----------

        Vector3 sideDir = tireTransform.right;

        float sideVel =
            Vector3.Dot(sideDir, tireWorldVel);

        float grip = tireGripFactor;

        // rear wheels stabilize the car
        if (!isSteerWheel)
            grip *= rearGripMultiplier;

        float desiredVelChange =
            -sideVel * grip;

        float desiredAccel =
            desiredVelChange / Time.fixedDeltaTime;

        float gripForce =
            tireMass * desiredAccel;

        gripForce = Mathf.Clamp(
            gripForce,
            -maxGripForce,
            maxGripForce
        );

        carRigidBody.AddForceAtPosition(
            sideDir * gripForce,
            tireTransform.position
        );

        // ---------- ENGINE FORCE ----------

        Vector3 forwardDir = tireTransform.forward;

        if (Mathf.Abs(accelInput) > 0.01f)
        {
            float carSpeed =
                Vector3.Dot(
                    carTransform.forward,
                    carRigidBody.linearVelocity
                );

            float normalizedSpeed =
                Mathf.Clamp01(
                    Mathf.Abs(carSpeed) / carTopSpeed
                );

            float torque =
                powerCurve.Evaluate(normalizedSpeed) *
                accelInput *
                enginePower;

            carRigidBody.AddForceAtPosition(
                forwardDir * torque,
                tireTransform.position
            );
        }

        // ---------- AUTO BRAKE ----------

        if (Mathf.Abs(accelInput) < 0.01f)
        {
            float forwardVel =
                Vector3.Dot(
                    forwardDir,
                    tireWorldVel
                );

            float brakeStrength = 900f;

            float brakeForce =
                -forwardVel * brakeStrength;

            carRigidBody.AddForceAtPosition(
                forwardDir * brakeForce,
                tireTransform.position
            );
        }
    }
}
