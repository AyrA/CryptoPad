using System;
using System.Linq;

namespace CryptoPad
{
    public static class Tools
    {
        public static T[] FlagsToArray<T>(T Values) where T : struct
        {
            var VarType = typeof(T);
            var Val = IntVal(Values);
            if (VarType.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
            {
                throw new Exception($"{VarType.Name} is not an enumeration with the Flags attribute");
            }
            return Enum.GetValues(VarType).OfType<T>().Where(m => (IntVal(m) & Val) != 0).ToArray();
        }

        private static int IntVal(object someEnum)
        {
            if (someEnum == null)
            {
                throw new ArgumentNullException(nameof(someEnum));
            }
            try
            {
                return (int)(long)someEnum;
            }
            catch { }
            try
            {
                return (int)someEnum;
            }
            catch { }
            try
            {
                return (short)someEnum;
            }
            catch { }
            try
            {
                return (int)(ulong)someEnum;
            }
            catch { }
            try
            {
                return (int)(uint)someEnum;
            }
            catch { }
            try
            {
                return (ushort)someEnum;
            }
            catch { }
            try
            {
                return (byte)someEnum;
            }
            catch { }
            throw new ArgumentException("Value can't be converted to an integer");
        }
    }
}
