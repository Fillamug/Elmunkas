using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CombatHelper;

public abstract class Unit : MonoBehaviour
{
    public event EventHandler OnHealthChanged;

    [SerializeField]
    private Stats stats;

    [SerializeField]
    private Team team;

    private int health;

    private Transform attackTarget = null;

    private float attackTimer, followTimer;

    protected virtual void Start()
    {
        OnHealthChanged += UpdateHealthBar;
        attackTimer = Stats.attackSpeed;
        health = Stats.maxHealth;
        followTimer = 0;
        Pathfinding.Instance.GetTile(transform.position).Empty = false;
        Pathfinding.Instance.GetTile(transform.position).PresentUnit = transform;
    }

    protected virtual void Update()
    {
        if (team != Test.playerTeam)
        {
            Collider[] possibleTarget = Physics.OverlapSphere(transform.position, stats.attackRange * Pathfinding.Instance.Grid.getScale());
            if (possibleTarget.Length > 0)
            {
                int i = 0;
                while (possibleTarget[i].transform.GetComponentInParent<Unit>()?.team == team)
                {
                    i++;
                }
                if (possibleTarget[i].transform.GetComponentInParent<Unit>()!=null)
                SetAttackTarget(possibleTarget[i].transform.position);
            }
        }
    }

    public Transform AttackTarget { get => attackTarget; set => attackTarget = value; }
    public Stats Stats { get => stats; set => stats = value; }
    public Team Team { get => team; set => team = value; }

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        if (health <= 0)
        {
            if (AttackTarget != null)
            {
                if (!AttackTarget.GetChild(0).GetComponent<Select>().selected)
                {
                    AttackTarget?.Find("HealthBar").gameObject.SetActive(false);
                    AttackTarget?.Find("SelectionCircle").gameObject.SetActive(false);
                }
            }
            Pathfinding.Instance.GetTile(transform.position).Empty = true;
            Pathfinding.Instance.GetTile(transform.position).PresentUnit = null;
            Destroy(transform.gameObject);
        }
    }

    public void GetHealed(int heal) {
        health += heal;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        if (health > Stats.maxHealth) health = Stats.maxHealth;
    }

    protected void SetAttackTarget (Vector3 targetPos){
        if (AttackTarget != null) {
            if (!AttackTarget.GetChild(0).GetComponent<Select>().selected)
            {
                AttackTarget?.Find("HealthBar").gameObject.SetActive(false);
                AttackTarget?.Find("SelectionCircle").gameObject.SetActive(false);
            }
        }
        Pathfinding.Instance.Grid.WorldToGrid(targetPos, out int x, out int y);
        if (Pathfinding.Instance.Grid.GetValue(x, y).PresentUnit != transform)
        {
            AttackTarget = Pathfinding.Instance.Grid.GetValue(x, y).PresentUnit;
            AttackTarget?.Find("HealthBar").gameObject.SetActive(true);
            AttackTarget?.Find("SelectionCircle").gameObject.SetActive(true);
        }
        else AttackTarget = null;
    }

    protected void Attack(Func<int> attackMethod) {
        if (AttackTarget != null) {
            Pathfinding.Instance.Grid.WorldToGrid(transform.position, out int x, out int y);
            Pathfinding.Instance.Grid.WorldToGrid(AttackTarget.position, out int targetX, out int targetY);
            attackTimer += Time.deltaTime;
            followTimer += Time.deltaTime;
            if (Math.Floor(Math.Sqrt((targetX - x) * (targetX - x) + (targetY - y) * (targetY - y))) <= Stats.attackRange)
            {
                if (attackTimer >= Stats.attackSpeed)
                {
                    AttackTarget.GetComponent<Unit>().TakeDamage(calculateDamage());
                    attackTimer = 0;
                }
            }
            else {
                if (followTimer >= 0.2f)
                {
                    followTimer = 0;
                    attackMethod();
                }
            }
        }
    }

    private void UpdateHealthBar(object sender, EventArgs e) {
        transform.Find("HealthBar").Find("Bar").localScale = new Vector3((float)health / Stats.maxHealth, 1, 1);
    }

    private int calculateDamage() {
        return (int)Math.Round(Stats.attackDamage * MultiplyVector4ByVector4(AttackTypeVector(stats.attackType), damageMatrix * ArmorTypeVector(AttackTarget.GetComponent<Unit>().stats.armorType)));
    }
}
