using UnityEngine;

public class Info : MonoBehaviour
{
    public GameObject info;
    public GameObject gameField;

    public void ShowInfo()
    {
        info.SetActive(true);
        gameField.SetActive(false);
    }

    public void HideInfo()
    {
        info.SetActive(false);
        gameField.SetActive(true);
    }
}
