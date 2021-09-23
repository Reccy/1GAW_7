using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class IntersectionUI : MonoBehaviour
{
    private Player m_player;
    private Controller m_lastActiveController;
    private const int PLAYER_ID = 0;

    private SpriteRenderer m_renderer;

    [SerializeField] private Sprite m_xboxUp;
    [SerializeField] private Sprite m_xboxDown;

    [SerializeField] private Sprite m_keyboardUp;
    [SerializeField] private Sprite m_keyboardDown;

    private void Awake()
    {
        m_player = ReInput.players.GetPlayer(PLAYER_ID);
        m_renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        m_lastActiveController = m_player.controllers.GetLastActiveController();
        
        if (m_lastActiveController == null)
            return;

        if (m_lastActiveController.type == ControllerType.Keyboard)
        {
            if (m_player.GetButton("SwitchJunction"))
            {
                m_renderer.sprite = m_keyboardDown;
            }
            else
            {
                m_renderer.sprite = m_keyboardUp;
            }
        }
        else if (m_lastActiveController.type == ControllerType.Joystick)
        {
            if (m_player.GetButton("SwitchJunction"))
            {
                m_renderer.sprite = m_xboxDown;
            }
            else
            {
                m_renderer.sprite = m_xboxUp;
            }
        }
    }
}
