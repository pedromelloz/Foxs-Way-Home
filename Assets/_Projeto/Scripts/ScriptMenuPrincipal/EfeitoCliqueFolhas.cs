
using System;
using System.Collections;
using UnityEngine;

public class EfeitoCliqueFolhas : MonoBehaviour
{
    [Header("Folhas")]
    public GameObject folhasClique;
    public float tempoAnimacao = 5f;

    [Header("Escurecimento da tela")]
    public CanvasGroup fadeTela;

    private bool animando = false;
    private AudioSource audioClique;

    private void Awake()
    {
        audioClique = gameObject.AddComponent<AudioSource>();
        audioClique.clip = Resources.Load<AudioClip>("AudioMenu/CliqueFolhasPingPong");
        audioClique.playOnAwake = false;
        audioClique.loop = false;
        audioClique.spatialBlend = 0f;
    }

    public void Reproduzir(
        RectTransform botao,
        Action acao,
        float duracaoFade = 0.25f,
        bool manterEscuro = false,
        GameObject folhasAlternativas = null)
    {
        if (!animando)
        {
            StartCoroutine(Executar(
                botao, acao, duracaoFade, manterEscuro, folhasAlternativas));
        }
    }

    private IEnumerator Executar(
        RectTransform botao,
        Action acao,
        float duracaoFade,
        bool manterEscuro,
        GameObject folhasAlternativas)
    {
        animando = true;
        GameObject folhasAtivas = folhasAlternativas != null ? folhasAlternativas : folhasClique;
        if (folhasAtivas == null || botao == null)
        {
            animando = false;
            yield break;
        }

        // Posiciona as folhas no botão clicado.
        RectTransform efeito =
            folhasAtivas.GetComponent<RectTransform>();

        efeito.position =
            botao.TransformPoint(botao.rect.center);

        // Reinicia as cinco animações.
        folhasAtivas.SetActive(false);
        folhasAtivas.SetActive(true);
        // O audio acompanha diretamente o inicio da animacao.
        if (audioClique != null && audioClique.clip != null)
        {
            audioClique.Stop();
            audioClique.volume = PlayerPrefs.GetInt("FoxAudio_EfeitosLigados", 1) == 1
                ? 0.42f * PlayerPrefs.GetFloat("FoxAudio_Efeitos", 1f) : 0f;
            audioClique.Play();
        }

        foreach (Animator animator in
            folhasAtivas.GetComponentsInChildren<Animator>())
        {
            animator.Rebind();
            animator.Update(0f);
        }

        // Espera até perto do final da queda.
        yield return new WaitForSeconds(
            Mathf.Max(0f, tempoAnimacao - duracaoFade));

        // Escurece a tela suavemente.
        yield return StartCoroutine(
            AnimarFade(0f, 1f, duracaoFade));

        // Esconde as folhas.
        folhasAtivas.SetActive(false);
        if (audioClique != null) audioClique.Stop();

        // Executa a função original do botão.
        acao?.Invoke();

        // Clareia novamente ao abrir painéis.
        if (!manterEscuro)
        {
            yield return StartCoroutine(
                AnimarFade(1f, 0f, duracaoFade));
        }

        animando = false;
    }

    private IEnumerator AnimarFade(
        float inicio,
        float fim,
        float duracao)
    {
        float tempo = 0f;

        if (duracao <= 0f)
        {
            fadeTela.alpha = fim;
            yield break;
        }

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float progresso =
                Mathf.Clamp01(tempo / duracao);

            fadeTela.alpha =
                Mathf.Lerp(inicio, fim, progresso);

            yield return null;
        }

        fadeTela.alpha = fim;
    }
}
