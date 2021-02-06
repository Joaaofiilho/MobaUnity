using UnityEngine;

public sealed class GlobalPrefabs : MonoBehaviour
{
    private static GlobalPrefabs _i;

    public static GlobalPrefabs i
    {
        get
        {
            if (_i == null)
            {
                _i = Instantiate(Resources.Load<GlobalPrefabs>("GlobalPrefabs"));
            }

            return _i;
        }
    }

    public GameObject damagePopupPrefab;
}
