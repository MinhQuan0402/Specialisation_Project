using System;
using UnityEngine;

namespace Utilities
{
    public class DistanceNotifier
    {
        public event Action OnNotify;
        
        private Vector3 referencePos;
        private float sqrDistance;
        
        private bool enabled;
        
        private Action<float> notifierCondition;

        public void Init(Vector3 referencePos, float distance, bool checkInside = false,
            bool triggerContinuously = false)
        {
            this.referencePos = referencePos;
            sqrDistance = distance * distance;
            if (checkInside)
            {
                notifierCondition = CheckInside;
            }
            else
            {
                notifierCondition = CheckOutside;
            }
            
            enabled = true;

            if (!triggerContinuously)
            {
                OnNotify += Disable;
            }
        }
        
        public void Disable()
        {
            enabled = false;

            OnNotify -= Disable;
        }
        
        public void Tick(Vector3 pos)
        {
            if (!enabled)
                return;

            // We are using the square of distances as square root function is expensive
            var currentSqrDistance = (referencePos - pos).sqrMagnitude;
            
            // Pass current distance to function stored within the Action. Avoids having to do an if else check every tick and instead moves that check to constructor.
            notifierCondition.Invoke(currentSqrDistance);
        }

        private void CheckInside(float dist)
        {
            if (dist <= sqrDistance)
            {
                OnNotify?.Invoke();
            }
        }

        private void CheckOutside(float dist)
        {
            if (dist >= sqrDistance)
            {
                OnNotify?.Invoke();
            }
        }
    }
}