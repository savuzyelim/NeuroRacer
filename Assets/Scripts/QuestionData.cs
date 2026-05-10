using UnityEngine;

[CreateAssetMenu(fileName = "Yeni Soru", menuName = "NeuroRacer/Soru")]
public class QuestionData : ScriptableObject
{
    public string soruMetni;
    public string[] siklar;
    public int dogruCevapIndex;
    public AudioClip soruSesi;
    public string kategori; // Örn: "b-d karisikligi"
    public int zorlukSeviyesi; // 1, 2, 3
}