namespace NetCasbin.Evaluation
{
    public interface IExpressionCache<T> : IExpressionCache
    {
        public bool TryGet(string expressionString, out T t);

        public void Set(string expressionString, T t);
    }

    public interface IExpressionCache
    {
        public void Clear();
    }
}
