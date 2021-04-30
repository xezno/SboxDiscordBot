namespace Disco
{
    /// <summary>
    /// like <see cref="System.Tuple"/> but cooler
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Pair<T1, T2>
    {
        private object p1;
        private object p2;

        public Pair(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public Pair(object p1, object p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }
}