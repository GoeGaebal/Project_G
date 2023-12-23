using UnityEngine.SceneManagement;
using Google.Protobuf.Protocol;

public class IntroScene : BaseScene
{
    // Start is called before the first frame update
    protected override void Init()
    {
        SceneType = SceneType.Intro;
    }
    private void Start()
    {
        Invoke(nameof(ToLobby), 3.5f);
    }

    public void ToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void Clear()
    {
        
    }
}
