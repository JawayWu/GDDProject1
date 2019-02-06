using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttackInfo
{
    [SerializeField]
    private string m_Name;
    public string AttackName { get { return m_Name; } }
    [SerializeField]
    private string m_Button;
    public string Button { get { return m_Button; } }
    [SerializeField]
    private string m_TriggerName;
    public string TriggerName { get { return m_TriggerName; } }
    [SerializeField]
    private GameObject m_AbilityGO; // Ability Game Object
    public GameObject AbilityGO { get { return m_AbilityGO; } }
    [SerializeField]
    private float m_WindupTime; // Ability Channel
    public float WindupTime { get { return m_WindupTime; } }
    [SerializeField]
    private float m_FrozenTime; // Time held in place during Channel
    public float FrozenTime { get { return m_FrozenTime; } }
    [SerializeField]
    private float m_Cooldown;
    [SerializeField]
    private int m_HealthCost;
    public int HealthCost { get { return m_HealthCost; } }
    [SerializeField]
    private Color m_Color;
    public Color AbilityColor { get { return m_Color; } }

    public float Cooldown // VARIABLE
    {
        get;
        set;
    }

    public void ResetCooldown()
    {
        Cooldown = m_Cooldown;
    }

    public bool IsReady()
    {
        return Cooldown <= 0;
    }
}
