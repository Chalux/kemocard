using System;
using System.Collections.Generic;
using System.Linq;
using cfg.card;
using cfg.character;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.MVC.Model;
using kemocard.Scripts.Pawn;
using Newtonsoft.Json;

namespace kemocard.Scripts.Module.Run;

public class RunModel(BaseController inController) : BaseModel(inController)
{
    public List<BaseCharacter> CharacterList = [];
    public HashSet<string> AllCards = [];

    public System.Collections.Generic.Dictionary<Role, BaseCharacter> Team = new()
    {
        { Role.ATTACKER, null },
        { Role.GUARD, null },
        { Role.BLOCKER, null },
        { Role.SUPPORT, null },
    };

    public override void Init()
    {
        base.Init();
        CharacterList = [];
        AllCards = [];
        Team[Role.ATTACKER] = null;
        Team[Role.GUARD] = null;
        Team[Role.BLOCKER] = null;
        Team[Role.SUPPORT] = null;
    }

    public void SetTeam(Role role, BaseCharacter character)
    {
        if (!Team.ContainsKey(role)) return;
        Team[role] = character;
        Save();
        GameCore.EventBus.PostEvent(CommonEvent.TeamListUpdate);
    }

    public void AddCard(HashSet<string> cardIds)
    {
        AllCards.UnionWith(cardIds);
        GameCore.EventBus.PostEvent(CommonEvent.RunCardPoolUpdate);
    }

    public void RemoveCard(HashSet<string> cardIds)
    {
        AllCards.ExceptWith(cardIds);
        GameCore.EventBus.PostEvent(CommonEvent.RunCardPoolUpdate);
    }

    public HashSet<string> GetCardsByTag(HashSet<Tag> tags, bool includeEx = false)
    {
        HashSet<string> result = [];
        if (tags == null || tags.Count == 0)
        {
            return AllCards.ToHashSet();
        }

        foreach (var cardId in from cardId in AllCards
                 let config = GameCore.Tables.TbCard.Get(cardId)
                 where config != null && (!includeEx || config.Exclusive) && config.Tag.IsSupersetOf(tags)
                 select cardId)
        {
            result.Add(cardId);
        }

        return result;
    }

    public HashSet<string> GetCardsByRole(HashSet<Role> roles, bool includeEx = false)
    {
        HashSet<string> result = [];
        if (roles == null || roles.Count == 0 || roles.Contains(Role.MAX))
        {
            return AllCards.ToHashSet();
        }

        foreach (var cardId in from cardId in AllCards
                 let config = GameCore.Tables.TbCard.Get(cardId)
                 where config != null && (!includeEx || config.Exclusive) && roles.Contains(config.Role)
                 select cardId)
        {
            result.Add(cardId);
        }

        return result;
    }

    public HashSet<string> GetCardsByRole(Role role, bool includeEx = false)
    {
        HashSet<string> result = [];
        if (role == Role.MAX)
        {
            return AllCards.ToHashSet();
        }

        foreach (var cardId in from cardId in AllCards
                 let config = GameCore.Tables.TbCard.Get(cardId)
                 where config != null && (!includeEx || config.Exclusive) && config.Role == role
                 select cardId)
        {
            result.Add(cardId);
        }

        return result;
    }

    public HashSet<string> FilterCardPool(HashSet<Tag> tags, Role role, bool includeEx = false)
    {
        HashSet<string> result = [];
        foreach (var card in from card in AllCards
                 let config = GameCore.Tables.TbCard.Get(card)
                 where config != null && (!includeEx || config.Exclusive) &&
                       (role == Role.MAX || config.Role == role) &&
                       (tags == null || tags.Count == 0 || config.Tag.IsSupersetOf(tags))
                 select card)
        {
            result.Add(card);
        }

        return result;
    }

    public const string RunSavePath = "user://run.sav";

    public static readonly JsonSerializerSettings SaveJsonSerializerSetting = new()
    {
        TypeNameHandling = TypeNameHandling.Arrays,
    };

    public void Load()
    {
        if (!FileAccess.FileExists(RunSavePath))
        {
            Init();
            return;
        }

        Init();

        using var file = FileAccess.Open(RunSavePath, FileAccess.ModeFlags.Read);
        GD.Print(file.GetAsText());
        try
        {
            int i = 0;
            while (file.GetPosition() < file.GetLength())
            {
                var str = file.GetLine();
                switch (i)
                {
                    case 0:
                        CharacterList =
                            JsonConvert.DeserializeObject<List<BaseCharacter>>(str, SaveJsonSerializerSetting);
                        break;
                    case 1:
                        AllCards = JsonConvert.DeserializeObject<HashSet<string>>(str, SaveJsonSerializerSetting);
                        break;
                    case 2:
                        Team =
                            JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<Role, BaseCharacter>>(
                                str,
                                SaveJsonSerializerSetting);
                        break;
                }

                i++;
            }
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
        // var strings = Json.ParseString(file.GetLine()).As<Array<string>>() ?? new();
        // foreach (var s in strings)
        // {
        //     BaseCharacter c = new BaseCharacter();
        //     c.FromDict(s);
        //     CharacterList.Add(c);
        // }
        //
        // AllCards = Json.ParseString(file.GetLine()).As<Array<string>>()?.ToHashSet() ?? new();
    }

    public void Save()
    {
        using var file = FileAccess.Open(RunSavePath, FileAccess.ModeFlags.WriteRead);
        var str = "";
        str = JsonConvert.SerializeObject(CharacterList, SaveJsonSerializerSetting);
        // var strings = new Array<string>();
        // foreach (var character in CharacterList)
        // {
        //     strings.Add(character.ToDict());
        // }
        //
        // str += Json.Stringify(strings);
        // file.StoreLine(Json.Stringify(strings));
        // str += Json.Stringify(AllCards.ToArray());
        // file.StoreLine(Json.Stringify(AllCards.ToArray()));
        // GD.Print(str);
        file.StoreLine(str);
        file.StoreLine(JsonConvert.SerializeObject(AllCards));
        file.StoreLine(JsonConvert.SerializeObject(Team));
        file.Flush();
        if (OS.IsDebugBuild())
        {
            GD.Print(file.GetAsText());
        }
    }
}