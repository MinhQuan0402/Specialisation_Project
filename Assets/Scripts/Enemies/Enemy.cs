using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<string, State> StatesDictionary = new();

    [SerializeField] private EnemyData enemyData;

    public Core Core {  get; private set; }
    public StateMachine StateMachine { get; private set; }
    public Rigidbody2D RB {  get; private set; } 
    public Animator Anim { get; private set; }
    
    private CoreComp<Stats> stats;

    private void Awake()
    {
        Core = GetComponentInChildren<Core>();
        StateMachine = new StateMachine();
        StateMachine.InitializeStates(ref StatesDictionary, false);
        
        stats = new CoreComp<Stats>(Core);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();

        foreach(KeyValue<string, State> pairs in StatesDictionary.pairs)
        {
            ((EnemyState)pairs.Value).Init(this, enemyData);
        }
        StateMachine.InitializeStartingState("move");
        stats.Comp.Poise.OnCurrentValueZero += HandlePoiseZero;
    }

    private void HandlePoiseZero()
    {
        //Change To Stun State
        StateMachine.ChangeState("stun");
    }
 
    private void OnDestroy()
    {
        stats.Comp.Poise.OnCurrentValueZero -= HandlePoiseZero;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        StateMachine.LogicUpdate();
        Core.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
    }
}
