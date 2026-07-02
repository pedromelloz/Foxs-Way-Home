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
    [SerializeField] private InputAction dashAcao; // Nova ação para o Dash

    [Header("Verificação de Chão")]
    [SerializeField] private Transform checadorDeChao;
    [SerializeField] private LayerMask camadaChao;
    private bool estaNoChao;

    private Rigidbody2D rb;
    private float direcaoHorizontal;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moverAcao.Enable();
        pularAcao.Enable();
        dashAcao.Enable(); // Não esqueça de ativar!
    }

    private void OnDisable()
    {
        moverAcao.Disable();
        pularAcao.Disable();
        dashAcao.Disable();
    }

    void Update()
    {
        // Se estiver no meio do dash, ignora os comandos normais de andar e pular
        if (estaNoDash) return;

        direcaoHorizontal = moverAcao.ReadValue<float>();

        estaNoChao = Physics2D.OverlapCircle(checadorDeChao.position, 0.2f, camadaChao);

        if (pularAcao.WasPressedThisFrame() && estaNoChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }

        // Verifica se apertou o botão de Dash e se ele não está no cooldown
        if (dashAcao.WasPressedThisFrame() && podeDash)
        {
            StartCoroutine(ExecutarDash());
        }
    }

    void FixedUpdate()
    {
        // Se estiver no meio do dash, a física do FixedUpdate normal é ignorada
        if (estaNoDash) return;

        rb.linearVelocity = new Vector2(direcaoHorizontal * velocidade, rb.linearVelocity.y);
    }

    // Corotina que controla o tempo e a força do Dash
    private IEnumerator ExecutarDash()
    {
        podeDash = false;
        estaNoDash = true;

        // Guarda a gravidade original e zera ela para o player não cair durante o dash
        float gravidadeOriginal = rb.gravityScale;
        rb.gravityScale = 0f;

        // Descobre para onde o jogador está olhando. Se não estiver apertando nada, usa a direção padrão (1 ou -1)
        float direcaoDash = direcaoHorizontal != 0 ? Mathf.Sign(direcaoHorizontal) : (transform.localScale.x > 0 ? 1 : -1);

        // Aplica a velocidade extrema do dash no eixo X
        rb.linearVelocity = new Vector2(direcaoDash * velocidadeDash, 0f);

        // Espera o tempo de duração do dash acabar
        yield return new WaitForSeconds(duracaoDash);

        // Devolve a gravidade normal e avisa que o dash acabou
        rb.gravityScale = gravidadeOriginal;
        estaNoDash = false;

        // Espera o tempo de recarga (cooldown) para permitir o próximo dash
        yield return new WaitForSeconds(cooldownDash);
        podeDash = true;
    }
}