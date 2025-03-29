using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameProgressionManager : MonoBehaviour
{
    public static GameProgressionManager GameProgressionManagerInstance;

    public string currentScene;

    // Transition    
    public FadeEffect fadeEffect;
    public GameObject blackTransition;

    [Header("[State]")]
    public int sceneNumber;
    public bool transitioning;
    public string previousScene;

    [Header("[Start Screen]")]
    private Button playButton;
    
    [Header("[Restaurant Overworld]")]
    public GameObject ravi;
    public GameObject dialogueCanvas;
    private DialogueSystemManager dialogueCanvasDialogueSystemManager;
    public bool currentlyTalking;
    public bool facingUp;
    public bool finishedCurrentRound;

    [Header("[Cooking Game]")]
    private CookingGameManager cookingGameManager;

    [Header("[Game Over]")]
    public Button retryButton;

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
        { 7, new List<string> { "EndScreen" } }
    };

    public Dictionary <int, List<string>> overworldDialogueLookup = new Dictionary<int, List<string>> 
    {
        { 1, new List<string> { "VisualNovel", "vampasta_1_before_first_round" } },
        { 3, new List<string> { "VisualNovel", "vampasta_2_before_second_round" } },
        { 5, new List<string> { "VisualNovel", "vampasta_3_before_third_round" } },
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

        transitioning = false;

        switch (currentScene)
        {
            case "StartScreen":
                playButton = GameObject.FindWithTag("Button").GetComponent<Button>();
                break;

            case "RestaurantOverworld":
                currentlyTalking = false;
                ravi = GameObject.FindWithTag("Player");
                dialogueCanvas = GameObject.FindWithTag("Dialogue");
                dialogueCanvasDialogueSystemManager = dialogueCanvas.GetComponentInChildren<DialogueSystemManager>();
                dialogueCanvas.SetActive(false);
                if (previousScene.Equals("CookingGame")) 
                {
                    ravi.transform.localPosition = new Vector2(-335, -65);
                }
                break;

            case "CookingGame":
                cookingGameManager = FindObjectOfType<CookingGameManager>();
                break;

            case "GameOver":
                StopMusic();
                retryButton = GameObject.FindWithTag("Button").GetComponent<Button>();
                break;
        }     
    }

    void Update()
    {
        if (!transitioning)
        {   
            if (currentScene.Equals("StartScreen"))
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playButton.onClick.Invoke();
                }
            }
            if (currentScene.Equals("RestaurantOverworld"))
            {
                // go to cooking game if ravi goes through door
                if (ravi.transform.position.x < 15)
                {
                    TransitionScene();
                    previousScene = "RestaurantOverworld";
                }
                // facing correct direction (up)
                else if (facingUp && Input.GetKeyDown(KeyCode.Space) && !currentlyTalking)
                {
                    // mateo is talkable
                    // TODO ... PREVENTING MOVEMENT
                    if (ravi.transform.position == new Vector3(365, 395, 0) && !currentlyTalking)
                    {
                        currentlyTalking = true;
                        dialogueCanvasDialogueSystemManager.enabled = true;
                        dialogueCanvas.SetActive(true);
                    }
                    // plate is settable
                    else if (ravi.transform.position == new Vector3(465, 395, 0) && finishedCurrentRound)
                    {
                        currentlyTalking = true;
                        finishedCurrentRound = false;
                        TransitionScene("play");
                        previousScene = "VisualNovel";
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
            else if (currentScene.Equals("GameOver"))
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    retryButton.onClick.Invoke();
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
        else if (possibleFlag.Equals("retry"))
        {
            fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "CookingGame");
            transitioning = true;
            return;
        }
        else
        {
            sceneType = currentScene.Equals("RestaurantOverworld") ? "CookingGame" : "RestaurantOverworld";
        }
        
        string nextSceneVisualNovelJSONFileName = "";

        switch (sceneType)
        {
            case "VisualNovel":
                nextSceneVisualNovelJSONFileName = sceneProgressionLookup[sceneNumber][1];
                nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Dialogue/{nextSceneVisualNovelJSONFileName}");

                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "VisualNovel");
                transitioning = true;
                if (sceneNumber == 0)
                {
                    StartCoroutine(PlayMusic(1)); 
                }
                break;
                
            case "RestaurantOverworld":
                nextSceneVisualNovelJSONFileName = overworldDialogueLookup[sceneNumber][1];
                nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Dialogue/{nextSceneVisualNovelJSONFileName}");

                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "RestaurantOverworld");
                transitioning = true;
                audioSourceBGM.loop = true;
                if (currentTrack != 0)
                {
                    StartCoroutine(PlayMusic(0));
                }
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
        audioSourceBGM.Stop();
        currentTrack = -1;
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
