using Godot;

namespace kemocard.Components.Card;

public partial class BattleCardItem : CardBigItem
{
    [Export] private Label _statusLabel;
    public BattleCardStatus Status { get; private set; } = BattleCardStatus.Normal;

    public void SetStatus(BattleCardStatus status)
    {
        Status = status;
        _statusLabel.Text = status switch
        {
            BattleCardStatus.Normal => "",
            BattleCardStatus.Targeting => "Targeting",
            BattleCardStatus.Confirmed => "Confirmed",
            BattleCardStatus.Used => "Used",
            _ => ""
        };
    }

    public void RefreshCard()
    {
    }

    public override void Clear()
    {
        base.Clear();
        _statusLabel.Text = "";
    }
}

public enum BattleCardStatus
{
    Normal,
    Targeting,
    Confirmed,
    Used,
}