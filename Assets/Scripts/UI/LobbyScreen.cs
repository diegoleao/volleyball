using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyScreen : MonoBehaviour
{
    [SerializeField] Menu MenuPrefab;

    public void BackToMenu()
    {
        this.Close();
        Instantiate(MenuPrefab);
    }

    public void Close()
    {
        Destroy(gameObject);
    }

}