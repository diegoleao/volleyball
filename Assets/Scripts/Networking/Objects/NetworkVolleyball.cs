
public class NetworkVolleyball : BaseVolleyball
{

    public override void Register(BaseVolleyball volleyball)
    {
        Provider.Register<NetworkVolleyball>(volleyball);

    }

    public override void IncreaseScoreForTeam(Team scoringTeam)
    {
        Provider.GameState.IncreaseNetworkedScoreFor(scoringTeam);

    }

}