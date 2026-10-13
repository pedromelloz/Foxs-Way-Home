
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

    public void Reproduzir(
        RectTransform botao,
        Action acao,
        float duracaoFade = 0.25f,
        bool manterEscuro = false)
    {
        if (!animando)
        {
            StartCoroutine(Executar(
                botao, acao, duracaoFade, manterEscuro));
        }
    }

    private IEnumerator Executar(
        RectTransform botao,
        Action acao,
        float duracaoFade,
        bool manterEscuro)
    {
        animando = true;

        // Posiciona as folhas no botão clicado.
        RectTransform efeito =
            folhasClique.GetComponent<RectTransform>();

        efeito.position =
            botao.TransformPoint(botao.rect.center);

        // Reinicia as cinco animações.
        folhasClique.SetActive(false);
        folhasClique.SetActive(true);

        foreach (Animator animator in
            folhasClique.GetComponentsInChildren<Animator>())
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
        folhasClique.SetActive(false);

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
