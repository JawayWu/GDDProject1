using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthPill : MonoBehaviour
{
    [SerializeField]
    private int m_HeatlhGain;

    public int HealthGain
    {
        get { return m_HeatlhGain; }
    }
}
