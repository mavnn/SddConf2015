using System.Text.RegularExpressions;
using NUnit.Framework;
using FsCheck;

namespace PhoneNumbers
{
    public class ValidE164
    {
        private static Regex reg;

        static ValidE164()
        {
            reg = new Regex(@"\+[0-9]+", RegexOptions.Compiled);
        }

        public readonly string Value;

        private ValidE164(string value)
        {
            this.Value = value;
        }

        public static bool TryCreate(string phoneNumber, out ValidE164 verified)
        {
            if (reg.IsMatch(phoneNumber))
            {
                verified = new ValidE164(phoneNumber);
                return true;
            }
            else
            {
                verified = null;
                return false;
            }
        }
    }
}

namespace PhoneNumbers.Test
{
    public class ValidE164Properties
    {
        public static bool DoesNotThrow(string pn)
        {
            ValidE164 v;
            ValidE164.TryCreate(pn, out v);
            return true;
        }
    }

    public class ValidE164Tests
    {
        [Test]
        public static void test_TryCreate()
        {
            Check.QuickThrowOnFailureAll<ValidE164Properties>();
        }
    }
}
