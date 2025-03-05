using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte BUTTON_0_FIRE = 1;
    public const byte BUTTON_1_JUMP = 2;
    public const byte BUTTON_2 = 3;

    public NetworkButtons buttons;
    public Vector3 direction;

}