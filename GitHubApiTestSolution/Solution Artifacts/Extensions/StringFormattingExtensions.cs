using System.Text.RegularExpressions;

namespace GitHubApiTestSolution.Solution_Artifacts.Extensions
{
    public static class StringFormattingExtensions
    {
        /// <summary>
        /// Use Regex to keep only single spaces, and remove any extra spaces
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveExtraSpaces(this string input)
        {
            return Regex.Replace(input, @"\s+", " ");
        }
    }
}
