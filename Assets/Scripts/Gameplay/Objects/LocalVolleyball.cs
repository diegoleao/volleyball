
public class LocalVolleyball : BaseVolleyball
{

    public override void Register(BaseVolleyball volleyball)
    {
        Provider.Register<LocalVolleyball>(volleyball);

    }

    public override void IncreaseScoreForTeam(Team scoringTeam)
    {
        Provider.Instance.GameState.IncreaseLocalScoreFor(scoringTeam);

    }

}