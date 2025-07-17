using cfg.card;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class BaseBuffCard(string buffId) : BaseCardScript
{
    private string _buffId = buffId;

    public sealed override void UseCard(BaseBattleCard parent)
    {
        var buff = StaticUtil.NewBuffOrNullById(_buffId, parent.User);
        if (buff == null) return;
        // if (parent.Tags.Contains(Tag.SELF))
        // {
        //     parent.User.AddBuff(buff);
        // }
        // else if (parent.Tags.Contains(Tag.AT))
        // {
        //     foreach (var battleCharacter in Mod.Teammates)
        //     {
        //         battleCharacter.AddBuff(buff);
        //     }
        // }
        // else if (parent.Tags.Contains(Tag.AE))
        // {
        //     foreach (var battleEnemy in Mod.Enemies)
        //     {
        //         battleEnemy.AddBuff(buff);
        //     }
        // }
        // else if (parent.Tags.Contains(Tag.SE) || parent.Tags.Contains(Tag.ST))
        // {
        //     foreach (var basePawn in parent.Target)
        //     {
        //         basePawn.AddBuff(buff);
        //     }
        // }
        foreach (var target in parent.Target)
        {
            target.AddBuff(buff);
        }
    }
}