using UnityEngine;
using System.Collections;

public class NewCarController : MonoBehaviour
{
    [Header("Bileþenler")]
    public Rigidbody rb;
    public LayerMask whatIsGround; // Müfettiþten (Inspector) "Ground" layer'ýný seçin

    [Header("Hareket Ayarlarý")]
    public float forwardAccel = 8f;
    public float reverseAccel = 4f;
    public float maxSpeed = 50f;
    public float turnStrength = 180f;

    [Header("Görsel Takip & Eðim Ayarlarý")]
    public float sphereOffset = 0.5f;
    public float alignmentSpeed = 10f; // Eðimlere uyum saðlama hýzý

    [Header("Fizik & Uçma Korumasý")]
    public float downforce = 50f; // Arabayý yere bastýran kuvvet
    public float raycastLength = 1f; // Zemini kontrol etme mesafesi

    [Header("Neuro Racer Boost Mekaniði")]
    public bool hareketEdebilir = false;
    public float boostMultiplier = 2f;
    public float boostDuration = 2f;
    private float currentBoost = 1f;

    private float speedInput, turnInput;
    private bool isGrounded;

    

    private void Start()
    {
        if (rb != null)
        {
            rb.transform.parent = null;
        }
    }

    private void Update()
    {
        if (!hareketEdebilir)
        {
            speedInput = 0;
            turnInput = 0;
            return;
        }

        // 1. Girdi Kontrolleri
        speedInput = 0f;
        float vertical = Input.GetAxis("Vertical");

        if (vertical > 0) speedInput = vertical * forwardAccel * 1000f;
        else if (vertical < 0) speedInput = vertical * reverseAccel * 1000f;

        turnInput = Input.GetAxis("Horizontal");

        // 2. Dönüþ Hesaplamasý
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            float newRotation = turnInput * turnStrength * Time.deltaTime;
            if (vertical < 0) newRotation *= -1;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, newRotation, 0f));
        }

        // 3. Eðim Uyumu (Raycast ile Yere Hizalama)
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, raycastLength, whatIsGround);

        if (isGrounded)
        {
            // Zeminin açýsýný (Normal) al ve arabayý ona göre döndür
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * alignmentSpeed);
        }

        // 4. Görsel Takip
        Vector3 targetPos = rb.transform.position - new Vector3(0, sphereOffset, 0);
        transform.position = targetPos;
    }

    private void FixedUpdate()
    {
        if (!hareketEdebilir) return;

        // 5. Hareket ve Yapay Yerçekimi
        if (isGrounded)
        {
            rb.AddForce(transform.forward * speedInput * currentBoost);
        }
        else
        {
            // Araba havadayken yere bastýran ekstra kuvvet (Uçmayý engeller)
            rb.AddForce(Vector3.down * downforce * 100f);
        }

        // Hýz Limitleyici
        if (rb.linearVelocity.magnitude > maxSpeed * currentBoost)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed * currentBoost;
        }
    }

    public void ActivateBoost()
    {
        StopCoroutine("BoostRoutine");
        StartCoroutine(BoostRoutine());
    }

    private IEnumerator BoostRoutine()
    {
        currentBoost = boostMultiplier;
        yield return new WaitForSeconds(boostDuration);
        currentBoost = 1f;
    }
}