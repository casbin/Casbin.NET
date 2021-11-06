using Casbin.Model;

namespace Casbin
{
    public static class ReadOnlyAssertionExtension
    {
        public static bool TryGetTokenIndex(this IReadOnlyAssertion assertion, string tokenName, out int index)
        {
            return assertion.Tokens.TryGetValue($"{assertion.Key}_{tokenName}", out index);
        }
    }
}
