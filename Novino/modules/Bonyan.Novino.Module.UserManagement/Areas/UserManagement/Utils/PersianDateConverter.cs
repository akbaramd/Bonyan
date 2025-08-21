using System.Globalization;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Utils
{
    public static class PersianDateConverter
    {
        private static readonly PersianCalendar PersianCalendar = new PersianCalendar();
        private static readonly CultureInfo PersianCulture = new CultureInfo("fa-IR");

        /// <summary>
        /// Converts Persian numbers to English numbers
        /// </summary>
        /// <param name="input">String containing Persian or English numbers</param>
        /// <returns>String with all numbers converted to English</returns>
        public static string NormalizeNumbers(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input ?? string.Empty;

            return input
                .Replace('۰', '0')
                .Replace('۱', '1')
                .Replace('۲', '2')
                .Replace('۳', '3')
                .Replace('۴', '4')
                .Replace('۵', '5')
                .Replace('۶', '6')
                .Replace('۷', '7')
                .Replace('۸', '8')
                .Replace('۹', '9');
        }

        /// <summary>
        /// Converts Persian date string (YYYY/MM/DD) to DateTime
        /// </summary>
        /// <param name="persianDate">Persian date in format YYYY/MM/DD</param>
        /// <returns>DateTime in Gregorian calendar</returns>
        public static DateTime? PersianToGregorian(string? persianDate)
        {
            if (string.IsNullOrWhiteSpace(persianDate))
                return null;

            try
            {
                // Normalize numbers (convert Persian to English)
                var normalizedDate = NormalizeNumbers(persianDate);
                
                // Parse Persian date parts
                var parts = normalizedDate.Split('/');
                if (parts.Length != 3)
                    return null;

                if (!int.TryParse(parts[0], out int year) ||
                    !int.TryParse(parts[1], out int month) ||
                    !int.TryParse(parts[2], out int day))
                    return null;

                // Validate Persian date
                if (year < 1300 || year > 1500 || month < 1 || month > 12 || day < 1 || day > 31)
                    return null;

                // Convert to Gregorian
                return PersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts DateTime to Persian date string (YYYY/MM/DD)
        /// </summary>
        /// <param name="gregorianDate">DateTime in Gregorian calendar</param>
        /// <returns>Persian date string in format YYYY/MM/DD</returns>
        public static string? GregorianToPersian(DateTime? gregorianDate)
        {
            if (!gregorianDate.HasValue)
                return null;

            try
            {
                var year = PersianCalendar.GetYear(gregorianDate.Value);
                var month = PersianCalendar.GetMonth(gregorianDate.Value);
                var day = PersianCalendar.GetDayOfMonth(gregorianDate.Value);

                return $"{year:0000}/{month:00}/{day:00}";
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts DateTime to Persian date string with Persian numbers (YYYY/MM/DD)
        /// </summary>
        /// <param name="gregorianDate">DateTime in Gregorian calendar</param>
        /// <returns>Persian date string with Persian numbers</returns>
        public static string? GregorianToPersianWithPersianNumbers(DateTime? gregorianDate)
        {
            var englishDate = GregorianToPersian(gregorianDate);
            if (string.IsNullOrWhiteSpace(englishDate))
                return englishDate;

            return englishDate
                .Replace('0', '۰')
                .Replace('1', '۱')
                .Replace('2', '۲')
                .Replace('3', '۳')
                .Replace('4', '۴')
                .Replace('5', '۵')
                .Replace('6', '۶')
                .Replace('7', '۷')
                .Replace('8', '۸')
                .Replace('9', '۹');
        }

        /// <summary>
        /// Converts DateTime to Persian date string with full format
        /// </summary>
        /// <param name="gregorianDate">DateTime in Gregorian calendar</param>
        /// <returns>Persian date string in full format</returns>
        public static string? GregorianToPersianFull(DateTime? gregorianDate)
        {
            if (!gregorianDate.HasValue)
                return null;

            try
            {
                return gregorianDate.Value.ToString("yyyy/MM/dd", PersianCulture);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Validates if a Persian date string is valid
        /// </summary>
        /// <param name="persianDate">Persian date string</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidPersianDate(string? persianDate)
        {
            if (string.IsNullOrWhiteSpace(persianDate))
                return false;

            // Normalize numbers first
            var normalizedDate = NormalizeNumbers(persianDate);
            
            // Check basic format
            var parts = normalizedDate.Split('/');
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out int year) ||
                !int.TryParse(parts[1], out int month) ||
                !int.TryParse(parts[2], out int day))
                return false;

            // Validate ranges
            return year >= 1300 && year <= 1500 && 
                   month >= 1 && month <= 12 && 
                   day >= 1 && day <= 31;
        }

        /// <summary>
        /// Gets age from Persian date of birth
        /// </summary>
        /// <param name="persianDateOfBirth">Persian date of birth</param>
        /// <returns>Age in years</returns>
        public static int GetAgeFromPersianDate(string? persianDateOfBirth)
        {
            // Normalize numbers first
            var normalizedDate = NormalizeNumbers(persianDateOfBirth);
            var gregorianDate = PersianToGregorian(normalizedDate);
            if (!gregorianDate.HasValue)
                return 0;

            var today = DateTime.Today;
            var age = today.Year - gregorianDate.Value.Year;
            
            if (gregorianDate.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }

        /// <summary>
        /// Gets age from Gregorian date of birth
        /// </summary>
        /// <param name="gregorianDateOfBirth">Gregorian date of birth</param>
        /// <returns>Age in years</returns>
        public static int GetAgeFromGregorianDate(DateTime? gregorianDateOfBirth)
        {
            if (!gregorianDateOfBirth.HasValue)
                return 0;

            var today = DateTime.Today;
            var age = today.Year - gregorianDateOfBirth.Value.Year;
            
            if (gregorianDateOfBirth.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
} 