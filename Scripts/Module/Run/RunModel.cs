using System;
using System.Collections.Generic;
using System.Linq;
using cfg.card;
using cfg.character;
using cfg.pawn;
using cfg.reward;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.MVC.Model;
using kemocard.Scripts.Pawn;
using Newtonsoft.Json;
using Attribute = cfg.pawn.Attribute;

namespace kemocard.Scripts.Module.Run;

public class RunModel(BaseController inController) : BaseModel(inController)
{
    public List<BaseCharacter> CharacterList = [];
    private HashSet<string> _allCards = [];
    public int Money;
    public Dictionary<string, UnhandledRewardStruct> UnhandledRewards { get; private set; } = [];
    private Dictionary<string, CharacterPoolStruct> _characterPool = new();
    private HashSet<string> _cardPool = [];

    public Dictionary<Role, BaseCharacter> Team = new()
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
        _allCards = [];
        Money = 0;
        UnhandledRewards = [];
        // _characterPool.Clear();
        // _cardPool.Clear();
        _characterPool = GameCore.Tables.TbHeroBaseProp.DataList.ToDictionary(hero => hero.Id, hero =>
            new CharacterPoolStruct
            {
                Attribute = hero.Attribute,
                Id = hero.Id,
                Role = hero.Role
            });
        _cardPool = GameCore.Tables.TbCard.DataList.Select(card => card.Id).ToHashSet();
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
        _allCards.UnionWith(cardIds);
        // 从卡池中删掉新增的卡牌
        _cardPool.ExceptWith(cardIds);
        GameCore.EventBus.PostEvent(CommonEvent.RunCardPoolUpdate);
    }

    public void RemoveCard(HashSet<string> cardIds)
    {
        _allCards.ExceptWith(cardIds);
        _cardPool.UnionWith(cardIds);
        GameCore.EventBus.PostEvent(CommonEvent.RunCardPoolUpdate);
    }

    public HashSet<string> GetCardsByTag(HashSet<Tag> tags, bool includeEx = false)
    {
        HashSet<string> result = [];
        if (tags == null || tags.Count == 0)
        {
            return _allCards.ToHashSet();
        }

        foreach (var cardId in from cardId in _allCards
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
            return _allCards.ToHashSet();
        }

        foreach (var cardId in from cardId in _allCards
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
            return _allCards.ToHashSet();
        }

        foreach (var cardId in from cardId in _allCards
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
        foreach (var card in from card in _allCards
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
            var i = 0;
            while (file.GetPosition() < file.GetLength())
            {
                var str = file.GetLine();
                switch (i)
                {
                    case 0:
                        CharacterList =
                            JsonConvert.DeserializeObject<List<BaseCharacter>>(str, SaveJsonSerializerSetting);
                        HashSet<string> ids = [];
                        foreach (var character in CharacterList)
                        {
                            ids.Add(character.Id);
                        }

                        _characterPool.Clear();
                        foreach (var hero in GameCore.Tables.TbHeroBaseProp.DataList.Where(hero =>
                                     !ids.Contains(hero.Id)))
                        {
                            _characterPool.Add(hero.Id, new CharacterPoolStruct
                            {
                                Attribute = hero.Attribute,
                                Id = hero.Id,
                                Role = hero.Role
                            });
                        }

                        break;
                    case 1:
                        _allCards = JsonConvert.DeserializeObject<HashSet<string>>(str, SaveJsonSerializerSetting);
                        HashSet<string> cardIds = [];
                        foreach (var card in GameCore.Tables.TbCard.DataList)
                        {
                            cardIds.Add(card.Id);
                        }

                        _cardPool = cardIds.Except(_allCards).ToHashSet();
                        break;
                    case 2:
                        Team =
                            JsonConvert.DeserializeObject<Dictionary<Role, BaseCharacter>>(
                                str,
                                SaveJsonSerializerSetting);
                        break;
                    case 3:
                        UnhandledRewards =
                            JsonConvert.DeserializeObject<Dictionary<string, UnhandledRewardStruct>>(str,
                                SaveJsonSerializerSetting);
                        break;
                    case 4:
                        Money = JsonConvert.DeserializeObject<int>(str, SaveJsonSerializerSetting);
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
        var str = JsonConvert.SerializeObject(CharacterList, SaveJsonSerializerSetting);
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
        file.StoreLine(JsonConvert.SerializeObject(_allCards));
        file.StoreLine(JsonConvert.SerializeObject(Team));
        file.StoreLine(JsonConvert.SerializeObject(UnhandledRewards));
        file.StoreLine(JsonConvert.SerializeObject(Money));
        file.Flush();
        if (OS.IsDebugBuild())
        {
            GD.Print(file.GetAsText());
        }
    }

    public void GetReward(string rewardId, HashSet<string> ids, bool isSkip = false)
    {
        if (!UnhandledRewards.TryGetValue(rewardId, out var data)) return;
        var r = new Random();
        var conf = GameCore.Tables.TbReward.GetOrDefault(rewardId);
        if (conf != null)
        {
            switch (conf.Type)
            {
                case RewardType.MONEY:
                    Money += r.Next(conf.MoneyMin, conf.MoneyMax);
                    break;
                case RewardType.CARD:
                    if (isSkip)
                    {
                        AddRandomCardFromPool(conf.Cards.Count);
                    }
                    else
                    {
                        AddCard(conf.Cards);
                    }

                    break;
                case RewardType.CHARACTER:
                    if (ids != null && data.Data.ToHashSet().IsSupersetOf(ids))
                    {
                        foreach (var id in ids)
                        {
                            AddCharacter(id);
                        }
                    }
                    else
                    {
                        StaticUtil.ShowBannerHint($"获取参数错误");
                        return;
                    }

                    break;
            }
        }

        UnhandledRewards.Remove(rewardId);
        Save();
        GameCore.EventBus.PostEvent(CommonEvent.UnhandledRewardChanged);
    }

    public void AddUnhandledReward(HashSet<string> rewardIds)
    {
        // UnhandledRewards.UnionWith(rewardIds);
        foreach (var rewardId in rewardIds)
        {
            var data = SpawnRewardById(rewardId);
            if (data != null) UnhandledRewards.Add(rewardId, data);
        }

        Save();
        GameCore.EventBus.PostEvent(CommonEvent.UnhandledRewardChanged);
    }

    private UnhandledRewardStruct SpawnRewardById(string id)
    {
        var reward = GameCore.Tables.TbReward.GetOrDefault(id);
        if (reward == null) return null;
        UnhandledRewardStruct result = new()
        {
            Id = id
        };
        Random r = new();
        switch (reward.Type)
        {
            case RewardType.NONE:
                result.Data = null;
                break;
            case RewardType.MONEY:
                result.Data = [r.Next(reward.MoneyMin, reward.MoneyMax).ToString()];
                break;
            case RewardType.CARD:
                result.Data = reward.Cards.ToList();
                break;
            case RewardType.CHARACTER:
                switch (reward.CharacterArg1)
                {
                    case 1:
                        result.Data = GetRandomRoleFromPool(reward.CharacterArg2, reward.CharacterRole,
                            reward.CharacterAttr, reward.CharacterRace);
                        break;
                    default:
                        result.Data = null;
                        break;
                }

                break;
            default:
                result.Data = null;
                break;
        }

        return result.Data == null ? null : result;
    }

    public void AddRandomCardFromPool(int amount = 1)
    {
        if (amount <= 0)
        {
            StaticUtil.ShowBannerHint("获取数量错误");
            return;
        }

        if (_cardPool.Count == 0)
        {
            StaticUtil.ShowBannerHint("已获得所有卡牌");
            return;
        }

        if (_cardPool.Count <= amount)
        {
            AddCard(_cardPool);
            return;
        }

        var r = new Random();
        var ids = new HashSet<string>();
        // 抽取amount个不重复的卡牌
        for (int i = 0; i < amount && i < _cardPool.Count; i++)
        {
            int index = r.Next(_cardPool.Count);
            var id = _cardPool.ElementAt(index);
            ids.Add(id); // 添加到结果集合中
            _cardPool.Remove(id); // 移除已选元素以避免重复
        }

        AddCard(ids);
    }

    public List<string> GetRandomRoleFromPool(int amount = 1, Role role = Role.MAX,
        Attribute attribute = Attribute.NONE, Race race = Race.NONE)
    {
        if (amount <= 0)
        {
            StaticUtil.ShowBannerHint("获取数量错误");
            return [];
        }

        List<string> filteredIds = [];

        if (role == Role.MAX && attribute == Attribute.NONE && race == Race.NONE)
        {
            filteredIds = _characterPool.Keys.ToList();
        }
        else
        {
            filteredIds.AddRange(from id in _characterPool.Values
                where (role == Role.MAX || id.Role == role) &&
                      (attribute == Attribute.NONE || id.Attribute == attribute) &&
                      (race == Race.NONE || id.Race == race)
                select id.Id);
        }

        if (filteredIds.Count <= amount)
        {
            return filteredIds;
        }

        StaticUtil.Shuffle(filteredIds);
        return filteredIds.GetRange(0, amount);
    }

    public void AddCharacter(string configId)
    {
        BaseCharacter character = new();
        var success = character.InitFromConfig(configId);
        if (success) CharacterList.Add(character);
    }
}

public record CharacterPoolStruct
{
    public string Id = "";
    public Role Role = Role.NORMAL;
    public Attribute Attribute = Attribute.NONE;
    public Race Race = Race.NONE;
}

public record UnhandledRewardStruct
{
    public string Id = "";
    public List<string> Data;
}