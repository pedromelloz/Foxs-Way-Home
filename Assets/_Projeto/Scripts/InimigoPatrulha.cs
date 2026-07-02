using UnityEngine;

public class InimigoPatrulha : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float tempoAndando = 2f; 
    private float cronometro;
    private bool andandoParaDireita = true;

    [Header("Configurações de Ataque")]
    [SerializeField] private int danoNoPlayer = 1;
    [SerializeField] private float tempoEntreAtaques = 1f; // Tempo para o player não morrer na hora
    private float cronometroAtaque;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cronometro = tempoAndando;
    }

    void Update()
    {
        // Controla o tempo de patrulha
        cronometro -= Time.deltaTime;
        if (cronometro <= 0)
        {
            andandoParaDireita = !andandoParaDireita;
            cronometro = tempoAndando;
            InverterSprite();
        }

        // Controla o tempo de recarga do dano
        if (cronometroAtaque > 0)
        {
            cronometroAtaque -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        float direcaoX = andandoParaDireita ? 1f : -1f;
        rb.linearVelocity = new Vector2(direcaoX * velocidade, rb.linearVelocity.y);
    }

    // Usamos OnTriggerStay2D para detectar se o player CONTINUA encostado nele
    private void OnTriggerStay2D(Collider2D colisor)
    {
        // Só dá dano se o tempo de recarga já tiver zerado
        if (cronometroAtaque <= 0 && colisor.CompareTag("Jogador"))
        {
            if (colisor.TryGetComponent<VidaJogador>(out VidaJogador vidaDoPlayer))
            {
                vidaDoPlayer.TomarDano(danoNoPlayer);
                cronometroAtaque = tempoEntreAtaques; // Inicia o tempo de espera para o próximo dano
            }
        }
    }

    private void InverterSprite()
    {
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}