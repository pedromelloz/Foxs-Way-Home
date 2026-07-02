using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Importante para controlar as imagens da UI

public class VidaJogador : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private int vidaMaxima = 3;
    private int vidaAtual;

    [Header("Interface de Vidas (UI)")]
    // Arraste os 3 objetos de imagem de coração para esta lista no Inspector
    [SerializeField] private Image[] iconesCoracao; 

    void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarInterfaceVida();
    }

    public void TomarDano(int quantidade)
    {
        vidaAtual -= quantidade;
        AtualizarInterfaceVida();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    // Método que liga/desliga os corações na tela baseado na vida atual
    private void AtualizarInterfaceVida()
    {
        for (int i = 0; i < iconesCoracao.Length; i++)
        {
            if (i < vidaAtual)
            {
                iconesCoracao[i].enabled = true; // Mostra o coração
            }
            else
            {
                iconesCoracao[i].enabled = false; // Esconde o coração
            }
        }
    }

    private void Morrer()
    {
        int cenaAtual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(cenaAtual);
    }
}