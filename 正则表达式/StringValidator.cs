using System;
using System.Windows.Forms;

namespace Core
{
    public class StringValidator
    {
         public static bool CheckNumber(string strCheck)
        {
            if (!RegexHelper.IsValidNumber(strCheck))
            {
                MessageBox.Show("仅可输入整数");
                return false;
            }
            return true;
        }

         public static bool CheckUnsignedNumber(string strCheck)
         {
             if (!RegexHelper.IsNumberUnsignedinteger(strCheck))
             {
                 MessageBox.Show("仅可输入正整数");
                 return false;
             }
             return true;
         }
    }
}
