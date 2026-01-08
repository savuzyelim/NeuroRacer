using UnityEngine;
using System.Collections;

public class RaceStartManager : MonoBehaviour
{
    [Header("Baðlantýlar")]
    public CarController oyuncuArabasi;
    public AudioSource sesKaynagi;

    [Header("Iþýk Modelleri (Renderers)")]
    // led_a, led_b, led_c objelerini buraya sürükle
    public MeshRenderer[] isikObjeleri;

    [Header("Renk Ayarlarý")]
    public Color kapaliRenk = Color.black; // Iþýk sönükken ne renk olsun? (Siyah/Gri)
    public Color kirmiziRenk = Color.red;  // Yanýnca hangi kýrmýzý?
    public Color yesilRenk = Color.green;  // Baþlangýçta hangi yeþil?

    [Header("Parlaklýk Þiddeti (Glow)")]
    [Range(1f, 10f)] public float parlaklik = 4.0f; // Iþýk ne kadar parlasýn? (HDR intensity)

    [Header("Sesler")]
    public AudioClip bipSesi;
    public AudioClip baslaSesi;

    private void Start()
    {
        oyuncuArabasi.hareketEdebilir = false;

        // Baþlangýçta hepsini kapat (Siyah yap)
        IsiklariBoya(kapaliRenk, 0);

        StartCoroutine(YarisBaslatmaSenaryosu());
    }

    IEnumerator YarisBaslatmaSenaryosu()
    {
        yield return new WaitForSeconds(2f);

        // --- KIRMIZI IÞIKLAR SIRAYLA YANSIN ---
        for (int i = 0; i < isikObjeleri.Length; i++)
        {
            if (sesKaynagi && bipSesi) sesKaynagi.PlayOneShot(bipSesi);

            // Sadece sýradaki ýþýðý kýrmýzý yap ve parlat
            RengiDegistir(isikObjeleri[i], kirmiziRenk, parlaklik);

            yield return new WaitForSeconds(1.0f);
        }

        // --- RASTGELE BEKLEME ---
        float rastgeleBekleme = Random.Range(0.5f, 2.0f);
        yield return new WaitForSeconds(rastgeleBekleme);

        // --- YEÞÝL IÞIK (BAÞLA) ---
        if (sesKaynagi && baslaSesi) sesKaynagi.PlayOneShot(baslaSesi);

        IsiklariBoya(yesilRenk, parlaklik);

        // YENÝ EKLENEN SATIR: Müziði baþlat!
        if (GameMusicManager.Instance != null)
        {
            GameMusicManager.Instance.MuzigiBaslat();
        }

        oyuncuArabasi.hareketEdebilir = true;

        // Hepsini Yeþil Yap
        IsiklariBoya(yesilRenk, parlaklik);

        // Arabayý serbest býrak
        oyuncuArabasi.hareketEdebilir = true;

        // Ýstersen 2 saniye sonra ýþýklarý söndürebilirsin:
        yield return new WaitForSeconds(2f);
        IsiklariBoya(kapaliRenk, 0);
    }

    // Tek bir lambanýn rengini deðiþtiren fonksiyon
    void RengiDegistir(MeshRenderer rend, Color renk, float siddet)
    {
        // URP Lit Shader'ý için BaseColor ve EmissionColor deðiþtiriyoruz
        rend.material.SetColor("_BaseColor", renk);

        // Emission (Parlama) rengini hesapla: Renk * Þiddet
        Color parlakRenk = renk * siddet;
        rend.material.SetColor("_EmissionColor", parlakRenk);
        rend.material.EnableKeyword("_EMISSION"); // Parlamayý zorla aç
    }

    // Tüm lambalarý ayný anda boyayan fonksiyon
    void IsiklariBoya(Color renk, float siddet)
    {
        foreach (var lamba in isikObjeleri)
        {
            RengiDegistir(lamba, renk, siddet);
        }
    }
}