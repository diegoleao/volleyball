
public class NetworkVolleyball : BaseVolleyball
{

    public override void Register(BaseVolleyball volleyball)
    {
        Provider.Register<NetworkVolleyball>((NetworkVolleyball)volleyball);

    }

    public override void IncreaseScoreForTeam(Team scoringTeam)
    {
        Provider.Instance.GameState.IncreaseNetworkedScoreFor(scoringTeam);

    }

}