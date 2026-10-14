using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Instalacao sem alterar cenas, artes ou animacoes existentes.
// Recursos em Assets/_Projeto/Resources/AudioMenu.
public class AudioMenuFox : MonoBehaviour
{
    private const string Prefix = "FoxAudio_";
    private static AudioMenuFox instancia;
    private AudioSource brisa, folhas, passaros, clique;
    private GameObject passaroMovimento;
    private GameObject folhasClique;
    private bool avesVisiveis;
    private readonly Dictionary<AudioSource, float> volumesOriginais = new Dictionary<AudioSource, float>();
    private float proximaFolha;
    private float fimClique;
    private bool cliqueAtivoAnterior;
    private float musica, efeitos;
    private bool musicaLigada, efeitosLigados;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Criar()
    {
        if (instancia != null) return;
        var go = new GameObject("FoxAudioManager");
        DontDestroyOnLoad(go);
        instancia = go.AddComponent<AudioMenuFox>();
    }

    private void Awake()
    {
        musica = PlayerPrefs.GetFloat(Prefix + "Musica", 1f);
        efeitos = PlayerPrefs.GetFloat(Prefix + "Efeitos", 1f);
        musicaLigada = PlayerPrefs.GetInt(Prefix + "MusicaLigada", 1) == 1;
        efeitosLigados = PlayerPrefs.GetInt(Prefix + "EfeitosLigados", 1) == 1;
        brisa = CriarFonte("BrisaFraca", true, .20f);
        folhas = CriarFonte("FolhasNaturais", false, .085f);
        passaros = CriarFonte("PassarosSuaves", false, .36f);
        clique = CriarFonte("CliqueFolhasPingPong", false, .42f);
        SceneManager.sceneLoaded += CenaCarregada;
    }

    private AudioSource CriarFonte(string nome, bool loop, float volume)
    {
        var fonte = gameObject.AddComponent<AudioSource>();
        fonte.clip = Resources.Load<AudioClip>("AudioMenu/" + nome);
        fonte.playOnAwake = false;
        fonte.loop = loop;
        fonte.spatialBlend = 0;
        fonte.volume = volume;
        return fonte;
    }

    private void CenaCarregada(Scene cena, LoadSceneMode modo)
    {
        brisa.Stop(); folhas.Stop(); passaros.Stop(); clique.Stop();
        passaroMovimento = null;
        folhasClique = null;
        avesVisiveis = false;
        cliqueAtivoAnterior = false;
        volumesOriginais.Clear();
        if (cena.name == "MenuPrincipal")
        {
            StartCoroutine(PrepararMenu());
        }
        else
        {
            StartCoroutine(AplicarVolumesAposCena());
        }
    }

    private IEnumerator PrepararMenu()
    {
        yield return null;
        passaroMovimento = Buscar("PassaroMovimento");
        folhasClique = Buscar("FolhasClique");
        ConectarControles();
        if (brisa.clip != null) brisa.Play();
        proximaFolha = Time.unscaledTime + 0.5f;
    }

    private IEnumerator AplicarVolumesAposCena()
    {
        yield return null;
        AplicarVolumesJogo();
    }

    private GameObject Buscar(string nome)
    {
        foreach (var t in Resources.FindObjectsOfTypeAll<Transform>())
            if (t.gameObject.scene.IsValid() && t.gameObject.scene.isLoaded && t.name == nome)
                return t.gameObject;
        return null;
    }

    private void ConectarControles()
    {
        var sm = Buscar("SliderMusica");
        var se = Buscar("SliderEfeitos");
        var tm = Buscar("ToggleMusica");
        var te = Buscar("ToggleEfeitos");
        if (sm != null)
        {
            var slider = sm.GetComponent<Slider>();
            if (slider != null) { slider.SetValueWithoutNotify(Mathf.Lerp(slider.minValue, slider.maxValue, musica)); slider.onValueChanged.AddListener(v => DefinirMusica(Mathf.InverseLerp(slider.minValue, slider.maxValue, v))); }
        }
        if (se != null)
        {
            var slider = se.GetComponent<Slider>();
            if (slider != null) { slider.SetValueWithoutNotify(Mathf.Lerp(slider.minValue, slider.maxValue, efeitos)); slider.onValueChanged.AddListener(v => DefinirEfeitos(Mathf.InverseLerp(slider.minValue, slider.maxValue, v))); }
        }
        if (tm != null)
        {
            var toggle = tm.GetComponent<Toggle>();
            if (toggle != null) { toggle.SetIsOnWithoutNotify(musicaLigada); toggle.onValueChanged.AddListener(LigarMusica); }
        }
        if (te != null)
        {
            var toggle = te.GetComponent<Toggle>();
            if (toggle != null) { toggle.SetIsOnWithoutNotify(efeitosLigados); toggle.onValueChanged.AddListener(LigarEfeitos); }
        }
    }

