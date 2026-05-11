using UnityEngine;
using TMPro; // TextMeshPro kullandýðýn için gerekli
using System.IO;

public class MenuStatsManager : MonoBehaviour
{
    [Header("UI Metin Referanslarý")]
    [SerializeField] private TextMeshProUGUI detaylarText;

    public void IstatistikleriGoster()
    {
        // Önce klasörde herhangi bir log dosyasý var mý kontrol et
        string[] dosyalar = Directory.GetFiles(Application.persistentDataPath, "NeuroRacer_*.csv");

        if (dosyalar.Length == 0)
        {
            EkraniSifirla();
            return;
        }

        // HistoryAnalyzer üzerinden tüm geçmiþin analizini al
        HistoryAnalyzer.GelisimOzeti ozet = HistoryAnalyzer.GecmisiAnalizEt();

        detaylarText.text = $"Görsel Benzerlik D/Y : {ozet.kategoriDogruSayilari[0]} / {ozet.kategoriYanlisSayilari[0]} \n" +
            $"Hece Karýþtýrma D/Y : {ozet.kategoriDogruSayilari[1]} / {ozet.kategoriYanlisSayilari[1]} \n" +
            $"Benzer Sesli Harfler D/Y : {ozet.kategoriDogruSayilari[2]} / {ozet.kategoriYanlisSayilari[2]} \n" +
            $"Ortalama Cevap Süresi : {ozet.ortalamaSure:F2} sn";
    }

    private void EkraniSifirla()
    {
        detaylarText.text = "Görsel Benzerlik D/Y : Henüz Veri Yok \n" +
            "Hece Karýþtýrma D/Y : Henüz Veri Yok \n" +
            "Benzer Sesli Harfler D/Y : Henüz Veri Yok \n" +
            "Ortalama Cevap Süresi : Henüz Veri Yok";
    }
}