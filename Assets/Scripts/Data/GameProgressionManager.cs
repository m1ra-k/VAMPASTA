using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameProgressionManager : MonoBehaviour
{
    public static GameProgressionManager GameProgressionManagerInstance;

    public string currentScene;

    // Transition    
    private FadeEffect fadeEffect;
    private GameObject blackTransition;

    [Header("[Moment]")]
    public int sceneNumber;

    [Header("[Player]")]
    public GameObject ravi;
    public bool transitioning;

    [Header("[Music]")]
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSourceBGM;
    private int currentTrack;

    // Data
    public TextAsset nextSceneVisualNovelJSONFile;
    public Dictionary <int, List<string>> sceneProgressionLookup = new Dictionary<int, List<string>> 
    {    
        { 0, new List<string> { "VisualNovel", "vampasta_0_prologue" } },
        { 1, new List<string> { "RestaurantOverworld" } },
        { 2, new List<string> { "VisualNovel", "vampasta_1_post_round" } },
        { 3, new List<string> { "RestaurantOverworld" } },
        { 4, new List<string> { "VisualNovel", "vampasta_2_post_round" } },
        { 5, new List<string> { "RestaurantOverworld" } },
        { 6, new List<string> { "VisualNovel", "vampasta_3_post_round" } }
    };

    void Awake()
    {        
        if (GameProgressionManagerInstance == null)
        {
            Application.targetFrameRate = 60;
            GameProgressionManagerInstance = this;
            DontDestroyOnLoad(gameObject);

            fadeEffect = GetComponent<FadeEffect>();
        }
        else
        {            
            Destroy(gameObject);
            return;
        }

        audioSourceBGM = GetComponent<AudioSource>();
        currentTrack = -1;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;

        blackTransition = GameObject.Find("Canvas").transform.Find("BlackTransition").gameObject;

        fadeEffect.FadeOut(blackTransition, 1f);

        switch (currentScene)
        {
            case "RestaurantOverworld":
                ravi = GameObject.FindWithTag("Player");
                break;

            case "CookingGame":
                transitioning = false;
                break;
        }
        
    }

    void Update()
    {
        if (currentScene.Equals("RestaurantOverworld") && ravi.transform.position.x < 15)
        {
            TransitionScene();
        }

    }

    // TODO get rid of flag soon
    public void TransitionScene(bool possibleFlag = false)
    {
        string sceneType = "";

        // not true always tho hmm
        if (possibleFlag)
        {
            sceneNumber += 1;
            sceneType = sceneProgressionLookup[sceneNumber][0];
        }
        else
        {
            sceneType = currentScene.Equals("RestaurantOverworld") ? "CookingGame" : "RestaurantOverworld";
        }
        
        switch (sceneType)
        {
            case "VisualNovel":
                string nextSceneVisualNovelJSONFileName = sceneProgressionLookup[sceneNumber][1];
                nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Dialogue/{nextSceneVisualNovelJSONFileName}");

                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "VisualNovel");
                StartCoroutine(PlayMusic(1));
                break;
                
            // TODO? focus on VisualNovel for now
            case "RestaurantOverworld":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "RestaurantOverworld");
                StartCoroutine(PlayMusic(0));
                break;

            case "CookingGame":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "CookingGame");
                transitioning = true;
                StartCoroutine(PlayMusic(2));
                break;
        }
    }

    public void StopMusic()
    {
        audioSourceBGM.Pause();
    }

    IEnumerator PlayMusic(int index)
    {
        float startVolume = audioSourceBGM.volume;

        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            audioSourceBGM.volume = Mathf.Lerp(startVolume, 0, t / 0.25f);
            yield return null;
        }

        audioSourceBGM.volume = 0;
        audioSourceBGM.Pause();

        yield return new WaitForSeconds(0.5f);

        audioSourceBGM.volume = 1;
        audioSourceBGM.UnPause();

        if (index != currentTrack)
        {
            audioSourceBGM.clip = audioClips[index];
            audioSourceBGM.Play();

            currentTrack = index;
        }
    }

    // BUTTONS - TODO MOVE
    public void PlayGame()
    {
        TransitionScene(true);
    }
}
