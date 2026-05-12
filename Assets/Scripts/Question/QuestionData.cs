using UnityEngine;

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