    public void DefinirMusica(float v) { musica = Mathf.Clamp01(v); PlayerPrefs.SetFloat(Prefix + "Musica", musica); AtualizarVolumes(); }
    public void DefinirEfeitos(float v) { efeitos = Mathf.Clamp01(v); PlayerPrefs.SetFloat(Prefix + "Efeitos", efeitos); AtualizarVolumes(); }
    public void LigarMusica(bool v) { musicaLigada = v; PlayerPrefs.SetInt(Prefix + "MusicaLigada", v ? 1 : 0); AtualizarVolumes(); }
    public void LigarEfeitos(bool v) { efeitosLigados = v; PlayerPrefs.SetInt(Prefix + "EfeitosLigados", v ? 1 : 0); AtualizarVolumes(); }

    private void AtualizarVolumes()
    {
        float v = efeitosLigados ? efeitos : 0f;
        brisa.volume = .20f * v;
        folhas.volume = .085f * v;
        passaros.volume = .36f * v;
        clique.volume = .42f * v;
        AplicarVolumesJogo();
        PlayerPrefs.Save();
    }

    private void AplicarVolumesJogo()
    {
        foreach (var fonte in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (fonte == brisa || fonte == folhas || fonte == passaros || fonte == clique) continue;
            if (!volumesOriginais.ContainsKey(fonte)) volumesOriginais[fonte] = fonte.volume;
            bool ehMusica = fonte.clip != null && fonte.clip.name.ToLowerInvariant().Contains("fase_01");
            float escala = ehMusica ? (musicaLigada ? musica : 0f) : (efeitosLigados ? efeitos : 0f);
            fonte.volume = volumesOriginais[fonte] * escala;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MenuPrincipal") return;
        AtualizarVolumesMenu();
        // Folhas aparecem com pausas, preservando a brisa continua.
        if (Time.unscaledTime >= proximaFolha && !folhas.isPlaying)
        {
            if (folhas.clip != null) folhas.Play();
            proximaFolha = Time.unscaledTime + 13f;
        }

        bool visivel = false;
        if (passaroMovimento != null && passaroMovimento.activeInHierarchy)
        {
            var rt = passaroMovimento.GetComponent<RectTransform>();
            if (rt != null)
            {
                var cantos = new Vector3[4];
                rt.GetWorldCorners(cantos);
                var canvas = rt.GetComponentInParent<Canvas>();
                Camera cam = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 ponto = RectTransformUtility.WorldToScreenPoint(cam, cantos[i]);
                    if (ponto.x >= 0 && ponto.x <= Screen.width && ponto.y >= 0 && ponto.y <= Screen.height)
                    { visivel = true; break; }
                }
            }
        }
        if (visivel && !avesVisiveis && passaros.clip != null) passaros.Play();
        if (!visivel && avesVisiveis) passaros.Stop();
        avesVisiveis = visivel;

        // Uma unica reproducao por ativacao da animacao, sem reiniciar em loop.
        bool ativo = folhasClique != null && folhasClique.activeInHierarchy;
        if (ativo && !cliqueAtivoAnterior && clique.clip != null) clique.Play();
        if (!ativo && cliqueAtivoAnterior) clique.Stop();
        cliqueAtivoAnterior = ativo;
    }

    private void AtualizarVolumesMenu()
    {
        float v = efeitosLigados ? efeitos : 0f;
        brisa.volume = .20f * v;
        folhas.volume = .085f * v;
        passaros.volume = .36f * v;
        clique.volume = .42f * v;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= CenaCarregada;
        if (instancia == this) instancia = null;
    }
}
