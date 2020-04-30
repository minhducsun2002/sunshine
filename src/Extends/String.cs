using System;
namespace sunshine
{
    static class StringExtension
    {
        public static string toSentencedCase(this string _)
        {
            if (_.Length < 1) return _;
            return _.Substring(0, 1).ToUpperInvariant() + _.Substring(1, Math.Max(1, _.Length - 1));
        }
    }
}