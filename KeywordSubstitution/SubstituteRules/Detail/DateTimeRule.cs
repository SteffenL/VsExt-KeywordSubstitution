using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides the current date and time with offset, in a universal format. </summary>
    public class DateTimeRule : SubstituteRule
    {
        public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";

        public override string GetName()
        {
            return "DateTime";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            substituteValue = DateTimeOffset.Now.ToString(DateTimeFormat);
            return true;
        }
    }
}
