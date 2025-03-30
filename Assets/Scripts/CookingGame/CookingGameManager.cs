using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingGameManager : MonoBehaviour
{    
    private GameProgressionManager GameProgressionManager;

    [Header("UI")]
    [SerializeField]
    private GameObject tutorial;
    [SerializeField]
    private GameObject countdown;
    [SerializeField]
    private GameObject done;
    [SerializeField]
    private Animator raviCookingAnimator;

    [Header("State")]
    [SerializeField]
    private bool playingCookingGame;
    [SerializeField]
    public bool finishedCooking;

    [Header("Beatmap")]
    [SerializeField]
    private GameObject[] approachCirclePrefabs;
    [SerializeField]
    private GameObject approachCircles;
    [SerializeField]
    private GameObject[] hitCircles;
    [SerializeField]
    private TextAsset[] beatmapFiles;
    [SerializeField]
    private List<List<(ApproachCircleTypeEnum?, int)>> beatmapList = new List<List<(ApproachCircleTypeEnum?, int)>>();
    [SerializeField]
    private int frameCount = 0;
    [SerializeField]
    private int beatmapListIndex;

    public Queue<GameObject> approachCircleFoodQueue = new Queue<GameObject>();
    public Queue<GameObject> approachCircleGarlicQueue = new Queue<GameObject>();
    public List<GameObject> hearts;

    public Image cookingRavi;
    public List<Sprite> cookingRaviReactions;

    void Awake()
    {
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();

        string[] lines = beatmapFiles[(GameProgressionManager.sceneNumber + 1 )/ 2 - 1].text.Split('\n');

        ApproachCircleTypeEnum? approachCircleTypeEnum;

        for (int i = 0; i < lines.Length; i++)
        {
            List<(ApproachCircleTypeEnum?, int)> beatsForFrameList = new List<(ApproachCircleTypeEnum?, int)>();
           
            if (lines[i].Length > 1)
            {
                string[] beatsForFrame = lines[i].Split('|');

                foreach (string beatForFrame in beatsForFrame)
                {
                    string[] eachBeat = beatForFrame.Split(",");

                    approachCircleTypeEnum = eachBeat[0].Trim().Equals("Food") ? ApproachCircleTypeEnum.Food : ApproachCircleTypeEnum.Garlic;

                    beatsForFrameList.Add((approachCircleTypeEnum, int.Parse(eachBeat[1])));
                }

                beatmapList.Add(beatsForFrameList);
            }
            // else
            // {
            //     beatmapList.Add(new List<ApproachCircleTypeEnum?> { ApproachCircleTypeEnum.Skip });
            // }
        }
    }

    void Start()
    {
        // scene number is 1 , 3 , 5
        // if scene number is 1
        if (GameProgressionManager.sceneNumber == 1)
        {
            tutorial.SetActive(true);
        }
    }

    void Update()
    {
        if (!playingCookingGame)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                tutorial.SetActive(!tutorial.activeSelf);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !tutorial.activeSelf)
            {
                playingCookingGame = true;
                countdown.SetActive(true);
                // allow for a 4 second countdown
                GameProgressionManager.StopMusic();
                StartCoroutine(GameProgressionManager.PlayMusic(2, 3.75f, countdown, 2f));
                GameProgressionManager.audioSourceBGM.loop = false;
            }
        }
        else if (!finishedCooking && !countdown.activeSelf)
        {
            if (beatmapListIndex < beatmapList.Count)
            {
                frameCount++;

                int waitFrames = beatmapList[beatmapListIndex][0].Item2;
                // print($"frame count current: {frameCount}; waiting for this many frames: {waitFrames}");

                if (frameCount == waitFrames)
                {
                    foreach (var approachCircle in beatmapList[beatmapListIndex])
                    {
                        GenerateApproachCircle(approachCircle.Item1.Value);
                    }
                    
                    frameCount = 0;
                    beatmapListIndex++;
                }
            }
            else
            {
                StartCoroutine(DisplayDone());
            }
        }
    }

    public void GenerateApproachCircle(ApproachCircleTypeEnum approachCircleTypeEnum)
    {
        if (approachCircleTypeEnum != ApproachCircleTypeEnum.Skip)
        {
            GameObject prefab = approachCirclePrefabs[approachCircleTypeEnum == ApproachCircleTypeEnum.Food ? 0 : 1];
            Transform parentTransform = approachCircleTypeEnum == ApproachCircleTypeEnum.Food ? hitCircles[0].transform : hitCircles[1].transform;
            GameObject approachCircle = Instantiate(prefab, parentTransform);
            approachCircle.transform.localPosition = new Vector2(0f, 700f);
            approachCircle.name += Random.Range(0, 1000).ToString("D3");
            // need to add it to the corresponding q
            switch (approachCircleTypeEnum)
            {
                case ApproachCircleTypeEnum.Food:
                    approachCircleFoodQueue.Enqueue(approachCircle);
                    break;

                case ApproachCircleTypeEnum.Garlic:
                    approachCircleGarlicQueue.Enqueue(approachCircle);
                    break;
            }
            // print($"added {approachCircle.name}");
        }
    }
    
    private IEnumerator DisplayDone()
    {
        yield return new WaitForSeconds(6f);

        done.SetActive(true);

        yield return new WaitForSeconds(3f);

        finishedCooking = true;
    }

    // todo: animation eventually?
    public IEnumerator RaviHurt()
    {
        cookingRavi.sprite = cookingRaviReactions[1];

        yield return new WaitForSeconds(0.25f);

        if (hearts.Count > 0)
        {
            cookingRavi.sprite = cookingRaviReactions[0];
        }
    }
}
