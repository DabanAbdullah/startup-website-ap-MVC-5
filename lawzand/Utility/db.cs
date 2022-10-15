using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace lawzand.Utility
{
  

     

        public class db
        {
            public static string uid = "", role = "", username = "", pass = "";
            public static SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            public static string docmd(string sql, SqlParameter[] param)
            {
                string result = "";
                try
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                    result = "سەرکەوتوو بوو";
                    if (con.State == ConnectionState.Closed) con.Close();
                }
                catch (Exception ex)
                {
                    result = ex.Message.ToString();
                }
                return result;
            }

            public static string Encrypt(string clearText)
            {
                string EncryptionKey = "RESV2PRZHA99795";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }

            public static string Decrypt(string cipherText)
            {
                string EncryptionKey = "RESV2PRZHA99795";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }


            public static string lid = "", per = "", name = "";
            public static SqlConnection getcon()
            {
                return con;
            }


            static DataTable dt = new DataTable();
            public static string getmax(string tablename, string field)
            {
                dt.Clear();
                string max = "";
                SqlDataAdapter da = new SqlDataAdapter("select isnull(max(" + field + "),0)+1 as max from " + tablename + "", con);
                da.Fill(dt);
                max = dt.Rows[0]["max"].ToString();

                return max;
            }

            public static string senddb(string message, string to)
            {
                string result = "";
                try
                {











                    using (MailMessage mm = new MailMessage("csisuliinstitute@gmail.com", to))
                    {
                        mm.Subject = "password";
                        mm.Body = message;



                        mm.IsBodyHtml = false;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential("csisuliinstitute@gmail.com", "!QAZ2wsx#EDC");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);


                    }

                    result = "we have sent an email to " + to;


                }
                catch (Exception ex)
                {

                    result = ex.Message;
                }
                return result;
            }

        }



        public class PasswordGeneratorSettings
        {
            const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
            const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMERIC_CHARACTERS = "0123456789";
            const string SPECIAL_CHARACTERS = @"!#$%&*@\";
            const int PASSWORD_LENGTH_MIN = 8;
            const int PASSWORD_LENGTH_MAX = 128;

            public bool IncludeLowercase { get; set; }
            public bool IncludeUppercase { get; set; }
            public bool IncludeNumbers { get; set; }
            public bool IncludeSpecial { get; set; }
            public int PasswordLength { get; set; }
            public string CharacterSet { get; set; }
            public int MaximumAttempts { get; set; }

            public PasswordGeneratorSettings(bool includeLowercase, bool includeUppercase, bool includeNumbers, bool includeSpecial, int passwordLength)
            {
                IncludeLowercase = includeLowercase;
                IncludeUppercase = includeUppercase;
                IncludeNumbers = includeNumbers;
                IncludeSpecial = includeSpecial;
                PasswordLength = passwordLength;

                StringBuilder characterSet = new StringBuilder();

                if (includeLowercase)
                {
                    characterSet.Append(LOWERCASE_CHARACTERS);
                }

                if (includeUppercase)
                {
                    characterSet.Append(UPPERCASE_CHARACTERS);
                }

                if (includeNumbers)
                {
                    characterSet.Append(NUMERIC_CHARACTERS);
                }

                if (includeSpecial)
                {
                    characterSet.Append(SPECIAL_CHARACTERS);
                }

                CharacterSet = characterSet.ToString();
            }

            public bool IsValidLength()
            {
                return PasswordLength >= PASSWORD_LENGTH_MIN && PasswordLength <= PASSWORD_LENGTH_MAX;
            }

            public string LengthErrorMessage()
            {
                return string.Format("Password length must be between {0} and {1} characters", PASSWORD_LENGTH_MIN, PASSWORD_LENGTH_MAX);
            }
        }


        public static class PasswordGenerator
        {

            /// <summary>
            /// Generates a random password based on the rules passed in the settings parameter
            /// </summary>
            /// <param name="settings">Password generator settings object</param>
            /// <returns>Password or try again</returns>
            public static string GeneratePassword(PasswordGeneratorSettings settings)
            {
                const int MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS = 2;
                char[] password = new char[settings.PasswordLength];
                int characterSetLength = settings.CharacterSet.Length;

                System.Random random = new System.Random();
                for (int characterPosition = 0; characterPosition < settings.PasswordLength; characterPosition++)
                {
                    password[characterPosition] = settings.CharacterSet[random.Next(characterSetLength - 1)];

                    bool moreThanTwoIdenticalInARow =
                        characterPosition > MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS
                        && password[characterPosition] == password[characterPosition - 1]
                        && password[characterPosition - 1] == password[characterPosition - 2];

                    if (moreThanTwoIdenticalInARow)
                    {
                        characterPosition--;
                    }
                }

                return string.Join(null, password);
            }


            /// <summary>
            /// When you give it a password and some settings, it validates the password against the settings.
            /// </summary>
            /// <param name="settings">Password settings</param>
            /// <param name="password">Password to test</param>
            /// <returns>True or False to say if the password is valid or not</returns>
            public static bool PasswordIsValid(PasswordGeneratorSettings settings, string password)
            {
                const string REGEX_LOWERCASE = @"[a-z]";
                const string REGEX_UPPERCASE = @"[A-Z]";
                const string REGEX_NUMERIC = @"[\d]";
                const string REGEX_SPECIAL = @"([!#$%&*@\\])+";

                bool lowerCaseIsValid = !settings.IncludeLowercase || (settings.IncludeLowercase && Regex.IsMatch(password, REGEX_LOWERCASE));
                bool upperCaseIsValid = !settings.IncludeUppercase || (settings.IncludeUppercase && Regex.IsMatch(password, REGEX_UPPERCASE));
                bool numericIsValid = !settings.IncludeNumbers || (settings.IncludeNumbers && Regex.IsMatch(password, REGEX_NUMERIC));
                bool symbolsAreValid = !settings.IncludeSpecial || (settings.IncludeSpecial && Regex.IsMatch(password, REGEX_SPECIAL));

                return lowerCaseIsValid && upperCaseIsValid && numericIsValid && symbolsAreValid;
            }
        }



        static class NumberToText

        {



            public static string numtoarab(string num)
            {
                return num.Replace('0', '٠').Replace('1', '١').Replace('2', '٢').Replace('3', '٣').Replace('4', '٤').Replace('5', '٥').Replace('6', '٦').Replace('7', '٧').Replace('8', '٨').Replace('9', '٩');
            }

            public static string arabtoeng(string num)
            {
                return num.Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4').Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9');
            }

            private static string[] _ones =
            {
            "سفر",
            "یەک",
            "دوو",
            "سێ",
            "چوار",
            "پێنج",
            "شەش",
            "حەوت",
            "هەشت",
            "نۆ"
        };

            private static string[] _teens =
            {
            "دە",
            "یانزە",
            "دوانزە",
            "سیانزە",
            "جواردە",
            "پانزە",
            "شانزە",
            "حەڤدە",
            "هەژدە",
            "نۆزدە"
        };

            private static string[] _tens =
            {
            "",
            "دە",
            "بیست",
            "سی",
            "چل",
            "پەنجا",
            "شەست",
            "حەفتا",
            "هەشتا",
            "نەوە"
        };

            // US Nnumbering:
            private static string[] _thousands =
            {
            "",
            "هەزار",
            "ملێۆن",
            "بلێۆن",
            "تریلوێن",
            "کوادریلیۆن"
        };

            /// <summary>
            /// Converts a numeric value to words suitable for the portion of
            /// a check that writes out the amount.
            /// </summary>
            /// <param name="value">Value to be converted</param>
            /// <returns></returns>
            public static string Convert(decimal value)
            {
                string digits, temp;
                bool showThousands = false;
                bool allZeros = true;

                // Use StringBuilder to build result
                StringBuilder builder = new StringBuilder();
                // Convert integer portion of value to string
                digits = ((long)value).ToString();
                // Traverse characters in reverse order
                for (int i = digits.Length - 1; i >= 0; i--)
                {
                    int ndigit = (int)(digits[i] - '0');
                    int column = (digits.Length - (i + 1));

                    // Determine if ones, tens, or hundreds column
                    switch (column % 3)
                    {
                        case 0:        // Ones position
                            showThousands = true;
                            if (i == 0)
                            {
                                // First digit in number (last in loop)
                                temp = String.Format("{0} ", _ones[ndigit]);
                            }
                            else if (digits[i - 1] == '1')
                            {
                                // This digit is part of "teen" value
                                temp = String.Format("{0} ", _teens[ndigit]);
                                // Skip tens position
                                i--;
                            }
                            else if (ndigit != 0)
                            {
                                // Any non-zero digit
                                temp = String.Format("{0} ", _ones[ndigit]);
                            }
                            else
                            {
                                // This digit is zero. If digit in tens and hundreds
                                // column are also zero, don't show "thousands"
                                temp = String.Empty;
                                // Test for non-zero digit in this grouping
                                if (digits[i - 1] != '0' || (i > 1 && digits[i - 2] != '0'))
                                    showThousands = true;
                                else
                                    showThousands = false;
                            }

                            // Show "thousands" if non-zero in grouping
                            if (showThousands)
                            {
                                if (column > 0)
                                {
                                    temp = String.Format("{0}{1}{2}",
                                        temp,
                                        _thousands[column / 3],
                                        allZeros ? " " : " و ");
                                }
                                // Indicate non-zero digit encountered
                                allZeros = false;
                            }
                            builder.Insert(0, temp);
                            break;

                        case 1:        // Tens column
                            if (ndigit > 0)
                            {
                                temp = String.Format("{0}{1}",
                                    _tens[ndigit],
                                    (digits[i + 1] != '0') ? "و" : " ");
                                builder.Insert(0, temp);
                            }
                            break;

                        case 2:        // Hundreds column
                            if (ndigit > 0)
                            {

                                if (_ones[ndigit].ToString() != "یەک")
                                {
                                    temp = String.Format("{0} سەد و", _ones[ndigit]);
                                    builder.Insert(0, temp);
                                }
                                else
                                {
                                    temp = String.Format("{0} سەد و   ", "");
                                    builder.Insert(0, temp);
                                }
                            }
                            break;
                    }
                }

                // Append fractional portion/cents
                builder.AppendFormat("", (value - (long)value) * 100);

                // Capitalize first letter
                return String.Format("{0}{1}",
                    Char.ToUpper(builder[0]),
                    builder.ToString(1, builder.Length - 1)).TrimEnd('و');
            }
        }


    }

