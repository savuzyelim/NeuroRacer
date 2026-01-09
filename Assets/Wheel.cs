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
    public float tireGripFactor = 1.2f; // Increased base grip
    public float rearGripMultiplier = 1.4f;
    public float tireMass = 20f;
    public float maxGripForce = 8000f;

    [Header("Engine")]
    public float accelInput;
    public float carTopSpeed = 60f;
    public AnimationCurve powerCurve;
    public float enginePower = 30000f;

    [Header("Steering")]
    public bool isSteerWheel = false;
    public float steerInput;
    public float maxSteerAngle = 35f;
    public float steerSpeed = 8f; // Faster response

    [Header("Steering Assistance")]
    public float lowSpeedSteerBoost = 2.5f; // Extra steering force at low speeds
    public float steerAssistForce = 3000f; // Direct steering force
    public float minSpeedForNormalGrip = 5f; // Speed threshold

    [Header("Raycast")]
    public LayerMask groundLayer;

    private float currentSteerAngle = 0f;
    private float forceHeightOffset = 0.1f;

    void Start()
    {
        tireTransform = transform;
    }

    void FixedUpdate()
    {
        // Apply steering rotation to front wheels
        if (isSteerWheel)
        {
            float targetAngle = steerInput * maxSteerAngle;
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetAngle, Time.fixedDeltaTime * steerSpeed);
            tireTransform.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
        }

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

        Vector3 tireWorldVel = carRigidBody.GetPointVelocity(tireTransform.position);

        // ---------- SUSPENSION ----------
        float offset = suspensionRestDist - hit.distance;
        float vel = Vector3.Dot(springDir, tireWorldVel);
        float springForce = (offset * springStrength) - (vel * springDamper);

        springForce = Mathf.Clamp(springForce, -maxSpringForce, maxSpringForce);

        carRigidBody.AddForceAtPosition(
            springDir * springForce,
            tireTransform.position
        );

        // ---------- FORCE POINT ----------
        Vector3 forcePoint = tireTransform.position;
        if (carRigidBody != null)
        {
            forcePoint.y = carRigidBody.worldCenterOfMass.y - forceHeightOffset;
        }

        // ---------- CALCULATE SPEED ----------
        float carSpeed = carRigidBody.linearVelocity.magnitude;
        float speedFactor = Mathf.Clamp01(carSpeed / minSpeedForNormalGrip);

        // ---------- STEERING ASSIST (For low speeds) ----------
        if (isSteerWheel && Mathf.Abs(steerInput) > 0.01f)
        {
            // Add direct steering force, stronger at low speeds
            float steerBoost = Mathf.Lerp(lowSpeedSteerBoost, 1f, speedFactor);
            Vector3 steerDir = tireTransform.right * steerInput;
            float steerForce = steerAssistForce * steerBoost;

            carRigidBody.AddForceAtPosition(steerDir * steerForce, forcePoint);
        }

        // ---------- LATERAL GRIP (with speed-based adjustment) ----------
        Vector3 sideDir = tireTransform.right;
        float sideVel = Vector3.Dot(sideDir, tireWorldVel);

        // Base grip
        float grip = tireGripFactor;

        // For steering wheels: reduce grip when turning at low speeds
        if (isSteerWheel && Mathf.Abs(steerInput) > 0.1f)
        {
            // Allow more sliding when steering at low speeds
            float steerGripReduction = Mathf.Lerp(0.4f, 1f, speedFactor);
            grip *= steerGripReduction;
        }
        else if (!isSteerWheel)
        {
            // Rear wheels keep high grip
            grip *= rearGripMultiplier;
        }

        float desiredVelChange = -sideVel * grip;
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
        float gripForce = tireMass * desiredAccel;

        gripForce = Mathf.Clamp(gripForce, -maxGripForce, maxGripForce);

        carRigidBody.AddForceAtPosition(sideDir * gripForce, forcePoint);

        // ---------- ENGINE FORCE ----------
        Vector3 forwardDir = tireTransform.forward;

        if (Mathf.Abs(accelInput) > 0.01f)
        {
            float forwardSpeed = Vector3.Dot(carTransform.forward, carRigidBody.linearVelocity);
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / carTopSpeed);

            float torque = powerCurve.Evaluate(normalizedSpeed) * accelInput * enginePower;

            carRigidBody.AddForceAtPosition(forwardDir * torque, forcePoint);
        }

        // ---------- ROLLING RESISTANCE ----------
        if (Mathf.Abs(accelInput) < 0.01f)
        {
            float forwardVel = Vector3.Dot(forwardDir, tireWorldVel);
            float rollingResistance = 120f;
            float resistForce = -forwardVel * rollingResistance;

            carRigidBody.AddForceAtPosition(
                forwardDir * resistForce,
                tireTransform.position
            );
        }

        Debug.DrawRay(tireTransform.position, -springDir * suspensionRestDist, Color.blue);

        // Debug: Show steering direction
        if (isSteerWheel && Mathf.Abs(steerInput) > 0.01f)
        {
            Debug.DrawRay(forcePoint, tireTransform.right * steerInput * 3f, Color.green);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.relativeVelocity.magnitude > 15f)
        {
            accelInput = 0f;
        }

        Vector3 normal = col.contacts[0].normal;

        carRigidBody.AddForce(
            -normal * 8000f,
            ForceMode.Acceleration
        );
    }
}