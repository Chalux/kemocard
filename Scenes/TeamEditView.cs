using Godot;
using kemocard.Components.Hero;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scenes;

public partial class TeamEditView : BaseView
{
    [Export] private FlowContainer _flowContainer;
    [Export] private PackedScene _heroTeamItem;
    [Export] private Button _btnClose;

    public override void DoShow(params object[] args)
    {
        RefreshContainer();
        _btnClose.Pressed += BtnCloseOnPressed;
    }

    private void BtnCloseOnPressed()
    {
        GameCore.ViewMgr.CloseView(ViewId);
    }

    public override void DoClose(params object[] args)
    {
        var children = _flowContainer.GetChildren();
        foreach (var child in children)
        {
            _flowContainer.RemoveChild(child);
            child.QueueFree();
        }

        _btnClose.Pressed -= BtnCloseOnPressed;
        base.DoClose();
    }

    private void RefreshContainer()
    {
        var controller = GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run);
        var characters = controller.GetCharacters();
        var children = _flowContainer.GetChildren();
        for (var i = 0; i < characters.Count; i++)
        {
            var node = children.Count > i ? _flowContainer.GetChild<HeroTeamItem>(i) : null;
            if (node == null)
            {
                node = _heroTeamItem.Instantiate<HeroTeamItem>();
                node.SetClick(true);
                _flowContainer.AddChild(node);
            }

            node.SetHero(characters[i]);
        }
    }
}