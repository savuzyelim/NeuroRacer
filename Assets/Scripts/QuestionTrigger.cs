using UnityEngine;

public class QuestionTrigger : MonoBehaviour
{
    [Header("Bu kutuda hangi soru çýkacak?")]
    public QuestionData soruVerisi; // Inspector'dan buraya soru dosyasýný sürükleyeceksin

    private bool isTriggered = false; // Sürekli tetiklenmesin diye kontrol

    private void OnTriggerEnter(Collider other)
    {
        // Sadece "Player" tag'ine sahip araba çarparsa ve daha önce tetiklenmemiþse
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true; // Bir daha tetiklenmesini engelle

            // Manager'a "Bu soruyu aç" diyoruz
            QuestionManager.Instance.OpenQuestionPanel(soruVerisi);

            // Ýstersen kutuyu tamamen yok edebilirsin:
            // Destroy(gameObject); 
        }
    }
}