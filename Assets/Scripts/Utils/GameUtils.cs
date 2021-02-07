using System;
using UnityEngine;

namespace Utils
{
    public static class GameUtils
    {
        public static Vector3[] GetMinionCustomDestinationsByLane(Teams team, Lanes lane)
        {
            return GameAssets.i.GetMinionsPath(team, lane);
        }

        public static String[] TagsOfAllUnits()
        {
            return new[]
            {
                Tags.Champion.Value,
                Tags.Minion.Value,
                Tags.Tower.Value
            };
        }
    }
}