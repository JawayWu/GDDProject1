using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    private int m_MaxHealth = 3;
    [SerializeField]
    [Tooltip("How fast the player moves around.")]
    private float m_Speed;
    [SerializeField]
    [Tooltip("How high the player jumps.")]
    private float m_JumpStrength;
    [SerializeField]
    private float m_RotationSpeed;
    [SerializeField]
    private PlayerAttackInfo[] m_Attacks;
    [SerializeField]
    private HUD m_Hud;
    #endregion
    #region Private Variables
    private Vector2 p_Velocity;
    private bool p_isJumping;
    private float p_rotationAmount;
    private float p_FrozenTimer = 0;
    private Color p_DefaultColor;
    private float p_AnimForwardSpeed;
    private float p_CurHealth;
    #endregion
    #region Cached Components
    private Rigidbody cc_Rb;
    private ConfigurableJoint cc_Spring;
    #endregion
    #region Cached References
    private Animator cr_Anim;
    private Renderer cr_Renderer;
    #endregion
    #region Initialization
    private void Awake()
    {
        p_CurHealth = m_MaxHealth;
        p_AnimForwardSpeed = 0;
        p_Velocity = Vector2.zero;
        p_isJumping = false;
        p_rotationAmount = 0;
        p_FrozenTimer = 0;
        cc_Rb = GetComponent<Rigidbody>();
        cc_Spring = GetComponent<ConfigurableJoint>();

        for (int i = 0; i < m_Attacks.Length; i++)
        {
            PlayerAttackInfo attack = m_Attacks[i];
            attack.Cooldown = 0;
            if (attack.WindupTime > attack.FrozenTime)
                Debug.LogError(attack.AttackName + " has a channel time that is longer than its frozen time");
        }
    }

    private void Start()
    {
        cr_Anim = GetComponentInChildren<Animator>();
        cr_Renderer = GetComponentInChildren<Renderer>();
        p_DefaultColor = cr_Renderer.material.color;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion
    #region Main Updates
    private void Update()
    {
        if (p_FrozenTimer > 0)
        {
            p_FrozenTimer -= Time.deltaTime;
            p_Velocity = Vector2.zero;
            return;
        }
        else
            p_FrozenTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, 1 << LayerMask.NameToLayer("Floor")))
        {
            cc_Spring.targetPosition = new Vector3(0, -hit.point.y, 0);
            if (p_isJumping && Mathf.Abs(transform.position.y - hit.point.y) < 1.05f)
                p_isJumping = false;
        }
        else
            cc_Spring.targetPosition = new Vector3(0, 100, 0);

        float forward = Input.GetAxisRaw("Vertical");
        float right = Input.GetAxisRaw("Horizontal");

        p_Velocity.Set(right, forward);
        p_Velocity.Normalize();

        p_AnimForwardSpeed = Mathf.Lerp(p_AnimForwardSpeed, p_Velocity.magnitude, .3f);
        cr_Anim.SetFloat("Speed", p_AnimForwardSpeed);

        if (Input.GetButtonDown("Jump") && !p_isJumping)
        {
            cc_Rb.AddForce(Vector3.up * m_JumpStrength, ForceMode.Impulse);
            p_isJumping = true;
        }

        p_rotationAmount = Input.GetAxis("Mouse X");

        for (int i = 0; i < m_Attacks.Length; i++)
        {
            PlayerAttackInfo attack = m_Attacks[i];
            if (attack.IsReady())
            {
                if (Input.GetButtonDown(attack.Button))
                {
                    p_FrozenTimer = attack.FrozenTime;
                    DecreaseHealth(attack.HealthCost);
                    StartCoroutine(UseAttack(attack));
                    break;
                }
            }
            else
                attack.Cooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Vector3 dir = transform.forward * p_Velocity.y + transform.right * p_Velocity.x;
        cc_Rb.MovePosition(cc_Rb.position + dir * m_Speed * Time.fixedDeltaTime);

        cc_Rb.angularVelocity = Vector3.zero;
        cc_Rb.MoveRotation(cc_Rb.rotation * Quaternion.Euler(0, m_RotationSpeed * p_rotationAmount, 0));
    }
    #endregion
    #region Health Methods
    public void DecreaseHealth(float amount)
    {
        p_CurHealth -= amount;
        m_Hud.UpdateHealth(1.0f * p_CurHealth / m_MaxHealth);
        if (p_CurHealth <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void IncreaseHealth(float amount)
    {
        p_CurHealth += amount;
        if (p_CurHealth > m_MaxHealth)
        {
            p_CurHealth = m_MaxHealth;
        }
        m_Hud.UpdateHealth(1.0f * p_CurHealth / m_MaxHealth);
    }
    #endregion
    #region Attack Methods
    private IEnumerator ChangeColor(Color newColor, float speed)
    {
        if (speed > 100)
        {
            Debug.LogWarning("Speed must be lower than 100.");
        }
        else if (speed <= 0)
        {
            Debug.LogWarning("Speed must be greater than 0.");
        }
        Color curColor = cr_Renderer.material.color;
        while (curColor != newColor)
        {
            curColor = Color.Lerp(curColor, newColor, speed / 100);
            cr_Renderer.material.color = curColor;
            yield return null;
        }
    }

    private IEnumerator UseAttack(PlayerAttackInfo attack)
    {
        cr_Anim.SetTrigger(attack.TriggerName);
        IEnumerator toColor = ChangeColor(attack.AbilityColor, 10);
        StartCoroutine(toColor);
        yield return new WaitForSeconds(attack.WindupTime);
        Vector3 hitpoint = transform.forward * attack.AbilityGO.GetComponent<Ability>().Range;
        GameObject go = Instantiate(attack.AbilityGO, transform.position + transform.forward * 0.2f, transform.rotation);
        go.GetComponent<Ability>().Use(transform.position, hitpoint);
        StopCoroutine(toColor);
        StartCoroutine(ChangeColor(p_DefaultColor, 50));
        yield return new WaitForSeconds(attack.FrozenTime - attack.WindupTime);
        attack.ResetCooldown();
    }
    #endregion
    #region Collision Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPill"))
        {
            IncreaseHealth(other.GetComponent<HealthPill>().HealthGain);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Start"))
        {
            other.GetComponent<ArenaStart>().Spawner.StartSpawning();
        }
        if (other.CompareTag("Teleporter"))
        {
            transform.position = other.GetComponent<Teleporter>().EndPoint;
        }
    }
    #endregion
    #region Access Methods
    public int health
    {
        get; private set;
    }
    #endregion
}
