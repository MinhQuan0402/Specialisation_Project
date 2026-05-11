using UnityEngine;

/*
 * Delegate that defines a condition for applying a weapon modifier. It takes in the source of the attack and the directional information, and returns a boolean indicating whether the modifier should be applied.
 */
public delegate bool ConditionalDelegate(Transform source, out DirectionalInformation directionalInfo);