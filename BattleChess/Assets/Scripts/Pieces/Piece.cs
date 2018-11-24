using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PieceMotor))]
[RequireComponent(typeof(Sensor))]
public abstract class Piece : MonoBehaviour, IDamageable
{
    public GameObject deathEffect;

    public ParticleSystem shootEffect;

    public Piece Target;

    [HideInInspector]
    public PieceMotor motor;

    [HideInInspector]
    public Sensor sensor;

    [Tooltip("How many tiles a pice can walk.")]
    public int Speed = 1;

    [Tooltip("If it reach 0, the piece is destroyed.")]
    public int Hitpoints = 1;

    [ReadOnly]
    public int _currentHitpoints;

    public int CurrentHitpoints
    {
        get
        {
            return this._currentHitpoints;
        }
        set
        {
            this._currentHitpoints = value;
            if (this.OnHealthChange != null)
            {
                this.OnHealthChange(this);
            }
        }
    }

    public int AttackPower = 1;

    public int ShootRange = 1;

    public DetectionMethod DetectionMethod;

    public AttackMethod AttackMethod;

    public int CurrentX { get; set; }

    public int CurrentY { get; set; }

    public bool IsHuman;

    public bool WalkConsumed { get; set; }

    private bool _actionConsumed;

    public bool ActionConsumed
    {
        get
        {
            return this._actionConsumed;
        }
        set
        {
            this._actionConsumed = value;

            if (this.OnActionStatusChange != null)
            {
                this.OnActionStatusChange(this);
            }
        }
    }

    public bool IsTurnComplete { get; set; }

    public bool IsDead { get; set; }

    public Type PieceType { get; set; }

    public event Action<Piece> OnTurnCompleted;

    public event Action<Piece> OnAttackComplete;

    public event Action<Piece> OnHealthChange;

    public event Action<Piece> OnActionStatusChange;

    private void Piece_OnTurnCompleted(Piece obj)
    {
        this.FinishTurn();
    }

    protected virtual void Awake()
    {
        this.sensor = GetComponent<Sensor>();
        this.motor = GetComponent<PieceMotor>();
        this.OnTurnCompleted += Piece_OnTurnCompleted;
        this.CurrentHitpoints = this.Hitpoints;
        this.PieceType = this.GetType();
    }

    protected float GetDistanceToPosition(Vector3 position)
    {
        return (position - transform.position).sqrMagnitude;
    }

    protected float GetDistanceBetweenPositions(Vector3 p1, Vector3 p2)
    {
        return (p1 - p2).sqrMagnitude;
    }

    public void SetPosition(int x, int y)
    {
        this.CurrentX = x;
        this.CurrentY = y;
    }

    public void InvokeOnTurnComplete()
    {
        if (this.OnTurnCompleted != null)
        {
            this.OnTurnCompleted(this);
        }
    }

    public virtual void FinishTurn()
    {
        this.IsTurnComplete = true;
        this.WalkConsumed = true;
        this.ActionConsumed = true;
    }

    public virtual void Attack()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (motor.isMoving)
        {
            yield return null;
        }

        if (this.sensor.AttackTargets != null && this.sensor.AttackTargets.Count > 0)
        {
            Piece faceTarget = this.sensor.AttackTargets[UnityEngine.Random.Range(0, this.sensor.AttackTargets.Count)];
            this.motor.FaceTarget(faceTarget.transform.position);

            while (motor.isRotating)
            {
                yield return null;
            }

            if (shootEffect != null)
            {
                shootEffect.Play();
            }

            float distance = Vector3.Distance(transform.position, faceTarget.transform.position);
            float delay = distance * 0.06429f; // magic number :)

            yield return new WaitForSeconds(delay);

            if (this.AttackMethod == AttackMethod.All)
            {
                foreach (var target in this.sensor.AttackTargets)
                {
                    target.TakeHit(this.AttackPower, Vector3.one);
                }
            }
            else if (this.AttackMethod == AttackMethod.Single)
            {
                faceTarget.TakeHit(this.AttackPower, Vector3.one);
            }
        }


        if (this.OnAttackComplete != null)
        {
            this.OnAttackComplete(this);
        }

    }

    public void TakeHit(int damage, Vector3 hitDirection)
    {
        this.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        this.CurrentHitpoints -= damage;
        Debug.Log(string.Format("{0} at {1}:{2} took {3} damage. HP: {4}/{5}",
            this.PieceType.Name, this.CurrentX, this.CurrentY, damage, this.CurrentHitpoints, this.Hitpoints));
        if (this.CurrentHitpoints <= 0 && !IsDead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    private void Die()
    {
        if (!this.ActionConsumed)
        {
            RoundManager.Instance.TotalActionsLeft--;
            if (IsHuman)
            {
                RoundManager.Instance.PlayerActionsLeft--;
            }
            else
            {
                RoundManager.Instance.AIActionsLeft--;
            }
        }
        Debug.Log(string.Format("{0} at {1}:{2} died!", this.PieceType.Name, this.CurrentX, this.CurrentY));
        this.WalkConsumed = true;
        this.ActionConsumed = true;
        this.IsDead = true;
        PlayerManager.Instance.Pieces.Remove(this);
        BoardManager.Instance.Pieces[this.CurrentX, this.CurrentY] = null;

        Destroy(Instantiate(deathEffect.gameObject, transform.position, Quaternion.identity) as GameObject, 2);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        var listeners = this.OnTurnCompleted.GetInvocationList();
        foreach (var item in listeners)
        {
            this.OnTurnCompleted -= (item as Action<Piece>);
        }

        // TODO: unsubscribe all other events. 
        // OnTurnCompleted;
        // OnDeath;
        // OnAttackComplete;
    }

    public abstract bool[,] PossibleMoves();
}
