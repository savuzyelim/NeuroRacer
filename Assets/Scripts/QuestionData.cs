using UnityEngine;

[CreateAssetMenu(fileName = "YeniSoru", menuName = "NeuroRacer/Soru")]
public class QuestionData : ScriptableObject
{
    [TextArea] public string soruMetni; // Sorunun kendisi
    public string[] siklar;             // A, B, C þýklarý
    public int dogruCevapIndex;         // 0=A, 1=B, 2=C...
    public AudioClip soruSesi;          // Varsa ses dosyasý
}