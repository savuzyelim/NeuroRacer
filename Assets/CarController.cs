using UnityEngine;

public class CarController : MonoBehaviour
{
    // Tekerlek scriptlerinin olduðu objeler
    public Wheel[] wheels;

    [Header("Denge Ayarlarý")]
    public Rigidbody carRb; // Arabanýn Rigidbody'sini buraya sürüklemeyi unutma
    public Vector3 centerOfMassOffset = new Vector3(0, -0.9f, 0); // Arabayý yere bastýran ayar

    [Header("Yarýþ Durumu")]
    public bool hareketEdebilir = false; // Yarýþ baþlamadan önce bu tik kapalý olacak

    [Header("Input (Ýzleme Amaçlý)")]
    public float accel;
    public float steer;

    [Header("Steering")]
    public float maxSteerAngle = 30f;

    void Start()
    {
        // 1. Arabanýn aðýrlýk merkezini aþaðý çekiyoruz (Takla atmamasý için)
        if (carRb != null)
        {
            carRb.centerOfMass += centerOfMassOffset;
        }
        
    }

    void Update()
    {
        // 2. Eðer yarýþ baþlamadýysa (hareketEdebilir kapalýysa) input alma
        if (!hareketEdebilir)
        {
            accel = 0f;
            steer = 0f;
            return; // Buradan sonrasýný okuma
        }

        // 3. Yarýþ baþladýysa tuþlarý dinle
        accel = Input.GetAxis("Vertical");   // W/S veya Yön Tuþlarý
        steer = Input.GetAxis("Horizontal"); // A/D veya Yön Tuþlarý
        wheels[1].steerInput = Input.GetAxis("Horizontal"); // Ön sol teker
        wheels[0].steerInput = Input.GetAxis("Horizontal"); // Ön sað teker
    }

    void FixedUpdate()
    {
        // 4. Gaz/Fren bilgisini tekerleklere ilet
        foreach (Wheel w in wheels)
        {
            w.accelInput = accel;
        }

        // 5. Direksiyonu çevir
        ApplySteering();
    }

    void ApplySteering()
    {
        float steerAngle = steer * maxSteerAngle;

        foreach (Wheel w in wheels)
        {
            // Sadece dönmesi gereken tekerlekleri (isSteerWheel) çevir
            if (!w.isSteerWheel) continue;

            Vector3 euler = w.transform.localEulerAngles;
            euler.y = steerAngle;
            w.transform.localEulerAngles = euler;
        }
    }
}