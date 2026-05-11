using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public List<QuestionLog> mevcutYarisLoglari = new List<QuestionLog>();

    private string dosyaAdi;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        // Her yeni yarýþ için benzersiz dosya adý: NeuroRacer_2026_05_11_1530.csv
        dosyaAdi = "NeuroRacer_" + DateTime.Now.ToString("yyyy_MM_dd_HHmm") + ".csv";
    }

    public void LogEkle(string soru, string secilen, bool sonuc, float sure, SoruKategorisi kat, int zorluk, string dogruCevap)
    {
        QuestionLog yeniLog = new QuestionLog(soru, secilen, sonuc, sure, kat, zorluk);
        // Log sýnýfýna doðru cevabý da eklediðinden emin ol (QuestionCore.cs güncellemesi aþaðýda)
        yeniLog.sorulanKelime = dogruCevap;

        mevcutYarisLoglari.Add(yeniLog);
    }

    public void VerileriKaydet()
    {
        if (mevcutYarisLoglari.Count == 0) return;

        string yol = Path.Combine(Application.persistentDataPath, dosyaAdi);
        using (StreamWriter sw = new StreamWriter(yol, false))
        {
            sw.WriteLine("Tarih,Secilen,DogruCevap,Sonuc,Sure,Kategori,Zorluk");
            foreach (var log in mevcutYarisLoglari)
            {
                sw.WriteLine($"{log.tarih},{log.secilenCevap},{log.sorulanKelime},{log.dogruMu},{log.cevaplamaSuresi},{log.kategori},{log.zorluk}");
            }
        }
        Debug.Log("Yarýþ kaydedildi: " + yol);
    }
}