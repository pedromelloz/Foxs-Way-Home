using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Configuração de Velocidade")]
    [Tooltip("0 = Segue a câmera perfeitamente (Fundo Fixo). 1 = Não se move com a câmera (Efeito Infinito Distante).")]
    [Range(0f, 1f)] 
    [SerializeField] private float efeitoParallax; 

    private Transform cameraTransform;
    private Vector3 ultimaPosicaoCamera;
    private float tamanhoDaImagemX;

    void Start()
    {
        // Pega a câmera principal do jogo automaticamente
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            ultimaPosicaoCamera = cameraTransform.position;
        }

        // Pega a largura do Sprite para saber quando a imagem precisa "dar o looping"
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            tamanhoDaImagemX = spriteRenderer.bounds.size.x;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // 1. Calcula o quanto a câmera se moveu desde o último frame
        Vector3 movimentoDaCamera = cameraTransform.position - ultimaPosicaoCamera;

        // 2. Move o fundo baseado no fator de parallax (multiplica o movimento da câmera)
        // Movemos apenas no eixo X para jogos de plataforma convencionais
        transform.position += new Vector3(movimentoDaCamera.x * (1f - efeitoParallax), 0f, 0f);

        // Atualiza a última posição da câmera para o próximo frame
        ultimaPosicaoCamera = cameraTransform.position;

        // 3. Efeito de Looping Infinito (Reposiciona o fundo caso a câmera passe dele)
        float distanciaMovidaRelativa = cameraTransform.position.x * efeitoParallax;

        if (distanciaMovidaRelativa > transform.position.x + (tamanhoDaImagemX / 2f))
        {
            transform.position += new Vector3(tamanhoDaImagemX, 0f, 0f);
        }
        else if (distanciaMovidaRelativa < transform.position.x - (tamanhoDaImagemX / 2f))
        {
            transform.position -= new Vector3(tamanhoDaImagemX, 0f, 0f);
        }
    }
}