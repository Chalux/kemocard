using System.Linq;
using cfg.character;
using Godot;
using kemocard.Components.Hero;
using kemocard.Components.List;
using kemocard.Components.RoleSelector;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;
using kemocard.Scripts.Pawn;

namespace kemocard.Scenes;

public partial class TeamEditView : BaseView
{
    [Export] private PackedScene _heroTeamItem;
    [Export] private Button _btnClose;
    [Export] private Control _teamContainer;
    [Export] private RoleSelector _roleSelector;
    [Export] private VirtualList _heroList;
    private RunModel _mod;

    public override void DoShow(params object[] args)
    {
        base.DoShow(args);
        _mod = GameCore.ControllerMgr.GetControllerModel<RunModel>(ControllerType.Run);
        _btnClose.Pressed += BtnCloseOnPressed;
        _heroList.RenderHandler = RenderHandler;
        _heroList.SelectedHandler = SelectedHandler;
        _roleSelector.OnRoleSelected = RefreshContainer;
        _roleSelector.DisableNormal(true);
        GameCore.EventBus.AddEvent(this, CommonEvent.TeamListUpdate, RefreshContainer);
        RefreshContainer();
    }

    private void SelectedHandler(int selectedIndex, int previousIndex)
    {
        if (_heroList.Array?.Count > selectedIndex && _heroList.Array[selectedIndex] != null)
        {
            GameCore.ViewMgr.OpenView(ViewType.DeckEditView, _heroList.Array[selectedIndex]);
        }
    }

    private void RenderHandler(Control arg1, int arg2, object arg3)
    {
        if (arg1 is HeroTeamItem heroItem)
        {
            heroItem.SetHero(arg3 as BaseCharacter);
        }
    }

    private void BtnCloseOnPressed()
    {
        GameCore.ViewMgr.CloseView(ViewId);
    }

    public override void DoClose(params object[] args)
    {
        _btnClose.Pressed -= BtnCloseOnPressed;
        _heroList.RenderHandler = null;
        _roleSelector.OnRoleSelected = null;
        GameCore.EventBus.RemoveEvent(this, CommonEvent.TeamListUpdate, RefreshContainer);
        base.DoClose();
    }

    private void RefreshContainer(object args)
    {
        RefreshContainer();
    }

    private void RefreshContainer()
    {
        var controller = GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run);
        _heroList.SetData(controller.GetCharacters(_roleSelector.CurrentRole).ToList<object>());
        if (_teamContainer.GetChild(0) is HeroTeamItem item)
        {
            item.SetHero(_mod.Team[Role.ATTACKER]);
        }

        item = _teamContainer.GetChild(1) as HeroTeamItem;
        item?.SetHero(_mod.Team[Role.GUARD]);
        item = _teamContainer.GetChild(2) as HeroTeamItem;
        item?.SetHero(_mod.Team[Role.BLOCKER]);
        item = _teamContainer.GetChild(3) as HeroTeamItem;
        item?.SetHero(_mod.Team[Role.SUPPORT]);
    }
}