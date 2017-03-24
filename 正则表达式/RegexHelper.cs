using System.Text.RegularExpressions;

namespace DotNet.Utilities
{
    /// <summary>
    /// 操作正则表达式的公共类
    /// </summary>    
    public class RegexHelper
    {
        #region 验证输入字符串是否与模式字符串匹配
        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">模式字符串</param>        
        public static bool IsMatch(string input, string pattern)
        {
            return IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="pattern">模式字符串</param>
        /// <param name="options">筛选条件</param>
        public static bool IsMatch(string input, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }
        #endregion

        #region 返回匹配结果
        /// <summary>
        /// 匹配到的多项
        /// </summary>
        /// <param name="input">待验证字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <param name="strStart">匹配项的起始标志</param>
        /// <param name="strEnd">匹配项的结束标志</param>
        /// <param name="result">匹配结果</param>
        public static void GetMatchList(string input,string pattern, string strStart,string strEnd, List<string> result)
        {
            if (result == null)
                result = new List<string>();
            
            if (!IsMatch(input, pattern)) return;

            int startIdx = 0, endIdx = 0;
            Regex regex = new Regex(pattern);
            MatchCollection matchList = regex.Matches(strHtml);
            foreach (Match item in matchList)
            {
                startIdx = item.Value.IndexOf(strStart);
                endIdx = item.Value.LastIndexOf(strEnd);
                result.Add(item.Value.Substring(startIdx + strStart.Length + 1, endIdx - startIdx - strStart.Length - 2));
            }
        }

        /// <summary>
        /// 匹配特定内容
        /// </summary>
        /// <param name="input">待验证字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <param name="strStart">匹配项的起始标志</param>
        /// <param name="strEnd">匹配项的结束标志</param>
        /// <returns>匹配结果</returns>
        public static string GetMatchItem(string input, string pattern, string strStart, string strEnd)
        {
            string result = string.Empty;

            if(!IsMatch(input,pattern)) return result;

            int startIdx = 0, endIdx = 0;
            Regex regex = new Regex(pattern);
            Match matchInfo = regex.Match(input);
            startIdx = matchInfo.Value.IndexOf(strStart);
            endIdx = matchInfo.Value.IndexOf(strEnd);

            result = matchInfo.Value.Substring(startIdx + strStart.Length + 1, endIdx - startIdx - strStart.Length - 2);

            return result;
        }

        #endregion

        #region 常用格式验证
        /// <summary>
        /// 中文字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsChineseWords(string input)
        {
            string strPattern = "^[\u4e00-\u9fa5]{0,}$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 网址URL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsURL(string input)
        {
            string strPattern = @"^http:\/\/([\w-]+(\.[\w-]+)+(\/[\w- .\/\?%&=\u4e00-\u9fa5]*)?)?$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 验证电子邮件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            string strPattern = @" \w+([-+.´]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 帐号是否合法(字母开头，允许5-16字节，允许字母数字下划线)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidName(string input)
        {
            string strPattern = @"^[a-zA-Z][a-zA-Z0-9_]{4,15}$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 中国的身份证为15位或18位，支持带X的  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIdentity(string input)
        {
            string strPattern = @"^[1-9]([0-9]{16}|[0-9]{13})[xX0-9]$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 验证手机号（包含159，不包含小灵通） 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsTelphone(string input)
        {
            string strPattern = @"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// ip地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIP(string input)
        {
            string strPattern = @" ^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 由26个英文字母组成的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEnglishAlphabet(string input)
        {
            string strPattern = @"^[A-Za-z]+$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 由26个英文字母的大写组成的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsCapitalLetter(string input)
        {
            string strPattern = @"^[A-Z]+$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 由数字和26个英文字母组成的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsWordsNumber(string input)
        {
            string strPattern = @"^[A-Za-z0-9]+$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 数字、26个英文字母或者下划线组成的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidInput(string input)
        {
            string strPattern = @"^\w+$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidNumber(string input)
        {
            string strPattern = @"^[0-9]*$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 匹配正整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumberUnsignedinteger(string input)
        {
            string strPattern = @"^[1-9]\d*$";
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 只能输入某个区间数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumberRange(string input, int min, int max)
        {
            string strPattern = string.Format(@"^[{0}-{1}]$", min, max);
            return IsMatch(input, strPattern);
        }

        /// <summary>
        /// 只能输入m到n个数字
        /// </summary>
        /// <param name="input"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsNumberLength(string input, int min, int max)
        {
            string strPattern = string.Format(@"\d{{0},{1}}$", min, max);
            return IsMatch(input, strPattern);
        }
        #endregion
    }
}
