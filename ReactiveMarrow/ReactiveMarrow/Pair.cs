namespace ReactiveMarrow
{
    public struct Pair<T>
    {
        public Pair(T left, T right)
            : this()
        {
            this.Left = left;
            this.Right = right;
        }

        public T Left { get; private set; }

        public T Right { get; private set; }
    }
}