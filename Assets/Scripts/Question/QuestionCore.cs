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

// --- 3. SORU DATA SINIFI ---
[CreateAssetMenu(fileName = "Yeni Soru", menuName = "NeuroRacer/Soru")]
public class QuestionData : ScriptableObject
{
    public string soruMetni;
    public string[] siklar;
    public int dogruCevapIndex;
    public AudioClip soruSesi;
    public SoruKategorisi kategori;
    [Range(1, 3)] public int zorlukSeviyesi = 1;
}