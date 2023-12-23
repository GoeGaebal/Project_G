using TMPro;
using UnityEngine.UI;

public class UI_RoomItem : UI_Base
{
    enum Texts
    {
        Name,
        NoP,
    }

    enum Buttons
    {
        UI_RoomItem,
    }

    public TextMeshProUGUI Name, NoP;
    public Button RoomBtn;

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        
        Name = GetTextMeshPro((int)Texts.Name);
        NoP = GetTextMeshPro((int)Texts.NoP);
        RoomBtn = GetButton((int)Buttons.UI_RoomItem);
    }
}
