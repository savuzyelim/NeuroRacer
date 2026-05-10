using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DataManager.Instance.VerileriKaydet();
        }
    }
}
