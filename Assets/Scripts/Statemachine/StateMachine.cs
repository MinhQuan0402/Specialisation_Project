using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class StateMachine
{
    public Dictionary<string, State> States { get; private set; } = new();

    public State CurrentState { get; private set; }

    public void InitializeStates(ref SerializableDictionary<string, State> states, bool singleInstance = true)
    {
        Dictionary<string, State> statesToDic = states.ToDictionary(); //Convert seriazable dictionary to dictionary
        if (singleInstance) //if only for 1 instance
        {
            States = statesToDic;
            return;
        }

        //for many instances
        foreach (var state in statesToDic)
        {
            AddState(state.Key, state.Value.GetType());
        }
        states.FromDictionary(States); //return the data of 
    }

    public void AddState<T>(string name) where T : State
    {
        T stateToAdd = ScriptableObject.CreateInstance<T>();
        States.Add(name, stateToAdd);
    }

    public void AddState(string name, Type type)
    {
        State stateToAdd = ScriptableObject.CreateInstance(type) as State;
        States.Add(name, stateToAdd);
    }

    public State GetState(string name)
    {
        if (States.TryGetValue(name, out State state)) return state;
        return null;
    }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public T GetState<T>() where T : State //Contraint the type of T
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        var component = States.Values.OfType<T>().FirstOrDefault();
        if (component) return component;

        return null;
    }

    //To get core component and assign the reference variable as the return value
    public T GetStateWithRef<T>(ref T value) where T : State
    {
        value = GetState<T>();
        return value;
    }

    public T GetStateWithRef<T>(string nameOfState, ref T value) where T : State
    {
        if (!States.ContainsKey(nameOfState)) return null;
        value = States[nameOfState] as T;
        return value;
    }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public bool RemoveState<T>() where T : State //Contraint the type of T
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        var state = GetState<T>();
        foreach (KeyValuePair<string, State> pairs in States)
        {
            if (pairs.Value == state)
            {
                States.Remove(pairs.Key);
                return true;
            }
        }

        Debug.LogWarning($"{typeof(T)} is not found in {CurrentState.Core.transform.parent.name}");
        return false;
    }

    public bool RemoveState(string name)
    {
        if(States.TryGetValue(name, out State state))
        {
            States.Remove(name);

            return true;
        }

        Debug.LogWarning($"{name} state is not found in {CurrentState.Core.transform.parent.name}");
        return false;
    }

    public void InitializeStartingState(State startingState)
    {
        if (startingState == null) return;

        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void InitializeStartingState(string startingStateName)
    {
        if(!States.TryGetValue(startingStateName, out State state))
        {
            CurrentState = States.First().Value;
            Debug.LogError($"{startingStateName} is not found in {CurrentState.Core.transform.parent.name}");
        }

        CurrentState = state;
        CurrentState.Enter();
    }

    public void ChangeState(State newState)
    {
        if(newState == null || CurrentState == newState) return;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void ChangeState(string newState)
    {
        if(!States.TryGetValue(newState, out State state))
        {
            Debug.LogWarning($"{CurrentState.GetType()}: {newState} is not found in {CurrentState.Core.transform.parent.name}");
        }

        CurrentState.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }

    public void LogicUpdate()
    {
        if (CurrentState == null) return;
        CurrentState.LogicUpdate();
    }

    public void PhysicsUpdate()
    {
        if(CurrentState == null) return;
        CurrentState.PhysicsUpdate();
    }
}
