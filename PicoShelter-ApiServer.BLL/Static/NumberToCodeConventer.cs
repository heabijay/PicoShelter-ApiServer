using System;

namespace PicoShelter_ApiServer.BLL.Bussiness_Logic
{
    public class NumberToCodeConventer
    {
        private static char[] Symbols = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        public static string Convert(int number)
        {
            string code = string.Empty;

            do
            {
                code = Symbols[number % Symbols.Length] + code;
            }
            while ((number /= Symbols.Length) != 0);

            return code;
        }


        public static int ConvertBack(string code)
        {
            int number = 0;
            for (int i = 0; i < code.Length; i++)
            {
                number += Array.IndexOf(Symbols, code[i]) * (int)Math.Pow(Symbols.Length, (code.Length - 1) - i);
            }

            return number;
        }
    }
}
