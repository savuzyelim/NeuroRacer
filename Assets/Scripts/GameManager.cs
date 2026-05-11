using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    [SerializeField] MenuStatsManager menuManager;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DataManager.Instance.VerileriKaydet();
        }
    }

    public void OpenClosePanels()
    {
        if(panels[0].activeSelf == true)
        {
            menuManager.IstatistikleriGoster();
            panels[0].SetActive(false);
            panels[1].SetActive(true);
        }
        else if(panels[0].activeSelf == false)
        {
            panels[0].SetActive(true);
            panels[1].SetActive(false);
        }
    }

    public void PlayTheGame()
    {
        SceneManager.LoadScene("YarýþSahne");
    }
}
