using ArtHandler.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ArtHandler.Repository
{
    public class PasswordGenerator
    {
        public int MinimumLengthPassword { get; private set; }
        public int MaximumLengthPassword { get; private set; }
        public int MinimumLowerCaseChars { get; private set; }
        public int MinimumUpperCaseChars { get; private set; }
        public int MinimumNumericChars { get; private set; }
        public int MinimumSpecialChars { get; private set; }

        public static string AllLowerCaseChars { get; private set; }
        public static string AllUpperCaseChars { get; private set; }
        public static string AllNumericChars { get; private set; }
        public static string AllSpecialChars { get; private set; }
        private readonly string _allAvailableChars;

        private readonly RandomSecureVersion _randomSecure = new RandomSecureVersion();
        private int _minimumNumberOfChars;

        static PasswordGenerator()
        {
            string randomPasswordType = "";
            if (randomPasswordType == "SendOTP")
            {
                AllNumericChars = GetCharRange('0', '9');
            }
            else
            {
                // Define characters that are valid and reject ambiguous characters such as ilo, IO and 1 or 0
                AllLowerCaseChars = GetCharRange('a', 'z', exclusiveChars: "ilo");
                AllUpperCaseChars = GetCharRange('A', 'Z', exclusiveChars: "IO");
                AllNumericChars = GetCharRange('2', '9');
                //AllSpecialChars = "!@#%*()$?+-=";
                AllSpecialChars = "!@#*()$";
                //AllNumericChars = GetCharRange('0', '9');
            }
        }

        public PasswordGenerator([Optional] string randomPasswordType)
        {
            if (randomPasswordType == "SendOTP")
            {
                MinimumLengthPassword = Constants.OTPLength;
                MaximumLengthPassword = Constants.OTPLength;

                _allAvailableChars =
                    OnlyIfOneCharIsRequired(MinimumNumericChars, AllNumericChars);
            }
            else if (randomPasswordType == "EnrollmentLink")
            {
                MinimumUpperCaseChars = 0;
                MinimumLowerCaseChars = 7;
                _allAvailableChars =
                  OnlyIfOneCharIsRequired(MinimumLowerCaseChars, AllLowerCaseChars) +
                  OnlyIfOneCharIsRequired(MinimumUpperCaseChars, AllUpperCaseChars);
            }
            else
            {
                MinimumLengthPassword = SingletonPasswordSettings.Instance.PasswordSettings.MinLength;//Convert.ToInt32(dt.Rows[0]["MinLength"]);
                MaximumLengthPassword = SingletonPasswordSettings.Instance.PasswordSettings.MaxLength;
                string tempspl = Convert.ToString(SingletonPasswordSettings.Instance.PasswordSettings.AllowedSplChars).Trim();
                //if (!string.IsNullOrEmpty(tempspl))
                //{
                //    AllSpecialChars = tempspl;
                //}
                //else
                //{
                AllSpecialChars = "!@#*()$";
                //}

                MinimumUpperCaseChars = SingletonPasswordSettings.Instance.PasswordSettings.CapsLength;
                MinimumLowerCaseChars = SingletonPasswordSettings.Instance.PasswordSettings.SmallLength;
                MinimumNumericChars = SingletonPasswordSettings.Instance.PasswordSettings.NumericLength;
                MinimumSpecialChars = SingletonPasswordSettings.Instance.PasswordSettings.SplCharsLength;

                _allAvailableChars =
                    OnlyIfOneCharIsRequired(MinimumLowerCaseChars, AllLowerCaseChars) +
                    OnlyIfOneCharIsRequired(MinimumUpperCaseChars, AllUpperCaseChars) +
                    OnlyIfOneCharIsRequired(MinimumNumericChars, AllNumericChars) +
                    OnlyIfOneCharIsRequired(MinimumSpecialChars, AllSpecialChars);
            }
        }

        private string OnlyIfOneCharIsRequired(int minimum, string allChars)
        {
            return minimum > 0 || _minimumNumberOfChars == 0 ? allChars : string.Empty;
        }

        public string Generate([Optional] string randomPasswordType)
        {
            var lengthOfPassword = _randomSecure.Next(MinimumLengthPassword, MaximumLengthPassword);
            var unshuffeledResult = "";
            // Get the required number of characters of each catagory and 
            // add random charactes of all catagories
            if (randomPasswordType == "SendOTP")
            {
                var minimumChars = GetRandomString(AllNumericChars, MinimumNumericChars);
                var rest = GetRandomString(_allAvailableChars, lengthOfPassword - minimumChars.Length);
                unshuffeledResult = minimumChars + rest;
            }
            else if (randomPasswordType == "EnrollmentLink")
            {
                var minimumChars = GetRandomString(AllLowerCaseChars, MinimumLowerCaseChars) +
                             GetRandomString(AllUpperCaseChars, MinimumUpperCaseChars);

                var rest = GetRandomString(_allAvailableChars, lengthOfPassword - minimumChars.Length);
                unshuffeledResult = minimumChars + rest;
            }
            else
            {
                var minimumChars = GetRandomString(AllLowerCaseChars, MinimumLowerCaseChars) +
                                GetRandomString(AllUpperCaseChars, MinimumUpperCaseChars) +
                                GetRandomString(AllNumericChars, MinimumNumericChars) +
                                GetRandomString(AllSpecialChars, MinimumSpecialChars);
                var rest = GetRandomString(_allAvailableChars, lengthOfPassword - minimumChars.Length);
                unshuffeledResult = minimumChars + rest;
            }
            // Shuffle the result so the order of the characters are unpredictable
            var result = unshuffeledResult.ShuffleTextSecure();
            return result;
        }

        private string GetRandomString(string possibleChars, int lenght)
        {
            var result = string.Empty;
            for (var position = 0; position < lenght; position++)
            {
                var index = _randomSecure.Next(possibleChars.Length);
                result += possibleChars[index];
            }
            return result;
        }

        private static string GetCharRange(char minimum, char maximum, string exclusiveChars = "")
        {
            var result = string.Empty;
            for (char value = minimum; value <= maximum; value++)
            {
                result += value;
            }
            if (!string.IsNullOrEmpty(exclusiveChars))
            {
                var inclusiveChars = result.Except(exclusiveChars).ToArray();
                result = new string(inclusiveChars);
            }
            return result;
        }
    }

    internal static class Extensions
    {
        private static readonly Lazy<RandomSecureVersion> RandomSecure =
            new Lazy<RandomSecureVersion>(() => new RandomSecureVersion());
        public static IEnumerable<T> ShuffleSecure<T>(this IEnumerable<T> source)
        {
            var sourceArray = source.ToArray();
            for (int counter = 0; counter < sourceArray.Length; counter++)
            {
                int randomIndex = RandomSecure.Value.Next(counter, sourceArray.Length);
                yield return sourceArray[randomIndex];

                sourceArray[randomIndex] = sourceArray[counter];
            }
        }

        public static string ShuffleTextSecure(this string source)
        {
            var shuffeldChars = source.ShuffleSecure().ToArray();
            return new string(shuffeldChars);
        }
    }

    internal class RandomSecureVersion
    {
        //Never ever ever never use Random() in the generation of anything that requires true security/randomness
        //and high entropy or I will hunt you down with a pitchfork!! Only RNGCryptoServiceProvider() is safe.
        private readonly RNGCryptoServiceProvider _rngProvider = new RNGCryptoServiceProvider();

        public int Next()
        {
            var randomBuffer = new byte[4];
            _rngProvider.GetBytes(randomBuffer);
            var result = BitConverter.ToInt32(randomBuffer, 0);
            return result;
        }

        public int Next(int maximumValue)
        {
            // Do not use Next() % maximumValue because the distribution is not OK
            return Next(0, maximumValue);
        }

        public int Next(int minimumValue, int maximumValue)
        {
            var seed = Next();

            //  Generate uniformly distributed random integers within a given range.
            return new Random(seed).Next(minimumValue, maximumValue);
        }
    }
}