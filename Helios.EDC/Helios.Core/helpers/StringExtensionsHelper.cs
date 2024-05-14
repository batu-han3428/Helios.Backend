using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Helios.Core.helpers
{
    public static class StringExtensionsHelper
    {
        public static string StyleOptionMerge(this int[] styleType)
        {
            if (styleType == null)
                return "0px 0px 0px 0px;";
            for (int i = 0; i < styleType.Count(); i++)
            {
                if (styleType[i].Equals(null))
                    styleType[i] = 0;
            }
            string result = styleType[0].ToString() + "px " + styleType[1].ToString() + "px " + styleType[2].ToString() + "px " + styleType[3].ToString() + "px;";
            return result;
        }

        public static int[] StyleOptionDivision(this string styleType)
        {
            var strValue = styleType.ToString().Substring(0, styleType.Length - 1);
            var trimSplit = strValue.Split(' ');
            for (int i = 0; i < trimSplit.Length; i++)
            {
                trimSplit[i] = trimSplit[i].ToString().Substring(0, trimSplit[i].Length - 2);
            }
            return Array.ConvertAll(trimSplit, int.Parse);
        }

        public static string ConvertNullStrToEmptyStr(this string value)
        {
            if (string.IsNullOrEmpty(value))
                value = "null";
            return value;
        }

        public static string ConvertNullRandomizeStrToEmptyStr(this string value)
        {
            if (string.IsNullOrEmpty(value))
                value = "Not Randomized";
            return value;
        }

        public static string ConvertNullPatientStrToEmptyStr(this string value)
        {
            if (string.IsNullOrEmpty(value) || value == "-")
                value = "No Data";
            return value;
        }

        public static string ConvertTRCharToENChar(this string text)
        {
            var t = string.Join("", text.Normalize(NormalizationForm.FormD).Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
            t = t.Replace("ı", "i");
            t = t.Replace("İ", "I");
            return t;
        }

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequireDigit = false,
                RequiredLength = 6,
                RequireNonAlphanumeric = false,
                RequireUppercase = false,
                RequireLowercase = false,
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length - 1)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        public static string GetContentType(string contentType)
        {
            var types = GetMimeTypes();
            return types.FirstOrDefault(p => p.Value == contentType).Key;
        }

        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        public static Color GetRandomColor()
        {
            Random rnd = new Random();
            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            return randomColor;
        }

        public static List<Color> GetColorList()
        {
            var colors = new List<Color>()
            {
               Color.MediumAquamarine,
               Color.LightBlue,
               Color.LightGoldenrodYellow,
               Color.AntiqueWhite,
               Color.LightGray,
               Color.Azure,
               Color.LightGreen,
               Color.LightCyan,
               Color.LightPink,
               Color.LightYellow,
               Color.LightSkyBlue,
            };
            return colors;
        }

        public static string TurkishCharacterReplace(string shortnameText)
        {
            if (string.IsNullOrEmpty(shortnameText))
                shortnameText = string.Empty;
            shortnameText = shortnameText.Trim();
            shortnameText = shortnameText.ToLower();
            shortnameText = shortnameText.Replace("ş", "s");
            shortnameText = shortnameText.Replace("Ş", "S");
            shortnameText = shortnameText.Replace("İ", "I");
            shortnameText = shortnameText.Replace("ı", "i");
            shortnameText = shortnameText.Replace("ö", "o");
            shortnameText = shortnameText.Replace("Ö", "O");
            shortnameText = shortnameText.Replace("ü", "u");
            shortnameText = shortnameText.Replace("Ü", "U");
            shortnameText = shortnameText.Replace("Ç", "C");
            shortnameText = shortnameText.Replace("ç", "c");
            shortnameText = shortnameText.Replace("ğ", "g");
            shortnameText = shortnameText.Replace("Ğ", "G");
            shortnameText = shortnameText.Replace(" ", "-");
            shortnameText = shortnameText.Trim();
            return shortnameText;
        }

        public static string GetTinyUrl(string strURL)
        {
            string URL;
            URL = "http://tinyurl.com/api-create.php?url=" + strURL.ToLower();

            System.Net.HttpWebRequest objWebRequest;
            System.Net.HttpWebResponse objWebResponse;

            System.IO.StreamReader srReader;

            string strHTML;

            objWebRequest = (System.Net.HttpWebRequest)System.Net
               .WebRequest.Create(URL);
            objWebRequest.Method = "GET";

            objWebResponse = (System.Net.HttpWebResponse)objWebRequest
               .GetResponse();
            srReader = new System.IO.StreamReader(objWebResponse
               .GetResponseStream());

            strHTML = srReader.ReadToEnd();

            srReader.Close();
            objWebResponse.Close();
            objWebRequest.Abort();

            return (strHTML);

        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                //return Convert.ToHexString(hashBytes); // .NET 5 +

                // Convert the byte array to hexadecimal string prior to .NET 5
                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static int ParseOrder(string order)
        {
            return int.Parse(order.Replace("-", ""));
        }
    }
}
