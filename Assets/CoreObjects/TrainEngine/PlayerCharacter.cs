using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerCharacter : MonoBehaviour
{
    private const int PLAYER_ID = 0;
    private Player m_player;
    private TrainEngine m_trainEngine;

    private bool m_accelerating = false;
    private bool m_decelerating = false;
    private bool m_braking = false;
    private bool m_switchJunction = false;

    private void Awake()
    {
        m_player = ReInput.players.GetPlayer(PLAYER_ID);
        m_trainEngine = GetComponent<TrainEngine>();
    }

    private void Update()
    {
        m_accelerating = m_player.GetButton("Accelerate");
        m_decelerating = m_player.GetButton("Decelerate");
        m_braking = m_player.GetButton("Brake");
        m_switchJunction = m_player.GetButtonDown("SwitchJunction");
    }

    private void FixedUpdate()
    {
        if (m_braking)
        {
            m_trainEngine.Brake();
        }
        else if (m_accelerating)
        {
            m_trainEngine.Accelerate();
        }
        else if (m_decelerating)
        {
            m_trainEngine.Decelerate();
        }

        if (m_switchJunction)
        {
            m_trainEngine.SwitchJunction();
        }
    }
}
