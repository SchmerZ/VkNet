namespace VkSync.Helpers
{
    public class StringPair : Pair<string, string>
    {
        public StringPair(string first, string second) 
            : base(first, second)
        { }
    }

    public class Pair<T, K>
    {
        public Pair(T first, K second)
        {
            First = first;
            Second = second;
        }

        public T First
        {
            get; 
            set;
        }

        public K Second
        {
            get; 
            set;
        }
    }
}