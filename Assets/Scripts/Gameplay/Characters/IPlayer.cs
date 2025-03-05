
public interface IPlayer
{
    public void Initialize(Team team, bool isAI);

    public bool IsAI { get; }

}