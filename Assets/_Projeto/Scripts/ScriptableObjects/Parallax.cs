using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform camara;
    private Vector3 posicaoAnteriorCamara;

    [Header("Efeito Parallax (0 = parado, 1 = move junto da câmera)")]
    [Range(0f, 1f)]
    [SerializeField] private float efeitoParallax = 0.5f;

    void Start()
    {
        // Pega a câmera principal automaticamente
        camara = Camera.main.transform;
        posicaoAnteriorCamara = camara.position;
    }

    void LateUpdate()
    {
        if (camara == null) return;

        // Calcula a variação de movimento da câmera
        Vector3 deltaCamara = camara.position - posicaoAnteriorCamara;

        // Move a camada de fundo proporcionalmente
        transform.position += new Vector3(deltaCamara.x * efeitoParallax, deltaCamara.y * efeitoParallax, 0f);

        posicaoAnteriorCamara = camara.position;
    }
}