namespace WebClient.Classies
{
    internal static class StringExtensions
    {
        internal static string UpperFirstChar(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
