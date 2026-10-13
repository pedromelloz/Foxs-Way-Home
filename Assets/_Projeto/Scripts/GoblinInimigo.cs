using UnityEngine;

public class GoblinEstatico : MonoBehaviour
{
    [Header("Configurações de Lançamento")]
    [SerializeField] private GameObject prefabBomba;
    [SerializeField] private Transform pontoDeLancamento;
    [SerializeField] private Vector2 forcaLanco = new Vector2(6f, 4f);
    
    [Header("Distância e Alcance")]
    [SerializeField] private float distanciaAtaque = 8f; // Variável para definir o alcance exato
    [SerializeField] private float tempoEntreAtaques = 2.5f;

    [Header("Áudio do Goblin")]
    [SerializeField] private AudioSource fonteAudio;
    [SerializeField] private AudioClip somAtaqueGoblin; // Som de risada/grito de ataque

    [Header("Referências")]
    [SerializeField] private Transform player;
    private Animator anim;
    private SpriteRenderer sprite;
    private float tempoProximoAtaque;

    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        if (fonteAudio == null)
        {
            fonteAudio = GetComponent<AudioSource>();
        }

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calcula a distância atual até o Player
        float distanciaAtePlayer = Vector2.Distance(transform.position, player.position);

        // Vira o sprite para o lado correto da raposa
        if (player.position.x < transform.position.x)
        {
            sprite.flipX = true; // Esquerda
        }
        else
        {
            sprite.flipX = false; // Direita
        }

        // Se a raposa estiver DENTRO da distância limite e o tempo de recarga passou
        if (distanciaAtePlayer <= distanciaAtaque && Time.time >= tempoProximoAtaque)
        {
            IniciarAtaqueESom();
            tempoProximoAtaque = Time.time + tempoEntreAtaques;
        }
    }

    private void IniciarAtaqueESom()
    {
        // 1. Toca o som do Goblin no momento em que ele detecta e decide atacar
        if (fonteAudio != null && somAtaqueGoblin != null)
        {
            fonteAudio.PlayOneShot(somAtaqueGoblin);
        }

        // 2. Dispara a animação
        if (anim != null)
        {
            anim.SetTrigger("atacar");
        }
        else
        {
            // Se não tiver animação, lança a bomba direto
            LançarBomba();
        }
    }

    // Método chamado pelo Animation Event no frame do arremesso (ou direto pelo código)
    public void LançarBomba()
    {
        if (prefabBomba == null || pontoDeLancamento == null) return;

        GameObject bombaInstanciada = Instantiate(prefabBomba, pontoDeLancamento.position, Quaternion.identity);
        Rigidbody2D rbBomba = bombaInstanciada.GetComponent<Rigidbody2D>();

        if (rbBomba != null)
        {
            float direcao = sprite.flipX ? -1f : 1f;
            Vector2 forcaFinal = new Vector2(forcaLanco.x * direcao, forcaLanco.y);
            
            rbBomba.AddForce(forcaFinal, ForceMode2D.Impulse);
        }
    }

    // Desenha um círculo vermelho no Editor para visualizar o limite da distância do ataque
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}