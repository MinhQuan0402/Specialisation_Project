using UnityEngine;

/*
 * The base class so that we can store all modifiers in a list together so that we can iterate though them
 */
public abstract class Modifier
{
    
}

/*
 * The generic class that will be used to create modifiers with different types of values
 */
public abstract class Modifier<T> : Modifier
{
    public abstract T ModifyValue(T value);
}   