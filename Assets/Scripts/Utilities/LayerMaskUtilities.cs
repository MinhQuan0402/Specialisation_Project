using UnityEngine;

namespace Utilities
{
    public static class LayerMaskUtilities
    {
        public static bool IsLayerInMask(int layer, LayerMask mask) => (mask & (1 << layer)) != 0;
        
        public static bool IsLayerInMask(RaycastHit2D hit, LayerMask mask) => IsLayerInMask(hit.collider.gameObject.layer, mask);
    }
}