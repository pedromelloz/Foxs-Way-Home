using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private float forcaPulo = 12f;

    [Header("Configurações de Dash")]
    [SerializeField] private float velocidadeDash = 24f;
    [SerializeField] private float duracaoDash = 0.2f;
    [SerializeField] private float cooldownDash = 1f;
    private bool podeDash = true;
    private bool estaNoDash;

    [Header("Inputs (Configure no Inspector)")]
    [SerializeField] private InputAction moverAcao;
    [SerializeField] private InputAction pularAcao;
    [SerializeField] private InputAction dashAcao;

    [Header("Verificação de Chão")]
    [SerializeField] private Transform checadorDeChao;
    [SerializeField] private LayerMask camadaChao;
    private bool estaNoChao;

    private Rigidbody2D rb;
    private float direcaoHorizontal;

    // --- COMPONENTES ADICIONADOS PARA ANIMAÇÃO ---
    private Animator anim;
    private SpriteRenderer sprite;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Pega as referências do Animator e do SpriteRenderer no GameObject da Raposa
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        moverAcao.Enable();
        pularAcao.Enable();
        dashAcao.Enable();
    }

    private void OnDisable()
    {
        moverAcao.Disable();
        pularAcao.Disable();
        dashAcao.Disable();
    }

    void Update()
    {
        // 1. Atualiza a checagem de chão constante
        estaNoChao = Physics2D.OverlapCircle(checadorDeChao.position, 0.2f, camadaChao);

        // 2. ATUALIZA AS ANIMAÇÕES NO ANIMATOR (Acontece sempre, mesmo durante o dash)
        AtualizarAnimacoes();

        // Se estiver no meio do dash, ignora os comandos normais de andar e pular
        if (estaNoDash) return;

        direcaoHorizontal = moverAcao.ReadValue<float>();

        // 3. VIRA A RAPOSA (FLIP DO SPRITE)
        if (direcaoHorizontal > 0)
        {
            sprite.flipX = false; // Olhando para a direita
        }
        else if (direcaoHorizontal < 0)
        {
            sprite.flipX = true;  // Olhando para a esquerda
        }

        if (pularAcao.WasPressedThisFrame() && estaNoChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }

        if (dashAcao.WasPressedThisFrame() && podeDash)
        {
            StartCoroutine(ExecutarDash());
        }
    }

    void FixedUpdate()
    {
        if (estaNoDash) return;

        rb.linearVelocity = new Vector2(direcaoHorizontal * velocidade, rb.linearVelocity.y);
    }

    // Método responsável por mandar as variáveis para o Animator Controller
    private void AtualizarAnimacoes()
    {
        if (anim == null) return;

        // Passa a velocidade absoluta no eixo X (Mathf.Abs transforma números negativos em positivos)
        anim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));

        // Passa se está tocando o chão
        anim.SetBool("isGrounded", estaNoChao);

        // Passa se está executando o dash
        anim.SetBool("isDashing", estaNoDash);
    }

    private IEnumerator ExecutarDash()
    {
        podeDash = false;
        estaNoDash = true;

        float gravidadeOriginal = rb.gravityScale;
        rb.gravityScale = 0f;

        // Se o flipX estiver ativo, a direção do dash vai para a esquerda (-1), senão vai para a direita (1)
        float direcaoDash = direcaoHorizontal != 0 ? Mathf.Sign(direcaoHorizontal) : (sprite.flipX ? -1f : 1f);

        rb.linearVelocity = new Vector2(direcaoDash * velocidadeDash, 0f);

        yield return new WaitForSeconds(duracaoDash);

        rb.gravityScale = gravidadeOriginal;
        estaNoDash = false;

        yield return new WaitForSeconds(cooldownDash);
        podeDash = true;
    }
}