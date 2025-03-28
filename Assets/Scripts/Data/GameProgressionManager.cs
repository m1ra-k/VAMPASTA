using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProgressionManager : MonoBehaviour
{
    public static GameProgressionManager GameProgressionManagerInstance;

    public string currentScene;

    // Transition    
    private FadeEffect fadeEffect;
    private GameObject blackTransition;

    [Header("[State]")]
    public int sceneNumber;
    public string previousScene;
    public bool finishedCurrentRound;

    [Header("[Player]")]
    public GameObject ravi;
    public bool facingUp;
    public bool transitioning;

    [Header("[Cooking Game]")]
    private CookingGameManager cookingGameManager;

    [Header("[End Screen]")]
    public bool fadedInTheEnd;

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
        { 2, new List<string> { "VisualNovel", "vampasta_1_post_first_round" } },
        { 3, new List<string> { "RestaurantOverworld" } },
        { 4, new List<string> { "VisualNovel", "vampasta_2_post_second_round" } },
        { 5, new List<string> { "RestaurantOverworld" } },
        { 6, new List<string> { "VisualNovel", "vampasta_3_post_third_round" } },
        { 7, new List<string> { "EndScreen" } },
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
                transitioning = false;
                ravi = GameObject.FindWithTag("Player");
                if (previousScene.Equals("CookingGame"))
                {
                    ravi.transform.localPosition = new Vector2(-335, -65);
                }
                break;

            case "CookingGame":
                transitioning = false;
                cookingGameManager = FindObjectOfType<CookingGameManager>();
                break;

            default:
                transitioning = false;
                break;
        }     
    }

    void Update()
    {
        if (!transitioning)
        {
            if (currentScene.Equals("RestaurantOverworld") )
            {
                // go to cooking game if ravi goes through door
                if (ravi.transform.position.x < 15)
                {
                    TransitionScene();
                    previousScene = "RestaurantOverworld";
                }
                // facing correct direction (up)
                else if (facingUp && Input.GetKeyDown(KeyCode.Space))
                {
                    // TODO ... MAKE PROMPTS FOR THESE PARTS NEXT !!!
                    // mateo is talkable
                    if (ravi.transform.position == new Vector3(365, 395, 0))
                    {
                        Debug.Log("talking to mateo!");
                    }
                    // plate is settable
                    else if (ravi.transform.position == new Vector3(465, 395, 0) && finishedCurrentRound)
                    {
                        finishedCurrentRound = false;
                        TransitionScene("play");
                        // Debug.Log("set down plate!");
                    }
                }
                
            }
            else if (currentScene.Equals("CookingGame"))
            {
                if (cookingGameManager.hearts.Count == 0)
                {
                    TransitionScene("lost");
                    previousScene = "CookingGame";
                }
                else if (cookingGameManager.finishedCooking)
                {
                    finishedCurrentRound = true;
                    TransitionScene();
                    previousScene = "CookingGame";
                }
            }
            else if (currentScene.Equals("EndScreen"))
            {
                if (!fadedInTheEnd)
                {
                    fadedInTheEnd = true;
                    fadeEffect.FadeIn(GameObject.FindWithTag("Fade"), fadeTime: 2f, fadeDelay: 2f);
                }
            }
        }
    }

    // TODO get rid of flag soon
    public void TransitionScene(string possibleFlag = "")
    {
        string sceneType = "";

        // not true always tho hmm
        if (possibleFlag.Equals("play"))
        {
            sceneNumber += 1;
            sceneType = sceneProgressionLookup[sceneNumber][0];
        }
        else if (possibleFlag.Equals("lost"))
        {
            sceneType = "GameOver";
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
                if (sceneNumber == 0)
                {
                    StartCoroutine(PlayMusic(1)); 
                }
                break;
                
            // TODO? focus on VisualNovel for now
            case "RestaurantOverworld":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "RestaurantOverworld");
                transitioning = true;
                audioSourceBGM.loop = true;
                // TODO if music 0 isnt already play it, then play it (vn to ro)
                StartCoroutine(PlayMusic(0));
                break;

            case "CookingGame":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "CookingGame");
                transitioning = true;
                StopMusic();
                break;

            case "GameOver":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "GameOver");
                transitioning = true;
                StopMusic();
                break;

            case "EndScreen":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "EndScreen");
                transitioning = true;
                break;
        }
    }

    public void StopMusic()
    {
        audioSourceBGM.Pause();
    }

    public IEnumerator PlayMusic(int index, float waitTime = 0.5f, GameObject gameObjectToDeactivate = null)
    {
        float startVolume = audioSourceBGM.volume;

        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            audioSourceBGM.volume = Mathf.Lerp(startVolume, 0, t / 0.25f);
            yield return null;
        }

        audioSourceBGM.volume = 0;
        audioSourceBGM.Pause();

        yield return new WaitForSeconds(waitTime);

        if (gameObjectToDeactivate)
        {
            gameObjectToDeactivate.SetActive(false);
        }

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
        TransitionScene("play");
    }
}
