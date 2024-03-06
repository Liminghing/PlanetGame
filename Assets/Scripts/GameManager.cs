using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;


//니저 스크립트에 Application.targetFrameRate = 60; 해주시고 동글의 리지드바디에는 Collision Type을 Continuse로 바꾸어주세요. 이래도 안된다면 Edit > Project Setting > Time항목에서 Fixed Time Interval을 조금만 더 낮추어서 물리 연산속도를 높이는 것도 방법이 있습니다.
public class GameManager : MonoBehaviour, IUnityAdsInitializationListener
{
    Coroutine gameRoutine;
    [Header("-----------[ Core ]-----------")]
    public bool isOver;
    public int maxLevel;
    public int score;
    
    [Header("-----------[ Object Pooling ]-----------")]
    public GameObject planetPrefebs;
    public Transform planetGroup;
    public List<Planet> planetPool;
    public GameObject effectPrefebs;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;
    [Range(1, 30)]
    public int poolSize;
    public int poolCursor;
    public Planet lastPlanet;
    [Header("-----------[ Audio ]-----------")]
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx {LevelUp,Next,Attach,Button,Over};
    int sfxCursor;
    [Header("-----------[ UI ]-----------")]
    public Text scoretext;
    public Text maxScoreText;
    public GameObject endGrop;
    public GameObject startGrop;
    public Text subScoreText;


    [Header("-----------[ ETC ]-----------")]
    public GameObject line;
    public GameObject finishLine;
    

    [SerializeField] string _androidGameId = "5568797";//유니티 게임id
    [SerializeField] bool _testMode = false;
    private string _gameId;
    public bool watchedAd;
    public Button watchedAdButton;
    public RewardedAdsButton rewardedAdsButton;

    void Awake()
    {
        Debug.Log("AwakeAwakeAwake");
        //광고초기화 
        InitializeAds();
        Application.targetFrameRate = 60;
        
        
        planetPool = new List<Planet>();
        effectPool = new List<ParticleSystem>();
        for(int index=0; index < poolSize; index++)
        {
            MakePlanet();
        }

        if (!PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
        //DontDestroyOnLoad(maxScoreText.gameObject);
        maxScoreText.text = "TR : " + PlayerPrefs.GetInt("MaxScore").ToString();


    }

    Planet MakePlanet()
    {
        
        GameObject instantEffectObject = Instantiate(effectPrefebs, effectGroup);
        instantEffectObject.name = "Effect " + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObject.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        GameObject instantPlanetObject = Instantiate(planetPrefebs, planetGroup);
        instantPlanetObject.name = "Planet " + planetPool.Count;
        Planet instantPlanet = instantPlanetObject.GetComponent<Planet>();
        instantPlanet.gm = this;
        instantPlanet.effect = instantEffect;
        planetPool.Add(instantPlanet);

        return instantPlanet;
    }

    Planet GetPlanet()
    {
        
        for (int index=0; index < planetPool.Count;index++)
        {
            poolCursor = (poolCursor + 1) % planetPool.Count;

            if (!planetPool[poolCursor].gameObject.activeSelf)
            {
                return planetPool[poolCursor];
            }
        }
        return MakePlanet();

    }
    public void NextPlanet()
    {
     

        if (isOver)
        {
            return;
        }
        lastPlanet = GetPlanet();

        if (maxLevel < 3) {
            lastPlanet.level = Random.Range(0, maxLevel);
        } else if (maxLevel < 6) {
            lastPlanet.level = Random.Range(0, 3);
        }
        else
        {
            lastPlanet.level = Random.Range(0, 4);
        }
        lastPlanet.gameObject.SetActive(true);

        SfxPlay(Sfx.Next);
        StartCoroutine("WaitNext");
    }

    IEnumerator WaitNext()
    {
        while (lastPlanet != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.5f);

        NextPlanet();
    }



    public void GameStart()
    {
        watchedAd = false;
        //광고시청버튼 활성화
        watchedAdButton.interactable = true;
        Debug.Log("GameStartGameStart");
        line.SetActive(true);
        finishLine.SetActive(true);
        scoretext.gameObject.SetActive(true);
        maxScoreText.gameObject.SetActive(true);
        startGrop.SetActive(false);
        bgmPlayer.Play();
        SfxPlay(Sfx.Button);
        Invoke("NextPlanet", 1.5f);///////
        
    }

    public void touchDown()
    {
        if(lastPlanet == null)
        {
            return;
        }
        lastPlanet.Drag();
    }
    public void touchUp()
    {
        if (lastPlanet == null)
        {
            return;
        }
        lastPlanet.Drop();
        lastPlanet = null;
    }
    public void GameOver()
    { 
        //광고시청버튼 비활성화
        if (watchedAd)
        {
            watchedAdButton.interactable = false;
        }
        //광고 로드하기
        if (isOver)
        {
            return;
        }
        isOver = true;
        rewardedAdsButton.LoadAd();

        StartCoroutine("GameOverRoutine");
        
    }
    IEnumerator GameOverRoutine()
    {
        Planet[] planets = GameObject.FindObjectsOfType<Planet>();

        for (int index = 0; index < planets.Length; index++)
        {
            planets[index].rigid.simulated = false;
        }

        for (int index = 0; index < planets.Length; index++)
        {
            planets[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);

        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);
        if (subScoreText != null)
        {
            subScoreText.text = "Record : " + score.ToString();
        }
        if (endGrop != null)
        {
            endGrop.SetActive(true);
        }
        
        SfxPlay(Sfx.Over);
    }


    public void Home()
    {
        SfxPlay(Sfx.Button);
        StartCoroutine("HomeCoroutine");
    }
    IEnumerator HomeCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("LoadScene Home");
    }

    public void Reset()
    {
        SfxPlay(Sfx.Button);
        StartCoroutine("ResetCoroutine");
    }
    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(1f);
    }

    void Start()
    {
        Debug.Log("StartStartStartStart");
        startGrop.SetActive(true);
    }

    public void SfxPlay(Sfx type)
    {
        switch (type)
        {
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(0, 3)];
                break;
            case Sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case Sfx.Attach:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case Sfx.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
        }
        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;

    }
    void LateUpdate()
    {
        if (scoretext!=null)
        {
            //@@
            scoretext.text = score.ToString();
        }
       
    }
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }




    public void InitializeAds()
    {
        _gameId = _androidGameId;

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    public void ExecuteNextPlanetAndDeactivateEndGroup()
    {
        isOver = false;
        watchedAd = true;
        ResetPlanetPool();
        NextPlanet();
         
        endGrop.SetActive(false);
    }
    void ResetPlanetPool()
    {
        // 모든 플래닛 오브젝트를 비활성화하고 풀에 반환합니다.
        for (int i = 0; i < planetPool.Count; i++)
        {
            if (planetPool[i].gameObject.activeSelf)
            {
                planetPool[i].gameObject.SetActive(false);
            }
        }
    }

}
