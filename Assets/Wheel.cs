using UnityEngine;

public class Wheel : MonoBehaviour
{
    Transform tireTransform;

    [Header("References")]
    public Rigidbody carRigidBody;
    public Transform carTransform;

    [Header("Suspension")]
    public float springStrength = 15000f; // Sert yay
    public float springDamper = 2000f;    // Yüksek sönümleme (Sallantýyý keser)
    public float suspensionRestDist = 0.5f;
    public float maxSpringForce = 16000f;

    [Header("Grip")]
    public float tireGripFactor = 0.8f;
    public float rearGripMultiplier = 1.4f;
    public float tireMass = 20f;
    public float maxGripForce = 8000f;

    [Header("Engine")]
    public float accelInput;
    public float carTopSpeed = 60f;
    public AnimationCurve powerCurve;
    public float enginePower = 30000f;

    [Header("Wheel Type")]
    public bool isSteerWheel = false;

    // YENÝ: Aðýrlýk merkezi dengelemesi için ayar
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
            suspensionRestDist
        );

        if (!grounded) return;

        // DÜZELTME BURADA: GetLinearVelocity -> GetPointVelocity
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

        // ================================================================
        // KUVVET UYGULAMA NOKTASI (FORCE POINT) HÝLESÝ
        // Motor gücünü tekerden deðil, aðýrlýk merkezinin hizasýndan itiyoruz.
        // ================================================================

        Vector3 forcePoint = tireTransform.position;
        if (carRigidBody != null)
        {
            // Unity 6'da worldCenterOfMass doðru çalýþýr
            forcePoint.y = carRigidBody.worldCenterOfMass.y - forceHeightOffset;
        }

        // ---------- LATERAL GRIP (Yanal Tutuþ) ----------

        Vector3 sideDir = tireTransform.right;
        float sideVel = Vector3.Dot(sideDir, tireWorldVel);
        float grip = tireGripFactor;

        if (!isSteerWheel) grip *= rearGripMultiplier;

        float desiredVelChange = -sideVel * grip;
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
        float gripForce = tireMass * desiredAccel;

        gripForce = Mathf.Clamp(gripForce, -maxGripForce, maxGripForce);

        // Yanal kuvveti dengeli noktadan uyguluyoruz
        carRigidBody.AddForceAtPosition(sideDir * gripForce, forcePoint);

        // ---------- ENGINE FORCE (Motor Gücü) ----------

        Vector3 forwardDir = tireTransform.forward;

        if (Mathf.Abs(accelInput) > 0.01f)
        {
            // Unity 6 için linearVelocity kullanýmý doðrudur
            float carSpeed = Vector3.Dot(carTransform.forward, carRigidBody.linearVelocity);
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

            float torque = powerCurve.Evaluate(normalizedSpeed) * accelInput * enginePower;

            // Ýleri gücü dengeli noktadan uyguluyoruz
            carRigidBody.AddForceAtPosition(forwardDir * torque, forcePoint);
        }

        // ---------- AUTO BRAKE (Otomatik Fren) ----------

        if (Mathf.Abs(accelInput) < 0.01f)
        {
            float forwardVel = Vector3.Dot(forwardDir, tireWorldVel);
            float brakeStrength = 1500f;
            float brakeForce = -forwardVel * brakeStrength;

            // Freni dengeli noktadan uyguluyoruz
            carRigidBody.AddForceAtPosition(forwardDir * brakeForce, forcePoint);
        }
    }
}