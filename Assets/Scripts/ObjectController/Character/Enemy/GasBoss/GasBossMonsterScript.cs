using System.Collections;
using System.Collections.Generic;
using System;
using Google.Protobuf.Protocol;
using UnityEngine;
using Google.Protobuf.Protocol;

public class GasBossMonsterScript : BasicMonster
{
    private enum Attack{
        EARTHQUAKE,
        FIREFLAME,
    }

    [SerializeField] float attackPeriod;
    [SerializeField] GameObject earthquakeAttackGameObj;
    [SerializeField] GameObject fireFlamePrefab;
    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsDead || !Managers.Network.IsHost) return;
        if (!hasTarget)
        {
            var size = Physics2D.OverlapCircleNonAlloc(gameObject.transform.position, detectRadius, _colliders, chaseTargetLayerMask);
            if (size > 0)
            {
                _target = _colliders[0].transform;
                hasTarget = true;
            }
        }

        if (hasTarget)
        {
            if (_target == null)
            {
                hasTarget = false;
                lastAttackTime = 0;
                return;
            }
            
            lastAttackTime += Time.deltaTime;
            if(lastAttackTime >= attackPeriod)
            {
                switch(ChooseNextAttack())
                {
                   case Attack.EARTHQUAKE:
                   _animator.SetTrigger("earthquake");
                    break;
                    case Attack.FIREFLAME:
                    _animator.SetTrigger("fireflame");
                    break;
                    default:
                    break; 
                }
            
                lastAttackTime = 0;
            }
        }
    }

   
    private Attack ChooseNextAttack()
    {
        System.Random random = new System.Random();
		Type type = typeof(Attack);

		Array values = type.GetEnumValues();

        int index = random.Next(values.Length);
		return (Attack)values.GetValue(index);
    }

    public void DoEarthquakeDamage()
    {
        earthquakeAttackGameObj.SetActive(true);
    }
    public void DoFireFlameDamage()
    {
        List<Player> values = new List<Player>(Managers.Object.PlayerDict.Values);
        foreach(var player in values)
        {
            Instantiate(fireFlamePrefab,player.transform.position, transform.rotation);
        }
    }
}
