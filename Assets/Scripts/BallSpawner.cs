
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private VolleyBall ball;

    public void Spawn()
    {
        var instance = Instantiate(ball);
        instance.transform.position = new Vector3(Random.Range(-2, 2), 5, Random.Range(-2, 2));

    }

}