using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Texto da historia")]
    public TMP_Text textoHistoria;

    [Header("Duracao de cada parte")]
    public float tempoPorParte = 5f;

    private bool terminou = false;

    private string[] historia =
    {
        "Em uma floresta distante, uma pequena raposa vivia feliz com seus pais.",

        "Certo dia, enquanto explorava a floresta, uma forte tempestade começou...",

        "Assustada, a raposinha correu para se abrigar. Quando a tempestade passou, seus pais haviam desaparecido.",

        "Agora, sozinha, ela precisa enfrentar os perigos da floresta para encontrar o caminho de volta para casa."
    };

    void Start()
    {
        StartCoroutine(ReproduzirHistoria());
    }

    IEnumerator ReproduzirHistoria()
    {
        foreach (string parte in historia)
        {
            textoHistoria.text = parte;

            yield return new WaitForSeconds(tempoPorParte);
        }

        IniciarFase();
    }

    public void PularIntroducao()
    {
        StopAllCoroutines();
        IniciarFase();
    }

    void IniciarFase()
    {
        if (terminou)
            return;

        terminou = true;

        SceneManager.LoadScene("Level01_Main");
    }
}