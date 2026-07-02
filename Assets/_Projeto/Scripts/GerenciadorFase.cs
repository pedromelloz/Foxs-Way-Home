using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para mudar de fase
using TMPro; // Se você estiver usando TextMeshPro para o texto

public class GerenciadorFase : MonoBehaviour
{
    [Header("Configurações Visuais")]
    [SerializeField] private GameObject painelVitoria; // O painel ou texto que vai aparecer
    [SerializeField] private float tempoDeEspera = 3f;   // Quanto tempo o jogo fica congelado antes de mudar de fase
    private bool jaGanhou = false;

    private void Awake()
    {
        // Garante que o painel de vitória comece escondido quando a fase iniciar
        if (painelVitoria != null)
        {
            painelVitoria.SetActive(false);
        }
    }

    // Método que detecta quando o jogador encosta no objeto de vitória (ex: uma bandeira, portal)
    private void OnTriggerEnter2D(Collider2D colisor)
    {
        // Verifica se quem encostou tem a Tag "Jogador" e se o jogador já não ganhou antes
        if (colisor.CompareTag("Jogador") && !jaGanhou)
        {
            StartCoroutine(IniciarVitoria());
        }
    }

    private IEnumerator IniciarVitoria()
    {
        jaGanhou = true;

        // 1. Mostra o letreiro/painel na tela
        if (painelVitoria != null)
        {
            painelVitoria.SetActive(true);
        }

        // 2. Congela o tempo do jogo (física, movimentos e updates param)
        Time.timeScale = 0f;

        // 3. Espera o tempo determinado (usamos WaitForSecondsRealtime porque o tempo normal está congelado)
        yield return new WaitForSecondsRealtime(tempoDeEspera);

        // 4. Descongela o tempo para a próxima fase não começar travada
        Time.timeScale = 1f;

        // 5. Carrega a próxima fase na fila do Build Settings
        SceneManager.LoadScene(1);
    }
}