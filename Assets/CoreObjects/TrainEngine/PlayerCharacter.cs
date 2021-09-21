using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerCharacter : MonoBehaviour
{
    private const int PLAYER_ID = 0;
    private Player m_player;
    private TrainEngine m_trainEngine;

    private bool Accelerating => m_player.GetButton("Accelerate");
    private bool Decelerating => m_player.GetButton("Decelerate");
    private bool Braking => Accelerating && Decelerating;

    private void Awake()
    {
        m_player = ReInput.players.GetPlayer(PLAYER_ID);
        m_trainEngine = GetComponent<TrainEngine>();
    }

    private void FixedUpdate()
    {
        if (Braking)
        {
            m_trainEngine.Brake();
        }
        else if (Accelerating)
        {
            m_trainEngine.Accelerate();
        }
        else if (Decelerating)
        {
            m_trainEngine.Decelerate();
        }
    }
}
