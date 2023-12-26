using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shablonizator
{
    public static partial class TemplateRegexes
    {
        // @{property}
        public static readonly Regex PropertyTemplateRegex = MyPropertyTemplateRegex();

        // @{if(condition)}
        public static readonly Regex IfTemplateRegex = MyIfTemplateRegex();

        // @then{action}
        public static readonly Regex ThenTemplateRegex = MyThenTemplateRegex();

        // @else{action}
        public static readonly Regex ElseTemplateRegex = MyElseTemplateRegex();

        // @{if(condition)}@then{action}@else{action}
        public static readonly Regex FullIfTemplateRegex = MyFullIfTemplateRegex();

        // @for(condition){action}
        public static readonly Regex FullForTemplateRegex = MyFullForTemplateRegex();

        // @for(condition)
        public static readonly Regex ForTemplateRegex = MyForTemplateRegex();

        // @{object.property}
        public static readonly Regex ForPropertyTemplate = MyForPropertyTemplate();

        // Dictionary for boolean
        public static readonly Dictionary<string, Func<IComparable?, IComparable?, bool>> BoolExpressionDictionary = new()
        {
            { ">", (x,y) => x?.CompareTo(y) > 0},
            {"<", (x,y) => x?.CompareTo(y) < 0},
            {"<=", (x,y) => x?.CompareTo(y) <= 0},
            {">=", (x,y) => x?.CompareTo(y) >= 0},
            {"==", (x,y) => x?.CompareTo(y) == 0}
        };

        [GeneratedRegex("@{\\w*}")]
        private static partial Regex MyPropertyTemplateRegex();
        [GeneratedRegex(@"@{if\(.*\)}")]
        private static partial Regex MyIfTemplateRegex();
        [GeneratedRegex("@then{.\\w*}")]
        private static partial Regex MyThenTemplateRegex();
        [GeneratedRegex("@else{\\w*}")]
        private static partial Regex MyElseTemplateRegex();
        [GeneratedRegex("@{if(.*)}")]
        private static partial Regex MyFullIfTemplateRegex();
        [GeneratedRegex("@for(.*){.*}")]
        private static partial Regex MyFullForTemplateRegex();
        [GeneratedRegex(@"@for\(.*\)")]
        private static partial Regex MyForTemplateRegex();
        [GeneratedRegex(@"@{\w*.\w*}")]
        private static partial Regex MyForPropertyTemplate();
    }
}
