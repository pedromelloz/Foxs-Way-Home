using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Monstro_musgo : MonoBehaviour
{
    [Header("Configurações de Visão e Movimento")]
    [SerializeField] private float raioVisao = 6f;
    [SerializeField] private float velocidadeMovimento = 3f;
    [SerializeField] private float distanciaMinima = 0.5f;

    [Header("Configurações de Pulo")]
    [SerializeField] private float forcaPulo = 7f;
    [SerializeField] private float alturaMinimaParaPular = 1.2f; // Diferença de altura (Y) necessária para pular
    [SerializeField] private Transform verificadorDeChao; // Objeto filho no pé do monstro
    [SerializeField] private float raioChao = 0.2f;
    [SerializeField] private LayerMask camadaChao; // Selecione a Layer do Chão no Inspector

    [Header("Referências")]
    [SerializeField] private Animator anim;
    [SerializeField] private Transform player;
    
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private bool playerNaVisao;
    private bool estaNoChao;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        if (player == null)
        {
            GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
            if (objPlayer != null)
            {
                player = objPlayer.transform;
            }
        }
    }

    void Update()
    {
        // 1. Verifica se está tocando o chão
        ChecarChao();

        if (player == null) return;

        float distanciaX = Mathf.Abs(transform.position.x - player.position.x);
        float distanciaTotal = Vector2.Distance(transform.position, player.position);

        if (distanciaTotal <= raioVisao)
        {
            if (!playerNaVisao)
            {
                playerNaVisao = true;
                Debug.Log("Player ENTROU na visão!");
            }

            // Vira o sprite para o lado correto
            if (sprite != null)
            {
                sprite.flipX = player.position.x < transform.position.x;
            }

            // Move na horizontal se estiver distante no eixo X
            if (distanciaX > distanciaMinima)
            {
                MoverApenasHorizontal();

                if (anim != null)
                {
                    anim.SetBool("isMoving", true);
                }
            }
            else
            {
                PararMovimentoHorizontal();
            }

            // 2. Lógica de Pulo: Se o player estiver mais alto e o monstro estiver no chão
            float diferencaAlturaY = player.position.y - transform.position.y;

            if (diferencaAlturaY > alturaMinimaParaPular && estaNoChao)
            {
                Pular();
            }
        }
        else
        {
            if (playerNaVisao)
            {
                playerNaVisao = false;
                Debug.Log("Player SAIU da visão!");
            }

            PararMovimentoHorizontal();
        }
    }

    private void MoverApenasHorizontal()
    {
        float direcao = player.position.x < transform.position.x ? -1f : 1f;
        
        // Mantém a velocidade 'y' atual (queda/pulo) e altera apenas o 'x'
        Vector2 vel = rb.linearVelocity;
        vel.x = direcao * velocidadeMovimento;
        rb.linearVelocity = vel;
    }

    private void PararMovimentoHorizontal()
    {
        Vector2 vel = rb.linearVelocity;
        vel.x = 0f;
        rb.linearVelocity = vel;

        if (anim != null)
        {
            anim.SetBool("isMoving", false);
        }
    }

    private void Pular()
    {
        Vector2 vel = rb.linearVelocity;
        vel.y = forcaPulo;
        rb.linearVelocity = vel;

        if (anim != null)
        {
            anim.SetTrigger("Pular"); // Trigger opcional para animação de pulo
        }
    }

    private void ChecarChao()
    {
        if (verificadorDeChao != null)
        {
            estaNoChao = Physics2D.OverlapCircle(verificadorDeChao.position, raioChao, camadaChao);
        }
        else
        {
            // Caso não tenha atribuído o 'verificadorDeChao', faz o teste a partir da própria posição do monstro
            estaNoChao = Physics2D.OverlapCircle(transform.position, raioChao, camadaChao);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Raio de visão (Vermelho)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioVisao);

        // Raio de verificação do chão (Azul)
        if (verificadorDeChao != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(verificadorDeChao.position, raioChao);
        }
    }
}