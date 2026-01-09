using UnityEngine;

public class Wheel : MonoBehaviour
{
    Transform tireTransform;

    [Header("References")]
    public Rigidbody carRigidBody;
    public Transform carTransform;

    [Header("Suspension")]
    public float springStrength = 15000f;
    public float springDamper = 2000f;
    public float suspensionRestDist = 0.5f;
    public float maxSpringForce = 16000f;

    [Header("Grip")]
    public float tireGripFactor = 0.8f;
    public float rearGripMultiplier = 1.4f;
    public float tireMass = 20f;
    public float maxGripForce = 8000f;

    [Header("Engine")]
    public float accelInput;   // W = 1 , S = -1
    public float carTopSpeed = 60f;
    public AnimationCurve powerCurve;
    public float enginePower = 30000f;

    [Header("Wheel Type")]
    public bool isSteerWheel = false;

    [Header("Raycast")]
    public LayerMask groundLayer;

    private float forceHeightOffset = 0.1f;

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
            suspensionRestDist,
            groundLayer
        );

        if (!grounded) return;

        Vector3 tireWorldVel =
            carRigidBody.GetPointVelocity(tireTransform.position);

        // ---------- SUSPENSION ----------

        float offset = suspensionRestDist - hit.distance;
        float vel = Vector3.Dot(springDir, tireWorldVel);

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

        // ---------- FORCE POINT ----------
        Vector3 forcePoint = tireTransform.position;
        forcePoint.y =
            carRigidBody.worldCenterOfMass.y -
            forceHeightOffset;

        // ---------- LATERAL GRIP (ALWAYS ACTIVE) ----------

        Vector3 sideDir = tireTransform.right;
        float sideVel =
            Vector3.Dot(sideDir, tireWorldVel);

        float grip = tireGripFactor;

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
            forcePoint
        );

        // ---------- ENGINE FORCE ----------

        Vector3 forwardDir =
            tireTransform.forward;

        if (accelInput > 0.05f)
        {
            float carSpeed =
                Vector3.Dot(
                    carTransform.forward,
                    carRigidBody.linearVelocity
                );

            float normalizedSpeed =
                Mathf.Clamp01(
                    Mathf.Abs(carSpeed) /
                    carTopSpeed
                );

            float torque =
                powerCurve.Evaluate(normalizedSpeed) *
                accelInput *
                enginePower;

            carRigidBody.AddForce(
                forwardDir * torque
            );
        }

        // ---------- BRAKE (ONLY WHEN S PRESSED) ----------

        if (accelInput < -0.1f)
        {
            float forwardVel =
                Vector3.Dot(
                    forwardDir,
                    tireWorldVel
                );

            float brakeStrength = 1200f;

            float brakeForce =
                -forwardVel * brakeStrength;

            carRigidBody.AddForceAtPosition(
                forwardDir * brakeForce,
                forcePoint
            );
        }

        // ---------- ENGINE BRAKING (COASTING) ----------

        if (Mathf.Abs(accelInput) < 0.05f)
        {
            float forwardVel =
                Vector3.Dot(
                    forwardDir,
                    tireWorldVel
                );

            float engineBrake = 80f;

            float brakeForce =
                -forwardVel * engineBrake;

            carRigidBody.AddForceAtPosition(
                forwardDir * brakeForce,
                forcePoint
            );
        }

        Debug.DrawRay(
            tireTransform.position,
            -springDir * suspensionRestDist,
            Color.blue
        );
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.relativeVelocity.magnitude > 15f)
            accelInput = 0f;

        Vector3 normal =
            col.contacts[0].normal;

        carRigidBody.AddForce(
            -normal * 8000f,
            ForceMode.Acceleration
        );
    }
}
