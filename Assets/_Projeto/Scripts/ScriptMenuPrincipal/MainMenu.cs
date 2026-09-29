using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Objetos do menu")]
    public GameObject botoesPrincipais;

    [Header("Paineis")]
    public GameObject painelConfig;
    public GameObject painelCredits;

    void Start()
    {
        VoltarMenu();
    }

    public void Jogar()
    {
        // Por enquanto, inicia a primeira fase.
        // Depois mudaremos para IntroCutscene.
        SceneManager.LoadScene("IntroCutscene");
    }

    public void AbrirConfig()
    {
        botoesPrincipais.SetActive(false);
        painelCredits.SetActive(false);
        painelConfig.SetActive(true);
    }

    public void AbrirCredits()
    {
        botoesPrincipais.SetActive(false);
        painelConfig.SetActive(false);
        painelCredits.SetActive(true);
    }

    public void VoltarMenu()
    {
        painelConfig.SetActive(false);
        painelCredits.SetActive(false);
        botoesPrincipais.SetActive(true);
    }

    public void Sair()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}