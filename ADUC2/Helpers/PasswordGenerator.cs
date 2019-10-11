using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ADUC2.Helpers
{
    public class PasswordGenerator
    {
        const int MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS = 2;
        const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
        const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMERIC_CHARACTERS = "0123456789";
        const string SPECIAL_CHARACTERS = @"!#$%&*@\";
        const int PASSWORD_LENGTH_MIN = 8;
        const int PASSWORD_LENGTH_MAX = 128;

        private readonly int length;
        private readonly string characterBank;
        private readonly bool includeLowercase;
        private readonly bool includeUppercase;
        private readonly bool includeNumeric;
        private readonly bool includeSpecial;

        public PasswordGenerator(int length, bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial)
        {
            if (length < PASSWORD_LENGTH_MIN || length > PASSWORD_LENGTH_MAX)
                throw new ArgumentOutOfRangeException("length", $"Password length must be between {PASSWORD_LENGTH_MIN} and {PASSWORD_LENGTH_MAX}.");

            if (!(includeLowercase || includeUppercase || includeNumeric || includeSpecial))
                throw new ArgumentException("The password must contain some kind of characters.");

            this.length = length;
            this.includeLowercase = includeLowercase;
            this.includeUppercase = includeUppercase;
            this.includeNumeric = includeNumeric;
            this.includeSpecial = includeSpecial;

            StringBuilder characters = new StringBuilder();

            if (includeLowercase)
                characters.Append(LOWERCASE_CHARACTERS);

            if (includeUppercase)
                characters.Append(UPPERCASE_CHARACTERS);

            if (includeNumeric)
                characters.Append(NUMERIC_CHARACTERS);

            if (includeSpecial)
                characters.Append(SPECIAL_CHARACTERS);

            characterBank = characters.ToString();
        }

        public string GeneratePassword()
        {
            ICollection<(int order, char character)> password = new List<(int, char)>(length);

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] byteArray = new byte[5];

                Func<string, (int order, char character)> getRandomChar = new Func<string, (int, char)>((characters) =>
                {
                    rng.GetBytes(byteArray);
                    return (BitConverter.ToInt32(byteArray, 1), characters[byteArray[0] % characters.Length]);
                });

                if (includeLowercase)
                    password.Add(getRandomChar(LOWERCASE_CHARACTERS));

                if (includeUppercase)
                    password.Add(getRandomChar(UPPERCASE_CHARACTERS));

                if (includeNumeric)
                    password.Add(getRandomChar(NUMERIC_CHARACTERS));

                if (includeSpecial)
                    password.Add(getRandomChar(SPECIAL_CHARACTERS));

                while (password.Count < length)
                {
                    var chr = getRandomChar(characterBank);

                    if (password.Count(x => x.character == chr.character) < MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS)
                        password.Add(chr);
                }
            }

            return String.Concat(password.OrderBy(x => x.order).Select(x => x.character));
        }
    }
}