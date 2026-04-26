using System.Text.RegularExpressions;

namespace AmazonBestSellersExplorer.WebAPI.Helpers
{
    public static partial class ValidationHelper
    {
        [GeneratedRegex(@"^[a-zA-Z0-9!@#\$%\^&\*\(\)_\+\-\=\[\]\{\}\|;:'"",\.<>\/\?\`~]+$", RegexOptions.Compiled)]
        private static partial Regex LoginCharactersRegex();

        [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$", RegexOptions.Compiled)]
        private static partial Regex PasswordComplexityRegex();

        [GeneratedRegex(@"([0-9.]+)", RegexOptions.Compiled)]
        private static partial Regex StarRatingRegex();

        public static bool IsValidLoginCharacters(string login)
            => LoginCharactersRegex().IsMatch(login);

        public static bool IsValidPasswordComplexity(string password)
            => PasswordComplexityRegex().IsMatch(password);

        public static string? ExtractFirstNumber(string input)
        {
            var match = StarRatingRegex().Match(input);
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}
