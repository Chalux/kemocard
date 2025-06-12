using System.Collections.Generic;
using System.Linq;
using cfg.card;
using cfg.character;
using cfg.pawn;
using Godot.Collections;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Run;

public class RunController : BaseController
{
    public RunController()
    {
        GameCore.ViewMgr.Register(ViewType.TeamEditView, new ViewInfo
        {
            Controller = this,
            ResPath = GameCore.GetScenePath("TeamEditView"),
            ViewName = "TeamEditView",
            ViewType = ViewType.TeamEditView,
        });
        GameCore.ViewMgr.Register(ViewType.DeckEditView, new ViewInfo
        {
            Controller = this,
            ResPath = GameCore.GetScenePath("DeckEditView"),
            ViewName = "DeckEditView",
            ViewType = ViewType.DeckEditView,
        });
        RunModel model = new(this);
        SetModel(model);
        model.Load();
    }

    public void ResetModel()
    {
        RunModel model = GetModel<RunModel>();
        model?.Init();
    }

    public void AddCharacter(BaseCharacter character)
    {
        RunModel model = GetModel<RunModel>();
        model?.CharacterList.Add(character);
    }

    public void AddCharacter(string configId)
    {
        BaseCharacter character = new();
        character.InitFromConfig(configId);
        AddCharacter(character);
    }

    public Array<BaseCharacter> GetCharacters(Role filterRole = Role.MAX, int filterAttribute = 0)
    {
        RunModel model = GetModel<RunModel>();
        var list = model?.CharacterList;
        if (list == null)
        {
            return [];
        }

        Array<BaseCharacter> result = [];
        foreach (var baseCharacter in list)
        {
            if ((filterRole == Role.MAX || baseCharacter.Role == filterRole)
                && (filterAttribute == 0 || (baseCharacter.Attribute & filterAttribute) > 0))
            {
                result.Add(baseCharacter);
            }
        }

        return result;
    }

    public BaseCharacter GetCharacterById(string id)
    {
        var model = GetModel<RunModel>();
        return model?.CharacterList?.ToList().Find(character => character.Id == id);
    }

    public HashSet<string> GetCardPoolByTag(HashSet<Tag> tags)
    {
        var model = GetModel<RunModel>();
        return model?.GetCardsByTag(tags);
    }

    public HashSet<string> GetCardPoolByRole(HashSet<Role> roles)
    {
        var model = GetModel<RunModel>();
        return model?.GetCardsByRole(roles);
    }

    public HashSet<string> GetCardPoolByRole(Role role)
    {
        var model = GetModel<RunModel>();
        return model?.GetCardsByRole(role);
    }

    public HashSet<string> FilterCardPool(HashSet<Tag> tags, Role role = Role.MAX)
    {
        var model = GetModel<RunModel>();
        return model?.FilterCardPool(tags, role);
    }

    public void Save()
    {
        var model = GetModel<RunModel>();
        model?.Save();
    }

    public void Load()
    {
        GetModel<RunModel>()?.Load();
    }
}