using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization; // Kültür ayarlarý için gerekli

public class HistoryAnalyzer : MonoBehaviour
{
    public struct GelisimOzeti
    {
        public int toplamSoru;
        public int toplamDogru;
        public SoruKategorisi enCokHataYapilanKategori;
        public float ortalamaSure;
        public int[] kategoriDogruSayilari;
        public int[] kategoriYanlisSayilari;
    }

    public static GelisimOzeti GecmisiAnalizEt()
    {
        GelisimOzeti ozet = new GelisimOzeti();
        ozet.kategoriDogruSayilari = new int[3];
        ozet.kategoriYanlisSayilari = new int[3];

        string path = Application.persistentDataPath;
        if (!Directory.Exists(path)) return ozet;

        string[] dosyalar = Directory.GetFiles(path, "NeuroRacer_*.csv");

        float toplamSure = 0;
        int veriSatiriSayisi = 0;

        foreach (string dosya in dosyalar)
        {
            string[] satirlar = File.ReadAllLines(dosya);
            for (int i = 1; i < satirlar.Length; i++)
            {
                // Satýrý parçala ve boþluklarý temizle
                string[] veri = satirlar[i].Split(',');
                if (veri.Length < 8) continue; // Sütun sayýsýný kontrol et (Tarih,Soru,Secilen,DogruCevap,Sonuc,Sure,Kategori,Zorluk)

                // 1. SÜRE ANALÝZÝ (GÜVENLÝ) - 5. index
                string sureMetni = veri[5].Trim();
                if (float.TryParse(sureMetni, NumberStyles.Any, CultureInfo.InvariantCulture, out float sureDegeri))
                {
                    toplamSure += sureDegeri;
                    veriSatiriSayisi++;
                }

                // 2. KATEGORÝ ANALÝZÝ (GÜVENLÝ) - 6. index
                string kategoriMetni = veri[6].Trim();
                if (System.Enum.TryParse(kategoriMetni, out SoruKategorisi kat))
                {
                    int katIndex = (int)kat;
                    ozet.toplamSoru++;

                    // 3. SONUÇ ANALÝZÝ (GÜVENLÝ) - 4. index
                    string sonucMetni = veri[4].Trim().ToLower();
                    if (sonucMetni == "true")
                    {
                        ozet.toplamDogru++;
                        ozet.kategoriDogruSayilari[katIndex]++;
                    }
                    else
                    {
                        ozet.kategoriYanlisSayilari[katIndex]++;
                    }
                }
            }
        }

        if (veriSatiriSayisi > 0)
            ozet.ortalamaSure = toplamSure / veriSatiriSayisi;

        // En çok hata yapýlaný bul
        int maxHataIndex = 0;
        for (int i = 1; i < ozet.kategoriYanlisSayilari.Length; i++)
        {
            if (ozet.kategoriYanlisSayilari[i] > ozet.kategoriYanlisSayilari[maxHataIndex])
                maxHataIndex = i;
        }
        ozet.enCokHataYapilanKategori = (SoruKategorisi)maxHataIndex;

        return ozet;
    }
}