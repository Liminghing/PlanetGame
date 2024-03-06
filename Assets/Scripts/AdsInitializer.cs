//using UnityEngine;
//using UnityEngine.Advertisements;

//public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
//{
//    [SerializeField] string _androidGameId= "5568797";//앱 id
//    [SerializeField] bool _testMode = true;
//    private string _gameId;

//    void Awake()
//    {
//        InitializeAds();
//    }

//    public void InitializeAds()
//    {
//        _gameId = _androidGameId;

//        if (!Advertisement.isInitialized && Advertisement.isSupported)
//        {
//            Advertisement.Initialize(_gameId, _testMode, this);
//        }
//    }


//    public void OnInitializationComplete()
//    {
//        Debug.Log("Unity Ads initialization complete.");
//    }

//    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
//    {
//        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
//    }
//}