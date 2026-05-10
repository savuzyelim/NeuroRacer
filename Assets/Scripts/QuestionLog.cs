[System.Serializable]
public class QuestionLog
{
    public string sorulanKelime;
    public string secilenCevap;
    public bool dogruMu;
    public float cevaplamaSuresi;
    public string tarih;

    public QuestionLog(string kelime, string secim, bool sonuc, float sure)
    {
        sorulanKelime = kelime;
        secilenCevap = secim;
        dogruMu = sonuc;
        cevaplamaSuresi = sure;
        tarih = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}