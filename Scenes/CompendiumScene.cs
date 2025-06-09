using System.Collections.Generic;
using System.Linq;
using cfg.character;
using cfg.pawn;
using Godot;
using kemocard.Components.AttributeSelector;
using kemocard.Components.Card;
using kemocard.Components.Hero;
using kemocard.Components.List;
using kemocard.Components.RoleSelector;
using kemocard.Components.TagSelector;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scenes;

public partial class CompendiumScene : BaseView
{
    [Export] private RoleSelector _characterRoleSlc;
    [Export] private RoleSelector _cardRoleSlc;
    [Export] private AttributeSelector _characterAttributeSlc;
    [Export] private TagSelector _cardTagSlc;
    [Export] private VirtualList _characterList;
    [Export] private VirtualList _cardList;
    [Export] private Button _closeBtn;

    public override void DoShow(params object[] args)
    {
        base.DoShow(args);
        _characterList.RenderHandler += RenderHandler;
        _characterRoleSlc.OnRoleSelected = OnRoleUpdate;
        _characterAttributeSlc.OnAttributeSelect = OnRoleUpdate;

        _cardList.RenderHandler += CardRenderHandler;
        _cardRoleSlc.OnRoleSelected = OnCardUpdate;
        _cardTagSlc.OnTagSelected = OnCardUpdate;

        _closeBtn.Pressed += Close;

        _characterRoleSlc.SetRole(Role.MAX);
        _characterAttributeSlc.SetAttribute(Attribute.NONE);
        _cardRoleSlc.SetRole(Role.MAX);
    }

    public override void DoClose(params object[] args)
    {
        _closeBtn.Pressed -= Close;
        _characterList.RenderHandler -= RenderHandler;
        _cardList.RenderHandler -= CardRenderHandler;
        _characterRoleSlc.OnRoleSelected -= OnRoleUpdate;
        _characterAttributeSlc.OnAttributeSelect -= OnRoleUpdate;
        _cardRoleSlc.OnRoleSelected -= OnCardUpdate;
        _cardTagSlc.OnTagSelected -= OnCardUpdate;
        base.DoClose(args);
    }

    private void RenderHandler(Control cell, int index, object data)
    {
        if (cell is HeroItem heroItem)
        {
            heroItem.SetHeroById(data as string);
        }
    }

    private void OnRoleUpdate()
    {
        List<object> list = [];
        list.AddRange(from hero in GameCore.Tables.TbHeroBaseProp.DataList
            where (_characterRoleSlc.CurrentRole == Role.MAX || hero.Role == _characterRoleSlc.CurrentRole) &&
                  (_characterAttributeSlc.CurrentAttribute == Attribute.NONE ||
                   ((int)hero.Attribute & (int)_characterAttributeSlc.CurrentAttribute) > 0)
            select hero.Id);

        _characterList.SetData(list);
    }

    private void CardRenderHandler(Control cell, int index, object data)
    {
        if (cell is CardBigItem cardItem)
        {
            cardItem.InitById(data as string);
        }
    }

    private void OnCardUpdate()
    {
        List<object> list = [];
        list.AddRange(from card in GameCore.Tables.TbCard.DataList
            where card.HideInDex == false
                  && (_cardRoleSlc.CurrentRole == Role.MAX || card.Role == _cardRoleSlc.CurrentRole)
                  && card.Tag.IsSupersetOf(_cardTagSlc.CurrentTags)
            select card.Id);

        _cardList.SetData(list);
    }
}