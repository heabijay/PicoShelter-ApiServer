using System;

namespace PicoShelter_ApiServer.BLL.Bussiness_Logic
{
    public class NumberToCodeConventer
    {
        private readonly static char[] _symbols = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        public static string Convert(int number)
        {
            string code = string.Empty;

            do
            {
                code = _symbols[number % _symbols.Length] + code;
            }
            while ((number /= _symbols.Length) != 0);

            return code;
        }


        public static int ConvertBack(string code)
        {
            int number = 0;
            for (int i = 0; i < code.Length; i++)
            {
                number += Array.IndexOf(_symbols, code[i]) * (int)Math.Pow(_symbols.Length, (code.Length - 1) - i);
            }

            return number;
        }
    }
}
