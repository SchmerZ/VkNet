namespace VkSync.Helpers
{
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