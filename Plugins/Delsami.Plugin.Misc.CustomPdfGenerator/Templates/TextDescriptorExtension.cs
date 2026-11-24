using DocumentFormat.OpenXml.Office.CustomUI;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Templates
{
    public static class TextDescriptorExtension
    {
        public static List<TextSpanDescriptor> GetSpans(this TextDescriptor textDescripto, string input, bool isPrice = false, float? fontSize = null, bool bold = false)
        {
            List<TextSpanDescriptor> result = new();

            if (string.IsNullOrWhiteSpace(input))
                return result;

            if (isPrice)
                input = input.Replace(",", "").Replace("٬", "");

            // Define a regular expression pattern to split text into alphabetic and numeric parts
            string pattern = @"(?<PersianDigits>[۰-۹]+)|(?<EnglishAlpha>[a-zA-Z0-9]+)|(?<Other>[^a-zA-Z0-9۰-۹]+)";

            // Use Regex to match the pattern
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);

            // Process each match and output the parts
            foreach (Match match in matches)
            {
                if (match.Groups["EnglishAlpha"].Success)
                {
                    // Alphabetic or space characters
                    string alphaPart = match.Groups["EnglishAlpha"].Value;
                    if (!string.IsNullOrEmpty(alphaPart))
                    {
                        var value = ConvertDigitsToPersian(alphaPart);

                        if (isPrice)
                            value = FormatNumbersWithCommas(value);

                        result.Add(textDescripto.Span(value).DirectionFromLeftToRight());
                    }
                }
                if (match.Groups["PersianDigits"].Success)
                {
                    // Alphabetic or space characters
                    string alphaPart = match.Groups["PersianDigits"].Value;
                    if (!string.IsNullOrEmpty(alphaPart))
                    {
                        var value = ConvertDigitsToPersian(alphaPart);

                        if (isPrice)
                            value = FormatNumbersWithCommas(value);

                        result.Add(textDescripto.Span(value).DirectionFromLeftToRight());
                    }
                }
                else if (match.Groups["Other"].Success)
                {
                    // Numeric characters
                    string numericPart = match.Groups["Other"].Value;
                    if (!string.IsNullOrEmpty(numericPart))
                    {
                        var value = ConvertDigitsToPersian(numericPart);

                        if (isPrice)
                            value = FormatNumbersWithCommas(value);


                        result.Add(textDescripto.Span(value));
                    }
                }
            }

            if (fontSize.HasValue && result.Any())
            {
                foreach (var item in result)
                    item.FontSize(fontSize.Value);
            }

            if (bold && result.Any())
                foreach (var item in result)
                    item.Bold();

            return result;
        }

        public static string ConvertDigitsToPersian(string body)
        {
            body = body.Replace("0", "۰");
            body = body.Replace("1", "۱");
            body = body.Replace("2", "۲");
            body = body.Replace("3", "۳");
            body = body.Replace("4", "۴");
            body = body.Replace("5", "۵");
            body = body.Replace("6", "۶");
            body = body.Replace("7", "۷");
            body = body.Replace("8", "۸");
            body = body.Replace("9", "۹");

            return body;
        }

        public static string ConvertDigitsToEnglish(string body)
        {
            body = body.Replace("۰", "0");
            body = body.Replace("۱", "1");
            body = body.Replace("۲", "2");
            body = body.Replace("۳", "3");
            body = body.Replace("۴", "4");
            body = body.Replace("۵", "5");
            body = body.Replace("۶", "6");
            body = body.Replace("۷", "7");
            body = body.Replace("۸", "8");
            body = body.Replace("۹", "9");

            return body;
        }

        public static string FormatNumbersWithCommas(string input)
        {
            Regex regex = new Regex(@"[\u06F0-\u06F9]+");

            // Replace all occurrences of numbers in the text with formatted versions
            string formattedText = regex.Replace(input, (match) =>
            {
                // Get the matched number string
                string numberStr = match.Value;

                // Format the number with commas
                string formattedNumber = FormatNumberWithCommas(numberStr);

                // Return the formatted number
                return formattedNumber;
            });

            return formattedText;
        }

        public static string FormatNumberWithCommas(string numberStr)
        {
            numberStr = ConvertDigitsToEnglish(numberStr);

            // Convert the number string to a long integer (to handle large numbers)
            if (long.TryParse(numberStr, out long number))
            {
                // Use standard numeric format with commas
                numberStr = number.ToString("#,##0");

                return ConvertDigitsToPersian(numberStr);
            }
            else
            {
                // If parsing fails, return the original string
                return numberStr;
            }
        }

        public static string ReverseDate(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

}
