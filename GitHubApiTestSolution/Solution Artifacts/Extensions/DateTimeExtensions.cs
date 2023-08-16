using System;

namespace GitHubApiTestSolution.Solution_Artifacts.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Outputs a DateTime i.e. 10/07/2023 as "10th July 2023"
        /// Change bool outputFullMonthName to false to output "10th Jul 2023"
        /// </summary>
        /// <param name="inputDate"></param>
        /// <param name="outputFullMonthName"></param>
        /// <returns></returns>
        public static string FormatDateWithBackgroundNumbers(this DateTime inputDate, bool outputFullMonthName = true)
        {
            var day = inputDate.Day;
            var ordinal = GetOrdinalSuffix(day);

            var monthFormat = outputFullMonthName ? "MMMM" : "MMM";
            var formattedDate = $"{day}{ordinal} {inputDate.ToString(monthFormat)} {inputDate.Year}";

            return formattedDate;
        }

        private static string GetOrdinalSuffix(int day)
        {
            if (day >= 11 && day <= 13)
            {
                return "th";
            }

            switch (day % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

    }
}
