using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    //G�zel hissedene kadar de�i�tirip test ederiz bunlar�
    public float acceleration = 500f;
    public float breakForce = 300f;
    public float maxTurnAnge = 15f;
    
    public float currentAcceleration = 0f;
    public float currentBreakForce = 0f;
    public float currentTurnAngle = 0;
    private void FixedUpdate()
    {
        // İleri Geri
        currentAcceleration = acceleration * Input.GetAxis("Vertical") * -1;

        // Fren
        if (Input.GetKey(KeyCode.Space))
            currentBreakForce = breakForce;
        else
            currentBreakForce = 0;

        // Tekerleri Döndür
        frontRight.motorTorque = currentAcceleration;
        frontLeft.motorTorque = currentAcceleration;

        // Fren Yap
        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;

        // Direksiyon - Hıza göre direksiyon hassasiyeti
        float speed = GetComponent<Rigidbody>().linearVelocity.magnitude;
        float speedFactor = Mathf.Clamp01(1 - (speed / 50f)); // 50 maksimum hız
        float adjustedTurnAngle = maxTurnAnge * speedFactor;

        currentTurnAngle = adjustedTurnAngle * Input.GetAxis("Horizontal");
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;

        // Tuşa basılmadığında yavaşça dur
        if (Input.GetAxis("Vertical") == 0 && !Input.GetKey(KeyCode.Space))
        {
            // Hafif sürtünme freni uygula
            float dragBrake = 50f; // Bu değeri ihtiyacına göre ayarla
            frontRight.brakeTorque = dragBrake;
            frontLeft.brakeTorque = dragBrake;
            backRight.brakeTorque = dragBrake;
            backLeft.brakeTorque = dragBrake;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            transform.rotation = Quaternion.identity;
        }
    }
}
