namespace ModernCode.Network.QueryString
{
    public class QueryParameterAttribute : QueryParameterBaseAttribute
    {
        public QueryParameterAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}
