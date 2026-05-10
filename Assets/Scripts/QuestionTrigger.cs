using UnityEngine;

public class QuestionTrigger : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Eðer iþaretliyse, bu kutu her zaman belirli bir soruyu sorar. Ýþaretli deðilse öðrenciye özel soru seçer.")]
    public bool sabitSoruMu = false;
    public QuestionData sabitSoruVerisi;

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Tetiklenme kontrolü ve Tag kontrolü
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;

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
                AdaptiveQuestionSelector selector = FindObjectOfType<AdaptiveQuestionSelector>();
                if (selector != null)
                {
                    sorulacakSoru = selector.SiradakiSoruyuGetir();
                }
            }

            // 2. Paneli Aç
            if (sorulacakSoru != null)
            {
                QuestionManager.Instance.OpenQuestionPanel(sorulacakSoru);
            }
            else
            {
                Debug.LogWarning("Soru verisi bulunamadý! Lütfen AdaptiveQuestionSelector veya sabit soruyu kontrol edin.");
            }

            // 3. Görsel Geri Bildirim ve Temizlik
            // Kutuyu hemen yok etmek yerine görünmez yapabiliriz (ses/efekt bitene kadar kalmasý için)
            //gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;

            // 5 saniye sonra tamamen temizle (bellek yönetimi)
            //Destroy(gameObject, 5f);
        }
    }
}