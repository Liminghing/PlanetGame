using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    //[SerializeField] Button _showAdButton; // 광고를 보여줄 버튼
    public GameManager gm;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android"; // 안드로이드 플랫폼용 광고 단위 ID (테스트용)

    string _adUnitId = null; // 지원하지 않는 플랫폼에 대한 광고 단위 ID는 null로 남겨둡니다.

    void Awake()
    {
        // 현재 플랫폼의 광고 단위 ID를 가져옵니다.
        _adUnitId = _androidAdUnitId;
        
        // 광고가 준비될 때까지 버튼을 비활성화합니다.
        //_showAdButton.interactable = false;
    }

    // 광고를 준비하기 원할 때 이 공개 메서드를 호출합니다.
    public void LoadAd()
    {
        _adUnitId = "Rewarded_Android"; 
        // 광고 초기화 후에만 콘텐츠를 로드해야 합니다 (이 예시에서는 초기화는 다른 스크립트에서 처리됩니다).
        Debug.Log("광고 로딩 중: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // 광고가 성공적으로 로드되면 버튼에 리스너를 추가하고 활성화합니다.
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("광고 로드됨: " + adUnitId);

        //if (adUnitId.Equals(_adUnitId))
        //{
        //    // 버튼이 클릭되었을 때 ShowAd() 메서드를 호출하도록 버튼을 설정합니다.
        //    _showAdButton.onClick.AddListener(ShowAd);
        //    // 사용자가 클릭할 수 있도록 버튼을 활성화합니다.
        //    _showAdButton.interactable = true;
        //}
    }

    // 사용자가 버튼을 클릭했을 때 실행할 메서드를 구현합니다.
    public void ShowAd()
    {
        Debug.Log("광고 표시: ");
        // 버튼을 비활성화합니다.
        //_showAdButton.interactable = false;
        // 광고를 표시합니다.
        Advertisement.Show(_adUnitId, this);
    }

    // 광고 보기 완료 콜백 메서드를 구현하여 사용자가 보상을 받았는지 확인합니다.
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {

            // 여기에 광고 시청에 대한 보상을 지급하는 코드를 추가하세요.
            gm = FindObjectOfType<GameManager>();
            
            if (gm == null)
            {
                Debug.LogError("GameManager를 찾을 수 없습니다.");
            }
            Debug.Log("유니티 광고 리워드 광고 완료");
            // 보상 지급.
            
            gm.ExecuteNextPlanetAndDeactivateEndGroup();
            //gm.watchedAd = true;

        }
    }

    // 로드 및 표시 오류 콜백을 구현합니다.
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"광고 단위 {adUnitId} 로드 오류: {error.ToString()} - {message}");
        // 오류 세부 정보를 사용하여 다른 광고를 로드할지 여부를 결정합니다.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"광고 단위 {adUnitId} 표시 오류: {error.ToString()} - {message}");
        // 오류 세부 정보를 사용하여 다른 광고를 로드할지 여부를 결정합니다.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // 버튼 리스너를 정리합니다.
        //_showAdButton.onClick.RemoveAllListeners();
    }

}