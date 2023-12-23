using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene 관련 매니저
/// </summary>
public class SceneManagerEx
{
    public BaseScene CurrentScene => Object.FindObjectOfType<BaseScene>();
    private AsyncOperation _asyncOp = null;
    float minTime = 2.0f;
    float loadedTime = 0;
    private bool _isLoading = false;
    public bool IsLoading{
        get { return _isLoading; }
    }
    private string nextSceneName;
    /// <summary>
    /// Scene을 불러오는 함수, 기존 Unity 방식으로는 type에 따라서 불러오지 못하므로 이를 반영하였다.
    /// </summary>
    /// <remark>
    /// PUN2의 모든 클라이언트들이 같은 씬을 로드하도록 하려면 PhotonNetwork.LoadLevel을 사용하여야 한다.
    /// </remark>
    /// <param name="type">
    /// Define에 존재하는 Scene enum class에 존재하는 상수
    /// /param>
	public void LoadScene(SceneType type)
    {
        if (nextSceneName == nameof(type)) return;
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(SceneType.Loading));
        _isLoading = true;
        loadedTime = 0;
        nextSceneName = GetSceneName(type);
        // PhotonNetwork.LoadLevel(GetSceneName(type));
    }

    public void WaitLoading(float time)
    {
        if(_isLoading == false) return;
        
        else if(_isLoading == true && loadedTime < minTime)
        {
            Managers.Input.Asset.Disable();
            loadedTime += time;
        }
        else
        {
            _asyncOp = SceneManager.LoadSceneAsync(nextSceneName);
            _isLoading = false;
            Managers.Input.Asset.Enable();
        }
    }

    private string GetSceneName(SceneType type)
    {
        string name = System.Enum.GetName(typeof(SceneType), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
