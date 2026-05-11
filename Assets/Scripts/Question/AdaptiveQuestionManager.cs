using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class AdaptiveQuestionManager : MonoBehaviour
{
    public static AdaptiveQuestionManager Instance;

    [Header("Soru Havuzu")]
    public List<QuestionData> tumSorular;
    private int[] hataListesi = new int[3];

    [Header("UI Referanslarý")]
    public GameObject anaPanelObjesi;
    public RectTransform soruKutusuRect;
    public Image arkaplanGorseli;

    [Header("UI Ýçerik")]
    public TextMeshProUGUI soruText;
    public Button[] sikButonlari;
    public AudioSource sesKaynagi;

    [Header("Doðru Cevap Ayarlarý")]
    public AudioClip dogruCevapSesi;
    public Color dogruRenk = Color.green;
    [Range(0f, 1f)] public float dogruCevapOpaklik = 0.3f;
    public float kutlamaSuresi = 1.5f;

    [Header("Animasyon Hýzlarý")]
    [Range(0.1f, 2f)] public float arkaplanGelisSuresi = 0.5f;
    [Range(0.1f, 2f)] public float kutuGelisSuresi = 0.6f;

    [Header("Zaman (Matrix Modu) Ayarlarý")]
    [Range(0.1f, 3f)] public float yavaslamaSuresi = 0.8f;
    [Range(0.1f, 3f)] public float hizlanmaSuresi = 0.5f;
    public float enYavasZaman = 0.005f;

    [Header("Konum Ayarlarý")]
    public float ekranDisiKonumY = -2000f;
    [Range(0f, 1f)] public float arkaplanHedefAlpha = 0.90f;

    private QuestionData simdikiSoru;
    private float varsayilanFixedDeltaTime;
    private Sequence acilisSequence;
    private Tween zamanTween;
    private float soruBaslangicZamani;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        anaPanelObjesi.SetActive(false);
        varsayilanFixedDeltaTime = 0.02f;
    }

    public void OpenQuestionPanel(QuestionData data)
    {
        // Eðer data null gelirse (Trigger'dan boþ gelirse) otomatik seçim yap
        simdikiSoru = (data != null) ? data : SiradakiSoruyuGetir();

        soruBaslangicZamani = Time.realtimeSinceStartup;

        if (acilisSequence != null) acilisSequence.Kill();
        if (zamanTween != null) zamanTween.Kill();

        arkaplanGorseli.color = new Color(0, 0, 0, 0);
        soruKutusuRect.anchoredPosition = new Vector2(0, ekranDisiKonumY);

        foreach (var btn in sikButonlari) btn.interactable = true;

        anaPanelObjesi.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        soruText.text = simdikiSoru.soruMetni;
        if (simdikiSoru.soruSesi != null && sesKaynagi != null)
        {
            sesKaynagi.clip = simdikiSoru.soruSesi;
            sesKaynagi.Play();
        }
        ButonlariHazirla(simdikiSoru);

        acilisSequence = DOTween.Sequence().SetUpdate(true);
        acilisSequence.Append(arkaplanGorseli.DOFade(arkaplanHedefAlpha, arkaplanGelisSuresi));
        acilisSequence.Join(soruKutusuRect.DOAnchorPosY(0, kutuGelisSuresi)
            .SetDelay(arkaplanGelisSuresi * 0.3f)
            .SetEase(Ease.OutBack));

        if (GameMusicManager.Instance != null) GameMusicManager.Instance.SesiKis();

        zamanTween = DOVirtual.Float(Time.timeScale, enYavasZaman, yavaslamaSuresi, (x) =>
        {
            Time.timeScale = x;
            Time.fixedDeltaTime = varsayilanFixedDeltaTime * x;
        }).SetUpdate(true).SetEase(Ease.OutExpo);
    }

    public QuestionData SiradakiSoruyuGetir()
    {
        // 1. Geçmiþ yarýþlarý analiz et (Tüm CSV dosyalarý üzerinden)
        HistoryAnalyzer.GelisimOzeti gecmisAnalizi = HistoryAnalyzer.GecmisiAnalizEt();

        Debug.Log(gecmisAnalizi.enCokHataYapilanKategori);

        // 2. Mevcut yarýþ içindeki hatalarý da ekle
        System.Array.Clear(hataListesi, 0, hataListesi.Length);
        foreach (var log in DataManager.Instance.mevcutYarisLoglari)
        {
            if (!log.dogruMu) hataListesi[(int)log.kategori]++;
        }

        // 3. En sorunlu kategoriyi belirle (Geçmiþ + Mevcut)
        SoruKategorisi hedefKategori = gecmisAnalizi.enCokHataYapilanKategori;

        // Eðer mevcut yarýþta baþka bir kategoride patlama yaþanýyorsa ona odaklan
        for (int i = 0; i < hataListesi.Length; i++)
        {
            if (hataListesi[i] > 3) hedefKategori = (SoruKategorisi)i;
        }

        // 4. Adaptif Seçim: %70 ihtimalle zorlanýlan alandan sor
        if (Random.value < 0.7f)
        {
            List<QuestionData> adaySorular = tumSorular.FindAll(s => s.kategori == hedefKategori);
            if (adaySorular.Count > 0) return adaySorular[Random.Range(0, adaySorular.Count)];
        }

        // Deðilse tamamen rastgele
        return tumSorular[Random.Range(0, tumSorular.Count)];
    }

    private void ButonlariHazirla(QuestionData data)
    {
        for (int i = 0; i < sikButonlari.Length; i++)
        {
            sikButonlari[i].onClick.RemoveAllListeners();
            if (i < data.siklar.Length)
            {
                sikButonlari[i].gameObject.SetActive(true);
                var text = sikButonlari[i].GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = data.siklar[i];
                int index = i;
                sikButonlari[i].onClick.AddListener(() => CevapVerildi(index));
            }
            else sikButonlari[i].gameObject.SetActive(false);
        }
    }

    public void CevapVerildi(int secilenIndex)
    {
        float gecenSure = Time.realtimeSinceStartup - soruBaslangicZamani;
        string secilenMetin = simdikiSoru.siklar[secilenIndex];
        string dogruMetin = simdikiSoru.siklar[simdikiSoru.dogruCevapIndex];
        bool dogruMu = (secilenIndex == simdikiSoru.dogruCevapIndex);

        if (DataManager.Instance != null)
        {
            DataManager.Instance.LogEkle(
                simdikiSoru.soruMetni,
                secilenMetin,
                dogruMu,
                gecenSure,
                simdikiSoru.kategori,
                simdikiSoru.zorlukSeviyesi,
                dogruMetin
            );
        }

        foreach (var btn in sikButonlari) btn.interactable = false;

        if (dogruMu)
        {
            var car = FindObjectOfType<NewCarController>();
            if (car != null) car.ActivateBoost();
            StartCoroutine(DogruCevapAnimasyonu());
        }
        else
        {
            PaneliKapat();
        }
    }

    IEnumerator DogruCevapAnimasyonu()
    {
        soruText.text = "DOÐRU BÝLDÝN!";
        if (sesKaynagi != null && dogruCevapSesi != null) sesKaynagi.PlayOneShot(dogruCevapSesi);

        Color seffafYesil = dogruRenk;
        seffafYesil.a = dogruCevapOpaklik;
        arkaplanGorseli.color = seffafYesil;

        arkaplanGorseli.DOFade(0.05f, 0.25f).SetLoops(6, LoopType.Yoyo).SetUpdate(true);

        yield return new WaitForSecondsRealtime(kutlamaSuresi);
        PaneliKapat();
    }

    public void PaneliKapat()
    {
        if (acilisSequence != null) acilisSequence.Kill();
        if (zamanTween != null) zamanTween.Kill();

        arkaplanGorseli.DOFade(0f, 0.3f).SetUpdate(true);
        soruKutusuRect.DOAnchorPosY(ekranDisiKonumY, 0.3f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => anaPanelObjesi.SetActive(false));

        if (GameMusicManager.Instance != null) GameMusicManager.Instance.SesiAc();

        zamanTween = DOVirtual.Float(Time.timeScale, 1.0f, hizlanmaSuresi, (x) =>
        {
            Time.timeScale = x;
            Time.fixedDeltaTime = varsayilanFixedDeltaTime * x;
        }).SetUpdate(true).SetEase(Ease.InSine);
    }
}