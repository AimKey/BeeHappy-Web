namespace Services.HelperServices;

using System.Globalization;
using System.Text;

public static class StringUtils
{
    public static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // Normalize to FormD = "decomposed" (accent marks separated)
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var ch in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            // Only keep base characters, skip diacritics
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }

        // Normalize back to FormC
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
