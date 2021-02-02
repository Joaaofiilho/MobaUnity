namespace Utils
{
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
    }
}