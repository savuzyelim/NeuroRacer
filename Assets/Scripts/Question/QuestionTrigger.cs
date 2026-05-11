using UnityEngine;

public class QuestionTrigger : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Eðer iþaretliyse, bu kutu her zaman belirli bir soruyu sorar. Ýþaretli deðilse öðrenciye özel soru seçer.")]
    public bool sabitSoruMu = false;
    public QuestionData sabitSoruVerisi;


    private void OnTriggerEnter(Collider other)
    {
        // Tetiklenme kontrolü ve Tag kontrolü
        if (other.CompareTag("Player"))
        {

            // 1. Soruyu Belirle
            QuestionData sorulacakSoru = null;

            if (sabitSoruMu && sabitSoruVerisi != null)
            {
                // Belirli bir eðitim noktasýysa sabit soruyu kullan
                sorulacakSoru = sabitSoruVerisi;
            }
            else
            {
                // Deðilse, öðrencinin loglarýný analiz eden sistemden dinamik soru al
                AdaptiveQuestionManager selector = FindObjectOfType<AdaptiveQuestionManager>();
                if (selector != null)
                {
                    sorulacakSoru = selector.SiradakiSoruyuGetir();
                }
            }

            // 2. Paneli Aç
            if (sorulacakSoru != null)
            {
                AdaptiveQuestionManager.Instance.OpenQuestionPanel(sorulacakSoru);
            }
            else
            {
                Debug.LogWarning("Soru verisi bulunamadý! Lütfen AdaptiveQuestionSelector veya sabit soruyu kontrol edin.");
            }
        }
    }
}