using UnityEngine;

public class ParallaxBackground : MonoBehaviour 
{
    public Transform cam; // Sua Câmera Principal

    [Header("Fundos do Fundo (Lento)")]
    // Mudado para aceitar uma lista/array de várias imagens de fundo!
    public Renderer[] fundosTras; 
    public float velocidadeTras = 0.05f;

    [Header("Fundo da Frente (Mais Rápido)")]
    public Renderer backgroundFrente;
    public float velocidadeFrente = 0.3f;

    private Vector3 lastCamPosition;
    private Material[] matTras;
    private Material matFrente;

    void Start() 
    {
        if (cam == null) 
        {
            cam = Camera.main.transform;
        }

        lastCamPosition = cam.position;

        // Guarda os materiais dos fundos de trás
        if (fundosTras != null && fundosTras.Length > 0)
        {
            matTras = new Material[fundosTras.Length];
            for (int i = 0; i < fundosTras.Length; i++)
            {
                if (fundosTras[i] != null)
                {
                    matTras[i] = fundosTras[i].material;
                }
            }
        }

        // Guarda o material do fundo da frente
        if (backgroundFrente != null) 
        {
            matFrente = backgroundFrente.material;
        }
    }

    void LateUpdate() 
    {
        // 1. Calcula o quanto a câmera se moveu desde o último frame
        Vector3 deltaMovement = cam.position - lastCamPosition;

        // 2. Faz todos os fundos de trás acompanharem a câmera e deslizarem a textura
        if (fundosTras != null) 
        {
            for (int i = 0; i < fundosTras.Length; i++)
            {
                if (fundosTras[i] != null)
                {
                    // Preserva o deslocamento X original do objeto em relação à câmera
                    float offsetOriginalX = fundosTras[i].transform.position.x - lastCamPosition.x;
                    
                    fundosTras[i].transform.position = new Vector3(
                        cam.position.x + offsetOriginalX, 
                        cam.position.y, 
                        fundosTras[i].transform.position.z
                    );

                    if (matTras != null && matTras[i] != null) 
                    {
                        Vector2 offset = matTras[i].mainTextureOffset;
                        offset.x += deltaMovement.x * velocidadeTras;
                        matTras[i].mainTextureOffset = offset;
                    }
                }
            }
        }

        // 3. Faz o fundo da frente acompanhar a câmera e deslizar a textura
        if (backgroundFrente != null) 
        {
            backgroundFrente.transform.position = new Vector3(
                cam.position.x, 
                cam.position.y, 
                backgroundFrente.transform.position.z
            );
            
            if (matFrente != null) 
            {
                Vector2 offset = matFrente.mainTextureOffset;
                offset.x += deltaMovement.x * velocidadeFrente;
                matFrente.mainTextureOffset = offset;
            }
        }

        lastCamPosition = cam.position;
    }
}