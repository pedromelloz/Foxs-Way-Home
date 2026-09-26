using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bomba : MonoBehaviour
{
    [Header("Configurações da Explosão")]
    [SerializeField] private float tempoAteExplodir = 3f;
    [SerializeField] private float raioExplosao = 2.5f;
    [SerializeField] private int dano = 1;
    [SerializeField] private LayerMask camadaPlayer; // Selecione a camada do Player no Inspector

    [Header("Animação / Efeitos")]
    [SerializeField] private GameObject efeitoExplosaoPrefab; // Opcional: efeito visual de explosão

    private Rigidbody2D rb;
    private bool jaExplodiu = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Inicia a contagem regressiva para explodir
        Invoke(nameof(Explodir), tempoAteExplodir);
    }

    public void Lancar(Vector2 destino, float alturaArco)
    {
        Vector2 origem = transform.position;
        float deslocamentoX = destino.x - origem.x;
        float deslocamentoY = destino.y - origem.y;

        float gravidade = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float alturaMaxima = Mathf.Max(alturaArco, deslocamentoY + 0.5f);

        float velocidadeY = Mathf.Sqrt(2 * gravidade * alturaMaxima);
        float tempoSubida = velocidadeY / gravidade;
        float tempoDescida = Mathf.Sqrt(2 * (alturaMaxima - deslocamentoY) / gravidade);
        float tempoTotal = tempoSubida + tempoDescida;

        float velocidadeX = deslocamentoX / tempoTotal;

        rb.linearVelocity = new Vector2(velocidadeX, velocidadeY);
    }

    private void Explodir()
    {
        if (jaExplodiu) return;
        jaExplodiu = true;

        // 1. Instancia efeito visual de explosão se houver
        if (efeitoExplosaoPrefab != null)
        {
            Instantiate(efeitoExplosaoPrefab, transform.position, Quaternion.identity);
        }

        // 2. Busca qualquer colisor na camada do Player dentro do RAIO DE EXPLOSÃO
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, raioExplosao, camadaPlayer);

        if (hitPlayer != null)
        {
            // Tenta aplicar o dano na raposa
            VidaJogador player = hitPlayer.GetComponent<VidaJogador>();
            if (player != null)
            {
                player.TomarDano(1);
                Debug.Log("Bomba explodiu e causou dano na raposa!");
            }
        }

        // 3. Destrói o objeto da bomba
        Destroy(gameObject);
    }

    // Desenha o raio de alcance da explosão em Laranja na janela Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, raioExplosao);
    }
}