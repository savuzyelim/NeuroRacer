using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager Instance; // Her yerden eriþmek için

    [Header("Müzik Kaynaðý")]
    public AudioSource muzikKaynagi;

    [Header("Ses Ayarlarý")]
    [Range(0f, 1f)] public float normalSes = 0.5f; // Normalde ses seviyesi (%50 iyi genelde)
    [Range(0f, 1f)] public float kisikSes = 0.1f;  // Soru gelince (%10'a düþsün - %80 azalma)

    public float gecisSuresi = 0.5f; // Sesi kýsma/açma hýzý

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Baþlangýçta müzik çalýyorsa bile sesini 0 yapalým ki "Fade In" ile girsin
        if (muzikKaynagi != null)
        {
            muzikKaynagi.volume = 0;
            muzikKaynagi.loop = true; // Müzik bitince baþa sarsýn
        }
    }

    public void MuzigiBaslat()
    {
        if (muzikKaynagi == null) return;

        muzikKaynagi.Play();
        // Sesi 0'dan normal seviyeye yumuþakça çýkar
        muzikKaynagi.DOFade(normalSes, 2.0f);
    }

    public void SesiKis()
    {
        if (muzikKaynagi == null) return;

        // SetUpdate(true) ÇOK ÖNEMLÝ:
        // Çünkü QuestionManager zamaný durduruyor (TimeScale 0). 
        // Bunu demezsek ses kýsýlma animasyonu da donar.
        muzikKaynagi.DOFade(kisikSes, gecisSuresi).SetUpdate(true);
    }

    public void SesiAc()
    {
        if (muzikKaynagi == null) return;

        // Sesi tekrar normale döndür
        muzikKaynagi.DOFade(normalSes, gecisSuresi).SetUpdate(true);
    }
}