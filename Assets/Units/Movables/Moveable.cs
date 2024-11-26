using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : Unit
{
    private List<PathTile> path;
    private int index = -1;
    private bool moving = false;
    private Vector3 endPos;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        Attack(FollowTarget);

        Move();
    }

    public void SetEndPos(Vector3 movementPos, Vector3 targetPos) {
        endPos = new Vector3(movementPos.x, transform.position.y, movementPos.z);
        FindPath(0);

        SetAttackTarget(targetPos);
    }

    private void Move()
    {
        if (moving)
        {
            if (index != -1)
            {
                Vector3 nextPos = Pathfinding.Instance.GetVector(path[index], transform.position);
                Vector3 velocity = (nextPos - transform.position).normalized;
                GetComponent<MovementSpeed>().SetVelocity(velocity);
                if (velocity.z > 0)
                {
                    transform.GetChild(0).localEulerAngles = new Vector3(0, Mathf.Atan(velocity.x / velocity.z) * 180 / Mathf.PI, 0);
                }
                else if (velocity.z < 0)
                {
                    transform.GetChild(0).localEulerAngles = new Vector3(0, 180 + Mathf.Atan(velocity.x / velocity.z) * 180 / Mathf.PI, 0);
                }
                else if (velocity.z == 0 & velocity.x > 0)
                {
                    transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
                }
                else if (velocity.z == 0 & velocity.x < 0)
                {
                    transform.GetChild(0).localEulerAngles = new Vector3(0, -90, 0);
                }

                if (Vector3.Distance(transform.position, nextPos) < 1f)
                //if (Pathfinding.Instance.GetTile(transform.position).x == path[index].x && Pathfinding.Instance.GetTile(transform.position).z == path[index].z)
                {
                    Pathfinding.Instance.GetTile(path[index].x, path[index].z).Empty = false;
                    Pathfinding.Instance.GetTile(path[index].x, path[index].z).PresentUnit = transform;

                    index++;
                    if (index >= path.Count)
                    {
                        index = -1;
                    }
                    else if (!Pathfinding.Instance.GetTile(path[index].x, path[index].z).Empty)
                    {
                        StopMoving();
                        FindPath(0);
                    }
                }
                else if (!Pathfinding.Instance.GetTile(transform.position).Empty)
                {
                    RemoveFromTile();
                }
            }
            else
            {
                StopMoving();
            }
        }
    }

    private void StopMoving()
    {
        GetComponent<MovementSpeed>().SetVelocity(Vector3.zero);
        //transform.position = new Vector3(Mathf.Round(transform.position.x / 5) * 5, transform.position.y, Mathf.Round(transform.position.z / 5) * 5);
        transform.position = Pathfinding.Instance.Grid.RoundToGrid(transform.position);
        moving = false;
    }

    private void FindPath(int range)
    {
        path = Pathfinding.Instance.FindPath(transform.position, endPos, range);
        if (path != null && path.Count > 0)
        {
            index = 0;
            moving = true;
        }
    }

    private int FollowTarget() {
        if (endPos != AttackTarget.position)
        {
            endPos = AttackTarget.position;
            FindPath(Stats.attackRange);
        }
        return 0;
    }
}
