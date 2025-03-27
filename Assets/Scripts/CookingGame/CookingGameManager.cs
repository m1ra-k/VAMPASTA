using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private List<List<ApproachCircleTypeEnum?>> beatmapList = new List<List<ApproachCircleTypeEnum?>>();
    [SerializeField]
    private int roundNumber;
    [SerializeField]
    private int frameCount = 51;
    [SerializeField]
    private int framesToWait = 52;
    [SerializeField]
    private int beatmapListIndex;

    public Queue<GameObject> approachCircleFoodQueue = new Queue<GameObject>();
    public Queue<GameObject> approachCircleGarlicQueue = new Queue<GameObject>();
    public List<GameObject> hearts;

    void Awake()
    {
        string[] lines = beatmapFiles[roundNumber].text.Split('\n');

        ApproachCircleTypeEnum? approachCircleTypeEnum;

        for (int i = 0; i < lines.Length; i++)
        {
            List<ApproachCircleTypeEnum?> beatsForFrameList = new List<ApproachCircleTypeEnum?>();
           
            if (lines[i].Length > 1)
            {
                string[] beatsForFrame = lines[i].Split('|');

                foreach (string beatForFrame in beatsForFrame)
                {
                    approachCircleTypeEnum = beatForFrame.Trim().Equals("Food") ? ApproachCircleTypeEnum.Food : ApproachCircleTypeEnum.Garlic;

                    beatsForFrameList.Add(approachCircleTypeEnum);
                }

                beatmapList.Add(beatsForFrameList);
            }
            else
            {
                beatmapList.Add(new List<ApproachCircleTypeEnum?> { ApproachCircleTypeEnum.Skip });
            }
        }
    }

    void Start()
    {
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();

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
                StartCoroutine(GameProgressionManager.PlayMusic(2, 3.75f, countdown));
                GameProgressionManager.audioSourceBGM.loop = false;
            }
        }
        else if (!finishedCooking && !countdown.activeSelf)
        {
            if (beatmapListIndex < beatmapList.Count)
            {
                frameCount++;

                if (frameCount == framesToWait)
                {
                    foreach (var approachCircle in beatmapList[beatmapListIndex])
                    {
                        GenerateApproachCircle(approachCircle.Value);
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
        yield return new WaitForSeconds(3f);

        done.SetActive(true);

        yield return new WaitForSeconds(3f);

        finishedCooking = true;
    }
}
