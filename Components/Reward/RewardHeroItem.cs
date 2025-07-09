using Godot;
using kemocard.Components.Hero;
using kemocard.Components.List;

namespace kemocard.Components.Reward;

public partial class RewardHeroItem : Control, ISelectableItem
{
    [Export] public HeroItem HeroItem;
    [Export] public ColorRect Edge;
    public int Index { get; set; }
    public VirtualList List { get; set; }
}