using System.Linq;
using cfg.reward;
using Godot;
using kemocard.Components.List;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;

namespace kemocard.Components.Reward;

public partial class RewardComponent : Control, ISelectableItem
{
    public int Index { get; set; }
    public VirtualList List { get; set; }
    [Export] private Label _descLab;
    [Export] private Button _skipBtn;
    [Export] private Button _getBtn;
    private UnhandledRewardStruct _rewardStruct;
    private cfg.config.Reward _conf;

    public override void _Ready()
    {
        base._Ready();
        _skipBtn.Visible = false;
        _skipBtn.Pressed += SkipBtnOnPressed;
        _skipBtn.MouseEntered += SkipBtnOnMouseEntered;
        _skipBtn.MouseExited += StaticUtil.HideHint;
        _getBtn.Pressed += GetBtnOnPressed;
        MouseEntered += OnMouseEntered;
        MouseExited += StaticUtil.HideHint;
    }

    public override void _ExitTree()
    {
        _skipBtn.Pressed -= SkipBtnOnPressed;
        _getBtn.Pressed -= GetBtnOnPressed;
        _skipBtn.MouseEntered -= SkipBtnOnMouseEntered;
        _skipBtn.MouseExited -= StaticUtil.HideHint;
        MouseEntered -= OnMouseEntered;
        MouseExited -= StaticUtil.HideHint;
        base._ExitTree();
    }

    public void Init(UnhandledRewardStruct rewardStruct)
    {
        _rewardStruct = rewardStruct;
        _conf = GameCore.Tables.TbReward.GetOrDefault(rewardStruct.Id);
        if (_conf == null) return;
        switch (_conf.Type)
        {
            case RewardType.MONEY:
                _descLab.Text = "金钱";
                break;
            case RewardType.CARD:
                _descLab.Text = "卡牌";
                _skipBtn.Visible = true;
                break;
            case RewardType.CHARACTER:
                _descLab.Text = _conf.Desc;
                break;
            case RewardType.NONE:
            default:
                break;
        }
    }

    private void GetBtnOnPressed()
    {
        switch (_conf.Type)
        {
            case RewardType.CHARACTER:
                GameCore.ViewMgr.OpenView(ViewType.GetRewardView, _rewardStruct);
                break;
            case RewardType.NONE:
            case RewardType.MONEY:
            case RewardType.CARD:
            default:
                GameCore.ControllerMgr.SendUpdate(ControllerType.Run, CommonEvent.GetReward, _rewardStruct.Id, null);
                break;
        }
    }

    private void SkipBtnOnPressed()
    {
        GameCore.ControllerMgr.SendUpdate(ControllerType.Run, CommonEvent.GetReward, _rewardStruct.Id, null, true);
    }

    private void OnMouseEntered()
    {
        if (_conf == null) return;
        switch (_conf.Type)
        {
            case RewardType.CARD:
                var str = "获得卡牌：\n";
                str += _conf.Cards.Select(StaticUtil.GetCardDesc).Where(t => !string.IsNullOrWhiteSpace(t))
                    .Aggregate("", (current, t) => current + (t + "\n"));

                StaticUtil.ShowHint(str);
                break;
        }
    }

    private void SkipBtnOnMouseEntered()
    {
        if (_conf == null) return;
        switch (_conf.Type)
        {
            case RewardType.CARD:
                StaticUtil.ShowHint($"跳过卡牌奖励以获取{_conf.Cards.Count}张随机未获得的卡牌");
                break;
        }
    }
}