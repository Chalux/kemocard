using System.Collections.Generic;
using System.Linq;
using cfg.character;
using Godot;
using kemocard.Components.Card;
using kemocard.Components.List;
using kemocard.Components.RoleSelector;
using kemocard.Components.TagSelector;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;
using kemocard.Scripts.Pawn;

namespace kemocard.Scenes;

public partial class DeckEditView : BaseView
{
    private string _heroId = "";
    private BaseCharacter _hero;
    [Export] private GridContainer _itemList;
    [Export] private Label _healthLab;
    [Export] private Label _pDefenseLab;
    [Export] private Label _mDefenseLab;
    [Export] private Label _pAttackLab;
    [Export] private Label _mAttackLab;
    [Export] private Label _healLab;
    [Export] private Label _deckLab;
    [Export] private Label _nameLab;
    [Export] private TextureRect _headIcon;
    [Export] private Button _closeBtn;
    [Export] private Button _joinTeamBtn;
    [Export] private VirtualList _cardList;
    [Export] private RoleSelector _roleSelector;
    [Export] private TagSelector _tagSelector;
    private bool _hasChanged;

    public override void DoShow(params object[] args)
    {
        base.DoShow(args);
        if (args[0] == null)
        {
            Close();
            return;
        }

        _closeBtn.Pressed += CloseBtnOnPressed;
        _joinTeamBtn.Pressed += JoinTeamBtnOnPressed;

        switch (args[0])
        {
            case string:
            {
                _heroId = (string)args[0];
                var runController = GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run);
                _hero = runController.GetCharacterById(_heroId);
                break;
            }
            case BaseCharacter character:
                _hero = character;
                _heroId = _hero.Id;
                break;
            default:
                Close();
                return;
        }

        GameCore.EventBus.AddEvent(CommonEvent.PlayerDeckUpdate, RefreshData);
        _roleSelector.OnRoleSelected = OnRoleSelected;
        _tagSelector.OnTagSelected = OnRoleSelected;
        RefreshContainer();
    }

    private void JoinTeamBtnOnPressed()
    {
        var mod = GameCore.ControllerMgr.GetControllerModel<RunModel>(ControllerType.Run);
        if (mod == null || _hero == null) return;
        mod.SetTeam(_hero.Role, mod.Team.GetValueOrDefault(_hero.Role) == _hero ? null : _hero);
        RefreshContainer();
    }

    private void CloseBtnOnPressed()
    {
        if (_hasChanged)
        {
            var runController = GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run);
            runController.Save();
        }

        GameCore.ViewMgr.CloseView(ViewId);
    }

    public override void DoClose(params object[] args)
    {
        _hero = null;
        _heroId = "";
        _cardList.RenderHandler = null;
        _closeBtn.Pressed -= CloseBtnOnPressed;
        _joinTeamBtn.Pressed -= JoinTeamBtnOnPressed;
        GameCore.EventBus.RemoveEvent(CommonEvent.PlayerDeckUpdate, RefreshData);
        _roleSelector.OnRoleSelected = null;
        _tagSelector.OnTagSelected = null;
        base.DoClose(args);
    }

    private void RefreshContainer()
    {
        if (_hero == null)
        {
            return;
        }

        _cardList.RenderHandler = RenderHandler;
        _cardList.SelectedHandler = SelectedHandler;

        RefreshData();
    }

    private void SelectedHandler(int newIndex, int oldIndex)
    {
        var data = _cardList.Array?[newIndex];
        if (data is not string configId) return;
        if (_hero.GetDeck().Any(baseCard => baseCard.Id == configId))
        {
            _hero.RemoveCardById(configId);
        }
        else
        {
            var conf = GameCore.Tables.TbCard.GetOrDefault(configId);
            if (conf == null) return;
            if (conf.Role != Role.NORMAL && conf.Role != _hero.Role)
            {
                StaticUtil.ShowBannerHint("与当前角色职能不同，无法加入卡组");
                return;
            }

            _hero.AddCard(configId);
        }

        _hasChanged = true;
    }

    private void RefreshData(object data = null)
    {
        if (_hero == null) return;
        var mod = GameCore.ControllerMgr.GetControllerModel<RunModel>(ControllerType.Run);
        _healthLab.Text = _hero.MaxHealth.ToString();
        _pDefenseLab.Text = _hero.PDefense.ToString();
        _mDefenseLab.Text = _hero.MDefense.ToString();
        _pAttackLab.Text = _hero.PAttack.ToString();
        _mAttackLab.Text = _hero.MAttack.ToString();
        _healLab.Text = _hero.Heal.ToString();
        _deckLab.Text = _hero.GetDeckString();
        bool isInTeam = mod != null && mod.Team.ContainsValue(_hero);
        _nameLab.Text = _hero.Name + (isInTeam ? " (已上阵)" : "");
        _joinTeamBtn.Text = isInTeam ? "下阵" : "上阵";
        if (FileAccess.FileExists(_hero.Icon)) _headIcon.Texture = ResourceLoader.Load<CompressedTexture2D>(_hero.Icon);
        var deck = _hero.GetDeck();
        var nodes = _itemList.GetChildren();
        for (var index = 0; index < nodes.Count; index++)
        {
            var child = nodes[index];
            if (child is not CardItem cardItem) continue;
            if (deck.Count > index)
            {
                cardItem.Init(deck[index]);
            }
            else
            {
                cardItem.Clear();
            }
        }

        _roleSelector.SetRole(_roleSelector.CurrentRole);
    }

    private void OnRoleSelected()
    {
        var cardSet = new HashSet<string>();
        var config = GameCore.Tables.TbHeroBaseProp.Get(_heroId);
        if (config != null)
        {
            foreach (var cardId in from cardId in config.CardList
                     let cardConfig = GameCore.Tables.TbCard.Get(cardId)
                     where cardConfig != null &&
                           (_roleSelector.CurrentRole == Role.MAX || cardConfig.Role == _roleSelector.CurrentRole) &&
                           cardConfig.Tag.IsSupersetOf(_tagSelector.CurrentTags)
                     select cardId)
            {
                cardSet.Add(cardId);
            }
        }

        cardSet.UnionWith(GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run)
            .FilterCardPool(_tagSelector.CurrentTags, _roleSelector.CurrentRole));
        _cardList.SetData(cardSet.ToList<object>());
    }

    private void RenderHandler(Control cell, int index, object data)
    {
        if (cell is not CardItem card) return;
        if (data is string cardId)
        {
            card.Init(new BaseCard(cardId));
        }
        else
        {
            card.Clear();
        }

        card.SetSelected(_hero.GetDeck().Any(baseCard => baseCard.Id == data as string));
    }
}