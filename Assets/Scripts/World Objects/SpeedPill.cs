using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPill : MonoBehaviour
{
    [SerializeField]
    private int m_speedGain;

    public int SpeedGain
    {
        get { return m_speedGain; }
    }
}