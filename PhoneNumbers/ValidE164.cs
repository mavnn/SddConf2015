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
    public class ValidTestNumber
    {
        public string Input;

        public ValidTestNumber(string input)
        {
            Input = input;
        }

        public override string ToString()
        {
            return string.Format("< {0} >", this.Input);
        }
    }

    public class InvalidTestNumber
    {
        public string Input;

        public InvalidTestNumber(string input)
        {
            Input = input;
        }

        public override string ToString()
        {
            return string.Format("< {0} >", this.Input);
        }
    }

    public class CustomArbs
    {
        public static Arbitrary<ValidTestNumber> ValidTestNumber()
        {
            var generator =
                Gen.Three(Gen.Choose(0,99999))
                    .Select(i => "+" + i.Item1.ToString() + i.Item2.ToString() + i.Item3.ToString())
                    .Select(pn => new ValidTestNumber(pn));
            return Arb.From(generator);
        }

        public static Arbitrary<InvalidTestNumber> InvalidTestNumber()
        {
            var tooLong =
                Gen.Choose(16, 50)
                .SelectMany(i => Gen.ArrayOf(i, Gen.Choose(0, 9)))
                .Select(arr => new InvalidTestNumber("+" + string.Concat(arr)));
            int a;
            var notNumeric =
                from valid in Arb.Generate<ValidTestNumber>()
                from c in Arb.Generate<char>()
                where !System.Int32.TryParse(c.ToString(), out a)
                select new InvalidTestNumber(valid.Input + c.ToString());
            return Arb.From(Gen.OneOf(tooLong, notNumeric));
        }
    }

    public class ValidE164Properties
    {
        public static bool DoesNotThrow(NonNull<string> pn)
        {
            ValidE164 v;
            ValidE164.TryCreate(pn.Get, out v);
            return true;
        }

        public static Property ValidNumbersAreValid(ValidTestNumber vtn)
        {
            ValidE164 v;
            return ValidE164.TryCreate(vtn.Input, out v).Label(vtn.Input);
        }

        public static Property InvalidNumbersAreInvalid(InvalidTestNumber itn)
        {
            ValidE164 v;
            return
                (!ValidE164.TryCreate(itn.Input, out v)).Label(itn.Input);
        }
    }

    public class ValidE164Tests
    {
        [Test]
        public static void test_TryCreate()
        {
            Arb.Register<CustomArbs>();
            Check.QuickThrowOnFailureAll<ValidE164Properties>();
        }
    }
}
