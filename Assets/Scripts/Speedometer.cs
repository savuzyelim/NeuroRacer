using UnityEngine;
using TMPro; // TextMeshPro için gerekli

public class Speedometer : MonoBehaviour
{
    [Header("Baðlantýlar")]
    public Rigidbody arabaRb;       // Arabanýn Rigidbody'sini sürükle
    public TextMeshProUGUI hizText; // Oluþturduðun Text'i sürükle

    [Header("Ayarlar")]
    public string sonEk = " KM/H"; // Sayýnýn yanýna ne yazsýn?

    void Update()
    {
        if (arabaRb == null) return;

        // MATEMATÝK:
        // Unity'de hýz "metre/saniye"dir.
        // Bunu 3.6 ile çarparsan "km/saat" olur.
        // linearVelocity Unity 6 içindir, hata verirse .velocity yap.
        float hiz = arabaRb.linearVelocity.magnitude * 3.6f;

        // GÖSTERÝM:
        // "F0" demek: Virgüllü sayý gösterme, tam sayý yap (örn: 120)
        hizText.text = hiz.ToString("F0") + sonEk;
    }
}