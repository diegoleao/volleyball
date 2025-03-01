using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //Inspector
    [Header("Player Attributes")]
    [SerializeField] private float speed = 6;

    [Header("Prefabs")]
    [SerializeField] private PhysxBall _prefabPhysxBall;


    //Networked
    [Networked] private TickTimer delay { get; set; }

    [Networked] public bool spawnedProjectile { get; set; }


    //Private
    private NetworkCharacterController netCharController;

    private Vector3 forward;


    void Awake()
    {
        netCharController = GetComponent<NetworkCharacterController>();
        forward = Vector3.forward;

    }


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            MoveCharacter(data);

            SpawnPrefabOnButtonPress(data);

        }

    }

    private void MoveCharacter(NetworkInputData data)
    {
        data.direction.Normalize();

        netCharController.Move(data.direction * Runner.DeltaTime);

    }

    private void SpawnPrefabOnButtonPress(NetworkInputData data)
    {

        if (data.direction.sqrMagnitude > 0)
            forward = data.direction;

        if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
        {
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                Runner.Spawn(_prefabPhysxBall,
                              transform.position + forward,
                              Quaternion.LookRotation(forward),
                              Object.InputAuthority,
                              (runner, obj) =>
                              {
                                  obj.GetComponent<PhysxBall>().Init(10 * forward);
                              });

                spawnedProjectile = !spawnedProjectile;

            }
            else if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
            {
                //?

            }

        }

    }

}