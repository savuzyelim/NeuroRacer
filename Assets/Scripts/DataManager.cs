using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public List<QuestionLog> tumLoglar = new List<QuestionLog>();

    private void Awake()
    {
        Instance = this;
    }

    public void LogEkle(string kelime, string secim, bool sonuc, float sure)
    {
        QuestionLog yeniLog = new QuestionLog(kelime, secim, sonuc, sure);
        tumLoglar.Add(yeniLog);

        Debug.Log($"Log Kaydedildi: {kelime} - Doðru mu: {sonuc}");
    }

    // Oyun bittiðinde verileri CSV (Excel'de açýlabilir) olarak kaydeder
    public void VerileriKaydet()
    {
        string yol = Application.persistentDataPath + "/NeuroRacer_Log.csv";
        TextWriter tw = new StreamWriter(yol, false);

        // Baþlýk satýrý
        tw.WriteLine("Tarih,Sorulan Kelime,Secilen Cevap,Sonuc,Sure(sn)");

        foreach (var log in tumLoglar)
        {
            tw.WriteLine($"{log.tarih},{log.sorulanKelime},{log.secilenCevap},{log.dogruMu},{log.cevaplamaSuresi}");
        }

        tw.Close();
        Debug.Log("Tüm veriler kaydedildi: " + yol);
    }
}