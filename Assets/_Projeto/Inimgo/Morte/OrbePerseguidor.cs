using UnityEngine;

public class OrbePerseguidor : MonoBehaviour
{
    [SerializeField] private float velocidade = 4f;
    [SerializeField] private float tempoDeVida = 5f;
    [SerializeField] private int dano = 1;

    private Transform player;

    void Start()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null)
        {
            player = objPlayer.transform;
        }

        // Destrói o orbe automaticamente após alguns segundos
        Destroy(gameObject, tempoDeVida);
    }

    void Update()
    {
        if (player != null)
        {
            // Persegue a posição do player continuamente
            transform.position = Vector2.MoveTowards(transform.position, player.position, velocidade * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //VidaJogador = GetComponent<VidaJogador>();
            VidaJogador scriptVida = collision.GetComponent<VidaJogador>();
            scriptVida.TomarDano(dano);
            Destroy(gameObject); // Somente destrói ao acertar o Player
        }
        else if (collision.CompareTag("Chao")) // Opcional: destrói ao bater em paredes/chão
        {
            Destroy(gameObject);
        }
    }
}