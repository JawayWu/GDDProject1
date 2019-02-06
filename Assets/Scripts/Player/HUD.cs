using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HUD : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    private RectTransform m_HealthBar;
    #endregion

    #region Private Variables
    private float m_HealthBarOrigWidth;
    #endregion

    #region Initialization
    private void Awake()
    {
        m_HealthBarOrigWidth = m_HealthBar.sizeDelta.x;
    }
    #endregion

    #region Update Health Bar
    public void UpdateHealth(float percent)
    {
        m_HealthBar.sizeDelta = new Vector2(m_HealthBarOrigWidth * percent, m_HealthBar.sizeDelta.y);
    }
    #endregion
}