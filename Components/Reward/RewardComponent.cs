using System.Linq;
using cfg.reward;
using Godot;
using kemocard.Components.List;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;

namespace kemocard.Components.Reward;

public partial class RewardComponent : Control, ISelectableItem
{
    public int Index { get; set; }
    public VirtualList List { get; set; }
    [Export] private Label _descLab;
    [Export] private Button _skipBtn;
    [Export] private Button _getBtn;
    private string _rewardId;
    private cfg.config.Reward _conf;

    public override void _Ready()
    {
        base._Ready();
        _skipBtn.Pressed += SkipBtnOnPressed;
        _getBtn.Pressed += GetBtnOnPressed;
        MouseEntered += OnMouseEntered;
        MouseExited += StaticUtil.HideHint;
    }

    public override void _ExitTree()
    {
        _skipBtn.Pressed -= SkipBtnOnPressed;
        _getBtn.Pressed -= GetBtnOnPressed;
        MouseEntered -= OnMouseEntered;
        MouseExited -= StaticUtil.HideHint;
        base._ExitTree();
    }

    public void Init(string rewardId)
    {
        _rewardId = rewardId;
        _conf = GameCore.Tables.TbReward.GetOrDefault(rewardId);
        if (_conf != null)
        {
            switch (_conf.Type)
            {
                case RewardType.MONEY:
                    _descLab.Text = "金钱";
                    break;
                case RewardType.CARD:
                    _descLab.Text = "卡牌";
                    break;
            }
        }
    }

    private void GetBtnOnPressed()
    {
        GameCore.ControllerMgr.SendUpdate(ControllerType.Run, CommonEvent.GetReward, _rewardId);
    }

    private void SkipBtnOnPressed()
    {
        GameCore.ControllerMgr.SendUpdate(ControllerType.Run, CommonEvent.GetReward, _rewardId, true);
    }

    private void OnMouseEntered()
    {
        if (_conf == null) return;
        switch (_conf.Type)
        {
            case RewardType.CARD:
                var str = "获得卡牌：\n";
                _conf.Cards.Select(StaticUtil.GetCardDesc).Where(t => !string.IsNullOrWhiteSpace(t))
                    .Aggregate("", (current, t) => current + (t + "\n"));

                StaticUtil.ShowHint(str);
                break;
        }
    }
}