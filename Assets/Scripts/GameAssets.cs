using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public sealed class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (!_i)
            {
                _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            }

            return _i;
        }
    }

    private static List<Vector3> _minionsTopPath;
    private static List<Vector3> _minionsMidPath;
    private static List<Vector3> _minionsBottomPath;

    public GameObject damagePopupPrefab;
    [SerializeField] private GameObject minionsTopPathPrefab;
    [SerializeField] private GameObject minionsMidPathPrefab;
    [SerializeField] private GameObject minionsBottomPathPrefab;
    
    public Material blueTeamMaterial;
    public Material redTeamMaterial;

    public Vector3[] GetMinionsPath(Teams team, Lanes lane)
    {
        List<Vector3> finalPath = null;
        switch (lane)
        {
            case Lanes.Top:
                if (_minionsTopPath == null)
                {
                    _minionsTopPath = GetPath(minionsTopPathPrefab);
                }
                finalPath = _minionsTopPath;
                break;
            case Lanes.Mid:
                if (_minionsMidPath == null)
                {
                    _minionsMidPath = GetPath(minionsMidPathPrefab);
                }
                finalPath = _minionsMidPath;
                break;
            case Lanes.Bottom:
                if (_minionsBottomPath == null)
                {
                    _minionsBottomPath = GetPath(minionsBottomPathPrefab);
                }
                finalPath = _minionsBottomPath;
                break;
        }
        
        //Removing the first/last 2 elements because the path is reused to both blue and red team. The first two elements are
        //always for the opposite team, so it isn't necessary.
        var pathVector = new Vector3[finalPath.Count - 2];
        for (var i = 0; i < finalPath.Count - 2; i++)
        {
            pathVector[i] = finalPath[i];
        }

        return team == Teams.Red ? pathVector.Reverse().ToArray() : finalPath?.GetRange(2, finalPath.Count - 2).ToArray();

    }

    private static List<Vector3> GetPath(GameObject prefab)
    {
        var pathList = new List<Vector3>();
        
        var numOfChildren = prefab.transform.childCount;

        for (var i = 0; i < numOfChildren; i++)
        {
            pathList.Add(prefab.transform.GetChild(i).position);
        }

        return pathList;
    }
}
