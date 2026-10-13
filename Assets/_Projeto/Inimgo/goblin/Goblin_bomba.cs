using UnityEngine;

public class Goblin_bomba : MonoBehaviour
{
    [Header("Configurações de Visão")]
    [SerializeField] private float raioVisao = 6f;

    [Header("Ataque com Bomba")]
    [SerializeField] private GameObject Bomba;
    [SerializeField] private Transform pontoDeLancamento;
    [SerializeField] private float tempoEntreAtaques = 2f;
    [SerializeField] private float alturaDoArco = 3f;
    [SerializeField] private Animator anim;

    [Header("Referências")]
    [SerializeField] private Transform player;
    
    private SpriteRenderer sprite;
    private bool playerNaVisao;
    private float cronometroAtaque;


    void Start()
    {
        Animator anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

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
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= raioVisao)
        {
            if (!playerNaVisao)
            {
                playerNaVisao = true;
                Debug.Log("Player ENTROU na visão!");
            }

            // Vira o sprite na direção do Player
            if (sprite != null)
            {
                sprite.flipX = player.position.x < transform.position.x;
            }

            // Lógica do temporizador de ataque
            anim.SetBool("Atacou", true);
            cronometroAtaque += Time.deltaTime;
            if (cronometroAtaque >= tempoEntreAtaques)
            {
                AtacarComBomba();
                cronometroAtaque = 0f;
            }
        }
        else
        {
            if (playerNaVisao)
            {
                playerNaVisao = false;
                Debug.Log("Player SAIU da visão!");
                cronometroAtaque = 0;
                anim.SetBool("Atacou", false);
            }

            // Reseta o cronômetro para atacar imediatamente quando o player voltar a entrar na visão
            //cronometroAtaque = tempoEntreAtaques;
        }
    }

    private void AtacarComBomba()
    {
        if (Bomba == null)
        {
            Debug.LogWarning("Prefab da Bomba não foi atribuído no Inspector do Inimigo!");
            return;
        }

        // Ponto de origem do lançamento (usa a própria posição do inimigo se o ponto não for especificado)
        Vector2 posicaoOrigem = pontoDeLancamento != null ? pontoDeLancamento.position : transform.position;
        // Instancia a bomba
        GameObject novaBombaObj = Instantiate(Bomba, posicaoOrigem, Quaternion.identity);
        Bomba bombaScript = novaBombaObj.GetComponent<Bomba>();

        if (bombaScript != null)
        {
            // Lança a bomba em arco até a posição atual do Player
            bombaScript.Lancar(player.position, alturaDoArco);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioVisao);
    }
}