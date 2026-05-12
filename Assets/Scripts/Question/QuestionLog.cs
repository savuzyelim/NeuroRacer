using UnityEngine;

// --- 1. ENUM ---
public enum SoruKategorisi
{
    GorselBenzerlik,
    HeceKaristirma,
    FonolojikFarklilik
}

// --- 2. LOG SINIFI ---
[System.Serializable]
public class QuestionLog
{
    public string sorulanKelime;
    public string secilenCevap;
    public bool dogruMu;
    public float cevaplamaSuresi;
    public SoruKategorisi kategori;
    public int zorluk;
    public string tarih;

    public QuestionLog(string kelime, string secim, bool sonuc, float sure, SoruKategorisi kat, int zor)
    {
        sorulanKelime = kelime;
        secilenCevap = secim;
        dogruMu = sonuc;
        cevaplamaSuresi = sure;
        kategori = kat;
        zorluk = zor;
        tarih = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}