using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Morte : MonoBehaviour
{
    [Header("Configurações de Visão e Movimento")]
    [SerializeField] private float raioVisao = 8f;
    [SerializeField] private float velocidadeMovimento = 3f;
    [SerializeField] private float distanciaMinima = 0.5f;

    [Header("Ataque 1: Espadada (Perto)")]
    [SerializeField] private float raioAtaquePerto = 1.5f;
    [SerializeField] private float tempoEntreEspadadas = 1.2f;
    [SerializeField] private int danoEspada = 1;
    [SerializeField] private Transform pontoDeAtaque;
    [SerializeField] private float raioAlcanceEspada = 0.8f;

    [Header("Ataque 2: Orbe Perseguidor (Distância)")]
    [SerializeField] private float raioAtaqueOrbe = 5f;
    [SerializeField] private float tempoEntreOrbes = 3f;
    [Range(0, 100)] 
    [SerializeField] private float chanceLancarOrbe = 60f; // Porcentagem (0% a 100%)
    [SerializeField] private GameObject prefabOrbe;
    [SerializeField] private Transform pontoDeLancamentoOrbe;

    [Header("Referências")]
    [SerializeField] private Animator anim;
    [SerializeField] private Transform player;

    [Header("Tempo anim Orbe")]
    [SerializeField] private float tempinho;
    
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private bool playerNaVisao;
    
    private float cronometroEspada;
    private float cronometroOrbe;

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

        cronometroEspada = tempoEntreEspadadas;
        cronometroOrbe = tempoEntreOrbes;
    }

    void Update()
    {
        if (player == null) return;

        // Atualiza os cronômetros
        cronometroEspada += Time.deltaTime;
        cronometroOrbe += Time.deltaTime;

        float distanciaX = Mathf.Abs(transform.position.x - player.position.x);
        float distanciaTotal = Vector2.Distance(transform.position, player.position);

        // 1. ZONA 1: VISÃO DO INIMIGO
        if (distanciaTotal <= raioVisao)
        {
            if (!playerNaVisao)
            {
                playerNaVisao = true;
            }

            // Vira o sprite na direção do Player
            if (sprite != null)
            {
                sprite.flipX = transform.position.x < player.position.x;
            }

            // 2. ZONA 3: ATAQUE DE PERTO (Prioridade Máxima)
            if (distanciaTotal <= raioAtaquePerto)
            {
                PararMovimentoHorizontal();

                if (cronometroEspada >= tempoEntreEspadadas)
                {
                    AtacarComEspada();
                    cronometroEspada = 0f;
                }
            }
            // 3. ZONA 2: CHANCE DE ATACAR COM ORBE
            else if (distanciaTotal <= raioAtaqueOrbe && cronometroOrbe >= tempoEntreOrbes)
            {
                // Sorteia uma chance entre 0 e 100
                float sorteio = Random.Range(0f, 100f);

                if (sorteio <= chanceLancarOrbe)
                {
                    PararMovimentoHorizontal();
                    LancarOrbe();
                    cronometroOrbe = 0f;
                }
                else
                {
                    // Se falhou no sorteio, avança para tentar chegar mais perto
                    cronometroOrbe = 0f; // Reseta para tentar o sorteio novamente no próximo ciclo
                    MoverOuParar(distanciaX);
                }
            }
            // 4. FORA DAS ZONAS DE ATAQUE: Apenas Persegue
            else
            {
                MoverOuParar(distanciaX);
            }
        }
        else
        {
            if (playerNaVisao)
            {
                playerNaVisao = false;
            }

            PararMovimentoHorizontal();
            cronometroEspada = tempoEntreEspadadas;
            cronometroOrbe = tempoEntreOrbes;
        }
    }

    private void MoverOuParar(float distanciaX)
    {
        if (distanciaX > distanciaMinima)
        {
            MoverApenasHorizontal();
            if (anim != null) anim.SetBool("isMoving", true);
        }
        else
        {
            PararMovimentoHorizontal();
        }
    }

    private void MoverApenasHorizontal()
    {
        float direcao = player.position.x < transform.position.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(direcao * velocidadeMovimento, rb.linearVelocity.y);
    }

    private void PararMovimentoHorizontal()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetBool("isMoving", false);
        }
    }

    private void AtacarComEspada()
    {
        if (anim != null)
        {
            anim.SetTrigger("Atacar"); // Trigger de Espadada
        }

        Vector2 posicaoGolpe = pontoDeAtaque != null ? pontoDeAtaque.position : transform.position;
        Collider2D[] acertos = Physics2D.OverlapCircleAll(posicaoGolpe, raioAlcanceEspada);

        foreach (Collider2D colisor in acertos)
        {
            if (colisor.CompareTag("Player"))
            {
                VidaJogador scriptVida = GetComponent<VidaJogador>();
                scriptVida.TomarDano(danoEspada);
                break;
            }
        }
    }

    private void LancarOrbe()
    {

        if (anim != null)
        {
            anim.SetTrigger("LancarOrbe"); // Trigger de Lançar Orbe no Animator
        }

        StartCoroutine(EsperarETecar());
    }

    IEnumerator EsperarETecar()
    {
        yield return new WaitForSeconds(tempinho);
        if (prefabOrbe != null)
        {
            Vector2 posicaoLancamento = pontoDeLancamentoOrbe != null ? pontoDeLancamentoOrbe.position : transform.position;
            Instantiate(prefabOrbe, posicaoLancamento, Quaternion.identity);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        // 1. Visão do Inimigo (Vermelho)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioVisao);

        // 2. Chance de Tacar Orbe (Laranja)
        Gizmos.color = new Color(1f, 0.5f, 0f); // Laranja
        Gizmos.DrawWireSphere(transform.position, raioAtaqueOrbe);

        // 3. Ataque de Perto / Espada (Amarelo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioAtaquePerto);

        // Alcance da Espada (Ciano)
        Vector2 posicaoGolpe = pontoDeAtaque != null ? pontoDeAtaque.position : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(posicaoGolpe, raioAlcanceEspada);
    }
}