using UnityEngine;

public class PlayerController : MonoBehaviour // Nome em inglês é padrão
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("References")]
    public Rigidbody2D rb; // Variável pública para aparecer no Inspector
    private bool isGrounded = false;

    void Start()
    {
        // Pega o Rigidbody2D automaticamente se não arrastar manualmente
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("Error: Rigidbody2D not found on Player!");
        }
    }

    void Update()
    {
        // Leitura de Input (Setas ou A/D)
        float inputX = Input.GetAxis("Horizontal");
        
        // Aplica velocidade no eixo X
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        // Pulo (Espaço)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica a Tag "Chao" (que você criou como 'Chao' ou 'Chao')
        if (collision.gameObject.CompareTag("Chao"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chao"))
        {
            isGrounded = false;
        }
    }
}