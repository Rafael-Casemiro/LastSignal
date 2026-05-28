using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastSignal.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartGame()
        {
            // Carrega a cena principal do jogo
            SceneManager.LoadScene("SampleScene");
        }
    }
}
