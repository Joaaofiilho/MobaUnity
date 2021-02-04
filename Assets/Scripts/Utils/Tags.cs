namespace Utils
{
    /// <summary>
    /// Here are centered all the tags used on the game.
    /// Important note: Remember to add all Units to the GameUtils.TagsOfAllUnits.
    /// </summary>
    public class Tags
    {
        private Tags(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Tags Map => new Tags("Map");
        public static Tags Minion => new Tags("Minion");
        public static Tags Champion => new Tags("Champion");
        public static Tags Tower => new Tags("Tower");
    }
}