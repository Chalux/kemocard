using System.Collections.Generic;
using System.Linq;
using cfg.config;
using cfg.reward;
using Godot;
using kemocard.Components.List;
using kemocard.Components.Reward;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.GameUI;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;
using kemocard.Scripts.Pawn;

namespace kemocard.Scenes;

public partial class GetRewardView : BaseView
{
    [Export] private VirtualList _list;
    private UnhandledRewardStruct _struct;
    private readonly List<string> _ids = [];
    private Reward _conf;
    [Export] private Button _btnGet;
    [Export] private Button _btnCancel;

    public override void DoShow(params object[] args)
    {
        base.DoShow(args);
        if (args?[0] is UnhandledRewardStruct unhandledRewardStruct)
        {
            _struct = unhandledRewardStruct;
            _conf = GameCore.Tables.TbReward.GetOrDefault(_struct.Id);
            _list.RenderHandler = RenderHandler;
            _list.SelectedHandler = SelectedHandler;
            _list.CanSelectHandler = CanSelectHandler;
            _list.SetData(unhandledRewardStruct.Data.Cast<object>().ToList());
            _btnGet.Pressed += BtnGetOnPressed;
            _btnCancel.Pressed += BtnCancelOnPressed;
        }
        else
        {
            Close();
        }
    }

    public override void DoClose(params object[] args)
    {
        _list.RenderHandler = null;
        _list.SelectedHandler = null;
        _list.CanSelectHandler = null;
        _ids.Clear();
        _struct = null;
        _conf = null;
        _btnGet.Pressed -= BtnGetOnPressed;
        _btnCancel.Pressed -= BtnCancelOnPressed;
        base.DoClose(args);
    }

    private bool CanSelectHandler(int idx)
    {
        switch (_conf.Type)
        {
            case RewardType.CHARACTER:
                if (_ids.Contains(_list.Array[idx] as string))
                {
                    return true;
                }

                return _ids.Count < _conf.CharacterArg3;
            case RewardType.NONE:
            case RewardType.MONEY:
            case RewardType.CARD:
            default:
                return true;
        }
    }

    private void SelectedHandler(int idx, int previousIdx)
    {
        switch (_conf.Type)
        {
            case RewardType.CHARACTER:
                if (_ids.Contains(_list.Array[idx] as string))
                {
                    _ids.Remove(_list.Array[idx] as string);
                }
                else
                {
                    _ids.Add(_list.Array[idx] as string);
                }

                break;
            case RewardType.NONE:
            case RewardType.MONEY:
            case RewardType.CARD:
            default:
                break;
        }
    }

    private void RenderHandler(Control arg1, int arg2, object arg3)
    {
        if (arg1 is RewardHeroItem heroItem)
        {
            if (heroItem.HeroItem.Hero != null) heroItem.HeroItem.RefreshItem();
            else
            {
                BaseCharacter hero = new();
                var bSuccess = hero.InitFromConfig(arg3 as string);
                if (bSuccess) heroItem.HeroItem.SetHero(hero);
            }

            heroItem.Edge.Visible = _ids.Contains(arg3);
        }
    }

    private void BtnGetOnPressed()
    {
        if (_conf == null) return;
        switch (_conf.Type)
        {
            case RewardType.CHARACTER:
                if (_ids.Count < _conf.CharacterArg3)
                {
                    AlertView.ShowAlert(new AlertViewData
                    {
                        Message = "选择的数量少于最大可领取数量，是否继续？",
                        agreeCallback = () =>
                        {
                            GameCore.ControllerMgr.SendUpdate(ControllerType.Run, CommonEvent.GetReward, _struct.Id,
                                _ids);
                            Close();
                        }
                    });
                }
                else if (_ids.Count == _conf.CharacterArg3)
                {
                    GameCore.ControllerMgr.SendUpdate(ControllerType.Run, CommonEvent.GetReward, _struct.Id, _ids);
                    Close();
                }
                else
                {
                    StaticUtil.ShowBannerHint("选择的个数错误");
                }

                break;
            case RewardType.NONE:
            case RewardType.MONEY:
            case RewardType.CARD:
            default:
                break;
        }
    }

    private void BtnCancelOnPressed()
    {
        Close();
    }
}