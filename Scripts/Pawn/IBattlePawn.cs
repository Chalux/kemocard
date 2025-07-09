namespace kemocard.Scripts.Pawn;

public interface IBattlePawn
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public bool IsDead { get; set; }
    public int PDefense { get; set; }
    public int MDefense { get; set; }
}