using UnityEngine;
using TMPro;            // TextMeshPro için
using UnityEngine.UI;   // UI Image/Button için
using DG.Tweening;      // Animasyonlar için (DOTween)
using System.Collections; // Coroutine için

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    [Header("UI Referanslarý")]
    public GameObject anaPanelObjesi;    // Komple paneli açýp kapatan obje
    public RectTransform soruKutusuRect; // Hareket edecek olan kutu
    public Image arkaplanGorseli;        // Arkadaki siyah perde

    [Header("UI Ýçerik")]
    public TextMeshProUGUI soruText;
    public Button[] sikButonlari;
    public AudioSource sesKaynagi;

    [Header("Doðru Cevap Ayarlarý (YENÝ)")]
    public AudioClip dogruCevapSesi;     // "Ding" sesi
    public Color dogruRenk = Color.green;// Hangi renk olsun? (Yeþil)
    [Range(0f, 1f)] public float dogruCevapOpaklik = 0.3f; // Yeþil ne kadar koyu olsun? (0.3 çok þeffaf)
    public float kutlamaSuresi = 1.5f;   // Panel kapanmadan önce kaç sn beklesin?

    [Header("Animasyon Hýzlarý")]
    [Range(0.1f, 2f)] public float arkaplanGelisSuresi = 0.5f;
    [Range(0.1f, 2f)] public float kutuGelisSuresi = 0.6f;

    [Header("Zaman (Matrix Modu) Ayarlarý")]
    [Range(0.1f, 3f)] public float yavaslamaSuresi = 0.8f; // Frene basma hýzý
    [Range(0.1f, 3f)] public float hizlanmaSuresi = 0.5f;  // Gaza basma hýzý
    public float enYavasZaman = 0.005f; // Oyun hýzý kaça düþsün?

    [Header("Konum Ayarlarý")]
    public float ekranDisiKonumY = -2000f; // Kutu aþaðýda nerede saklansýn?
    [Range(0f, 1f)] public float arkaplanHedefAlpha = 0.90f; // Siyah ekran ne kadar koyu olsun?

    // Özel deðiþkenler
    private QuestionData simdikiSoru;
    private float varsayilanFixedDeltaTime;
    private Sequence acilisSequence;
    private Tween zamanTween;

    private void Awake()
    {
        // Singleton yapýsý
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        anaPanelObjesi.SetActive(false);
        varsayilanFixedDeltaTime = 0.02f; // Unity varsayýlaný
    }

    public void OpenQuestionPanel(QuestionData data)
    {
        simdikiSoru = data;

        // Önceki animasyonlarý temizle
        if (acilisSequence != null) acilisSequence.Kill();
        if (zamanTween != null) zamanTween.Kill();

        // -- SIFIRLAMA (Reset) --
        // Arkaplaný siyaha ve tam þeffafa çek
        arkaplanGorseli.color = new Color(0, 0, 0, 0);
        // Kutuyu ekranýn altýna ýþýnla
        soruKutusuRect.anchoredPosition = new Vector2(0, ekranDisiKonumY);

        // Butonlarý aktif et
        foreach (var btn in sikButonlari) btn.interactable = true;

        // Paneli Aç
        anaPanelObjesi.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ýçerikleri Doldur
        soruText.text = data.soruMetni;
        if (data.soruSesi != null && sesKaynagi != null)
        {
            sesKaynagi.clip = data.soruSesi;
            sesKaynagi.Play();
        }
        ButonlariHazirla(data);

        // -- GÖRSEL ANÝMASYON (DOTween) --
        acilisSequence = DOTween.Sequence();
        acilisSequence.SetUpdate(true); // Zaman dursa bile çalýþ

        // 1. Arkaplan kararsýn
        acilisSequence.Append(arkaplanGorseli.DOFade(arkaplanHedefAlpha, arkaplanGelisSuresi));
        // 2. Kutu alttan gelsin (Hafif gecikmeli)
        acilisSequence.Join(soruKutusuRect.DOAnchorPosY(0, kutuGelisSuresi)
            .SetDelay(arkaplanGelisSuresi * 0.3f)
            .SetEase(Ease.OutBack));

        // -- MÜZÝK KISMA --
        if (GameMusicManager.Instance != null) GameMusicManager.Instance.SesiKis();

        // -- ZAMANI YAVAÞLATMA --
        zamanTween = DOVirtual.Float(Time.timeScale, enYavasZaman, yavaslamaSuresi, (x) =>
        {
            Time.timeScale = x;
            Time.fixedDeltaTime = varsayilanFixedDeltaTime * x;
        }).SetUpdate(true).SetEase(Ease.OutExpo);
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
        // Butonlarý kilitle (Tekrar basamasýnlar)
        foreach (var btn in sikButonlari) btn.interactable = false;

        if (secilenIndex == simdikiSoru.dogruCevapIndex)
        {
            // DOÐRU! Kutlamayý baþlat
            StartCoroutine(DogruCevapAnimasyonu());
        }
        else
        {
            // YANLIÞ!
            Debug.Log("YANLIÞ CEVAP");
            // Ýstersen buraya yanlýþ sesi/efekti de ekleyebilirsin
            PaneliKapat();
        }
    }

    // --- SENÝN ÝSTEDÝÐÝN ÖZEL YEÞÝL EFEKT ---
    IEnumerator DogruCevapAnimasyonu()
    {
        // 1. Yazýyý deðiþtir
        soruText.text = "DOÐRU BÝLDÝN!";

        // 2. "Ding" sesini çal
        if (sesKaynagi != null && dogruCevapSesi != null)
        {
            sesKaynagi.PlayOneShot(dogruCevapSesi);
        }

        // 3. Arkaplan Rengi Ayarla (Transparan Yeþil)
        Color seffafYesil = dogruRenk;
        seffafYesil.a = dogruCevapOpaklik; // Senin ayarladýðýn düþük alpha (0.3 gibi)
        arkaplanGorseli.color = seffafYesil;

        // 4. Nefes Alma Efekti (Yanýp Sönme)
        // Belirlediðin alpha'dan (0.3), neredeyse sýfýra (0.05) inip geri gelecek.
        // Göz yormayan yumuþak bir efekt.
        arkaplanGorseli.DOFade(0.05f, 0.25f)
            .SetLoops(6, LoopType.Yoyo)
            .SetUpdate(true);

        // 5. Kutlama süresi kadar bekle
        yield return new WaitForSecondsRealtime(kutlamaSuresi);

        // 6. Bitir
        PaneliKapat();
    }

    public void PaneliKapat()
    {
        if (acilisSequence != null) acilisSequence.Kill();
        if (zamanTween != null) zamanTween.Kill();

        // Kapanýþ Animasyonu (Hýzlýca aþaðý git ve kaybol)
        arkaplanGorseli.DOFade(0f, 0.3f).SetUpdate(true);
        soruKutusuRect.DOAnchorPosY(ekranDisiKonumY, 0.3f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => anaPanelObjesi.SetActive(false));

        // Müziði Geri Aç
        if (GameMusicManager.Instance != null) GameMusicManager.Instance.SesiAc();

        // Zamaný Normale Döndür (Hýzlý ivmeyle)
        zamanTween = DOVirtual.Float(Time.timeScale, 1.0f, hizlanmaSuresi, (x) =>
        {
            Time.timeScale = x;
            Time.fixedDeltaTime = varsayilanFixedDeltaTime * x;
        }).SetUpdate(true).SetEase(Ease.InSine);
    }
}