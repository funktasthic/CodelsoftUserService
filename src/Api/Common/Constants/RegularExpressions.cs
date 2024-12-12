using System.Text.RegularExpressions;

namespace UserService.Api.Common.Constants
{
    public static partial class RegularExpressions
    {
        /// <summary>
        /// Regex for password validation, must have at least one letter and one number
        /// </summary>
        public const string PasswordValidation = @"^(?=.*[a-zA-Z])(?=.*\d).+$";

        [GeneratedRegex("^([0-9]+-[0-9K])$", RegexOptions.Compiled)]
        public static partial Regex RutRegex();

        [GeneratedRegex("^([a-zA-Z]+\\.)*ucn\\.cl$", RegexOptions.Compiled)]
        public static partial Regex UCNEmailDomainRegex();
    }
}