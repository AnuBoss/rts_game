using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator anim;
    private Unit unit;
    void Start()
    {
        anim = GetComponent<Animator>();
        unit = GetComponent<Unit>();

    }

    // Update is called once per frame
    void Update()
    {
        ChooseAnimation(unit);

    }

    private void ChooseAnimation(Unit u)
    {
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsMove", false);
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsBuilding", false);

        switch (u.State)
        {
            case UnitState.Idle:
                anim.SetBool("IsIdle", true);
                break;
            case UnitState.Move:
                anim.SetBool("IsMove", true);
                break;
            case UnitState.Attack:
                anim.SetBool("IsAttack", true);
                break;

            case UnitState.Building:
                anim.SetBool("IsBuilding", true);
                break;

        }
    }
}
