using UnityEngine;

public class CarFollowCamera : MonoBehaviour
{
    [Header("Takip Ayarları")]
    public Transform hedef;
    public float mesafe = 5.0f;
    public float yukseklik = 2.0f;

    [Header("Model Ters ise İşaretle")]
    public bool modelTersMi = true; // Eğer arabanın önünü görüyorsan bunu tikli bırak

    [Header("Yumuşatma Ayarları")]
    public float takipHizi = 10f;
    public float donusHizi = 10f;

    void LateUpdate()
    {
        if (!hedef) return;

        // Model tersse yön vektörünü tersine çeviriyoruz
        Vector3 ileriYon = modelTersMi ? -hedef.forward : hedef.forward;

        // Hedeflenen pozisyonu hesapla
        Vector3 hedefPozisyon = hedef.position - (ileriYon * mesafe) + (Vector3.up * yukseklik);

        // Kamerayı yumuşak bir şekilde taşı
        transform.position = Vector3.Lerp(transform.position, hedefPozisyon, takipHizi * Time.deltaTime);

        // Kamerayı arabaya (ve biraz yukarısına) odakla
        Vector3 bakisNoktasi = hedef.position + (Vector3.up * (yukseklik * 0.5f));
        Quaternion hedefDonus = Quaternion.LookRotation(bakisNoktasi - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, hedefDonus, donusHizi * Time.deltaTime);
    }
}