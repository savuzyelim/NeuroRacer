using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    //Güzel hissedene kadar deðiþtirip test ederiz bunlarý
    public float acceleration = 500f;
    public float breakForce = 300f;
    public float maxTurnAnge = 15f;
    
    public float currentAcceleration = 0f;
    public float currentBreakForce = 0f;
    public float currentTurnAngle = 0;

    private void FixedUpdate()
    {
        //Ýleri Geri
        currentAcceleration = acceleration * Input.GetAxis("Vertical") * -1;
        //Fren
        if (Input.GetKey(KeyCode.Space))
            currentBreakForce = breakForce;
        else
            currentBreakForce = 0;
        //Tekerleri Döndür
        frontRight.motorTorque = currentAcceleration;
        frontLeft.motorTorque = currentAcceleration;
        //Fren Yap
        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;
        //Direksiyon
        currentTurnAngle = maxTurnAnge * Input.GetAxis("Horizontal");
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;       
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
