using System.Collections.Generic;
using cfg.card;
using cfg.character;

namespace kemocard.Scripts.Common;

public class CardManager
{
    public readonly Dictionary<Tag, HashSet<string>> TagCardDict = new();
    public readonly Dictionary<Role, HashSet<string>> RoleCardDict = new();

    public void ClearPool()
    {
        RoleCardDict.Clear();
        RoleCardDict[Role.NORMAL] = [];
        RoleCardDict[Role.ATTACKER] = [];
        RoleCardDict[Role.BLOCKER] = [];
        RoleCardDict[Role.GUARD] = [];
        RoleCardDict[Role.SUPPORT] = [];
        TagCardDict.Clear();
    }

    public void InitPool()
    {
        foreach (var card in GameCore.Tables.TbCard.DataList)
        {
            foreach (var tag in card.Tag)
            {
                if (!TagCardDict.ContainsKey(tag))
                {
                    TagCardDict.Add(tag, []);
                }

                TagCardDict[tag].Add(card.Id);
            }

            RoleCardDict[card.Role].Add(card.Id);
        }
    }
}