using UnityEngine;

namespace Utils
{
    public class LayerUtils
    {
        public static bool LayerIsOnLayerMask(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << layer));
        }
    }
}