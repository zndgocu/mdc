using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.doosan.fms.commonLib.Extension
{
    public static class RegexExtension
    {

        /// <summary>
        /// 쿼리의 문단이 변경될때 개행이 포함되어 있어야 합니다
        /// ex
        /// insert into (a, b) \r\n <- 필수
        /// values ('a', 'b') \r\n 
        /// where ( a = 'a') <- 마지막 문단의 경우 필수가 아님
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> GetInsertRegex(string str)
        {
            //string pattern = @"[\(].*[\)]";
            string pattern = @"([\(].*[\)])+";
            MatchCollection matchList = Regex.Matches(str, pattern, RegexOptions.Multiline);
            var list = matchList.Cast<Match>().Select(match => match.Value).ToList();
            return list;
        }

        public enum Regex_Insert_Index
        {
            InsertInto = 0,
            Values = 1,
            Where = 2
        }
    }
}
