using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HideWall : MonoBehaviour
{
    [SerializeField]
    private float m_ShiftAmount;

    private bool m_shift;
    private Vector3 m_FinalPos;

    private void Awake()
    {
        m_shift = false;
        m_FinalPos = transform.position + Vector3.down * m_ShiftAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_shift && transform.position.y > m_FinalPos.y)
        {
            transform.Translate(Vector3.down * Time.deltaTime);
        }
        else if (transform.position.y <= m_FinalPos.y)
        {
            Destroy(gameObject);
        }
    }

    public void Shift()
    {
        m_shift = true;
    }
}
