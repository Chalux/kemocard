using System.Collections.Generic;
using System.Linq;
using cfg.card;
using cfg.character;
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
        GameCore.ViewMgr.Register(ViewType.GetRewardView, new ViewInfo()
        {
            Controller = this,
            ResPath = GameCore.GetScenePath("GetRewardView"),
            ViewName = "GetRewardView",
            ViewType = ViewType.GetRewardView,
        });
        RunModel model = new(this);
        SetModel(model);
        // model.Load();
    }

    public override void InitModuleEvent()
    {
        RegisterEvent(CommonEvent.GetReward, GetReward);
        RegisterEvent(CommonEvent.AddUnhandledReward, AddUnhandledReward);
    }

    public void ResetModel()
    {
        RunModel model = GetModel<RunModel>();
        model?.Init();
    }

    public void AddCharacter(string configId)
    {
        RunModel model = GetModel<RunModel>();
        model?.AddCharacter(configId);
    }

    public List<BaseCharacter> GetCharacters(Role filterRole = Role.MAX, int filterAttribute = 0)
    {
        RunModel model = GetModel<RunModel>();
        var list = model?.CharacterList;
        if (list == null)
        {
            return [];
        }

        List<BaseCharacter> result = [];
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

    private void GetReward(object[] args)
    {
        if (args[0] is not string rewardStruct) return;
        var model = GetModel<RunModel>();
        bool isSkip = args.Length > 2 && (bool)args[2];
        var ids = args[1] as List<string>;
        model?.GetReward(rewardStruct, ids?.ToHashSet(), isSkip);
    }

    private void AddUnhandledReward(object[] args)
    {
        if (args[0] is not HashSet<string> rewardIds) return;
        var model = GetModel<RunModel>();
        model?.AddUnhandledReward(rewardIds);
    }

    public void AddUnhandledReward(HashSet<string> rewardIds)
    {
        (Model as RunModel)?.AddUnhandledReward(rewardIds);
    }
}