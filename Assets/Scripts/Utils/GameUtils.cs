using System;
using UnityEngine;

namespace Utils
{
    public static class GameUtils
    {
        private static readonly Vector3[] CustomDestinationsTop = {
            new Vector3(-62f, 0, 31.7f), 
            new Vector3(-51.66f, 0, 40.92f),
            new Vector3(-35.79f, 0, 47.48f),
            new Vector3(26.67f, 0, 46.87f),
            new Vector3(48.28f, 0, 44.17f)
        };
        
        private static readonly Vector3[] CustomDestinationsMid = {
            new Vector3(48.28f, 0, 44.17f)
        };
        
        private static readonly Vector3[] CustomDestinationsBottom = {
            new Vector3(32.19f, 0, -49.04f),
            new Vector3(47.97f, 0, -40.77f),
            new Vector3(54.08f, 0, -31.68f),
            new Vector3(51.44f, 0, -38.69f),
            new Vector3(48.28f, 0, 44.17f)
        };
        
        public static Vector3[] GetMinionCustomDestinationsByLane(Lanes lane)
        {
            switch (lane)
            {
                case Lanes.Top:
                    return CustomDestinationsTop;
                case Lanes.Mid:
                    return CustomDestinationsMid;
                case Lanes.Bottom:
                    return CustomDestinationsBottom;
                default:
                    return null;
            }
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