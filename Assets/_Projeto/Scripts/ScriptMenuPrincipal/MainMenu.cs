
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Objetos do menu")]
    public GameObject botoesPrincipais;

    [Header("Paineis")]
    public GameObject painelConfig;
    public GameObject painelCredits;

    [Header("Efeito das folhas")]
    public EfeitoCliqueFolhas efeitoFolhas;

    [Header("Botoes")]
    public RectTransform botaoJogar;
    public RectTransform botaoConfig;
    public RectTransform botaoCredits;
    public RectTransform botaoSair;
    public RectTransform botaoVoltarConfig;
    public RectTransform botaoVoltarCredits;

    void Start()
    {
        VoltarMenu();
    }

    // ==================================
    // CLIQUES COM FOLHAS E ESCURECIMENTO
    // ==================================

    // JOGAR - Fade mais lento
    public void ClicarJogar()
    {
        efeitoFolhas.Reproduzir(
            botaoJogar,
            Jogar,
            0.8f,
            true
        );
    }

    // CONFIGURACOES - Fade rapido
    public void ClicarConfig()
    {
        efeitoFolhas.Reproduzir(
            botaoConfig,
            AbrirConfig,
            0.25f
        );
    }

    // CREDITOS - Fade rapido
    public void ClicarCredits()
    {
        efeitoFolhas.Reproduzir(
            botaoCredits,
            AbrirCredits,
            0.25f
        );
    }

    // SAIR - Fade rapido
    public void ClicarSair()
    {
        efeitoFolhas.Reproduzir(
            botaoSair,
            Sair,
            0.25f,
            true
        );
    }

    // VOLTAR DAS CONFIGURACOES
    public void ClicarVoltarConfig()
    {
        efeitoFolhas.Reproduzir(
            botaoVoltarConfig,
            VoltarMenu,
            0.25f
        );
    }

    // VOLTAR DOS CREDITOS
    public void ClicarVoltarCredits()
    {
        efeitoFolhas.Reproduzir(
            botaoVoltarCredits,
            VoltarMenu,
            0.25f
        );
    }

    // ==================================
    // FUNCOES ORIGINAIS DO MENU
    // ==================================

    public void Jogar()
    {
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
