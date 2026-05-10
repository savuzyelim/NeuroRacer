using System.Collections.Generic;
using UnityEngine;

public class AdaptiveQuestionSelector : MonoBehaviour
{
    public List<QuestionData> tumSorular;

    public QuestionData SiradakiSoruyuGetir()
    {
        // DataManager'daki loglara bak
        int hataSayisiBD = 0;
        foreach (var log in DataManager.Instance.tumLoglar)
        {
            if (!log.dogruMu && log.sorulanKelime.Contains("b")) hataSayisiBD++;
        }

        // Eðer b-d harflerinde çok hata varsa, o kategoriden soru seç
        if (hataSayisiBD > 2)
        {
            return tumSorular.Find(x => x.kategori == "b-d karisikligi");
        }

        // Deðilse, rastgele veya seviyeye göre devam et
        return tumSorular[Random.Range(0, tumSorular.Count)];
    }
}