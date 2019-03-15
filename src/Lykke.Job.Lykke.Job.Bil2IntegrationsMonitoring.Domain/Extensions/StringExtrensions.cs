using System.Text;

namespace Lykke.Job.Lykke.Job.Bil2IntegrationsMonitoring.Domain.Extensions
{
    public static class StringExtrensions
    {
        public static string UseLowercaseAndUnderscoreDelimeter(this string str)
        {
            var sb = new StringBuilder(str.Length);

            for (int i = 0; i < str.Length-1; i++)
            {
                sb.Append(char.ToLower(str[i]));
                if (char.IsLower(str[i]) && char.IsUpper(str[i + 1]))
                    sb.Append("_");
            }

            sb.Append(char.ToLower(str[str.Length - 1]));

            return sb.ToString();
        }
    }
}
