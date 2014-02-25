using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides a simple counter that increments the value. </summary>
    public class IncrementNumberRule : SubstituteRule
    {
        public override string GetName()
        {
            return "IncrementNumber";
        }


        public override string[] GetDefaultNameAliases()
        {
            return new string[] { "IncrementInteger" };
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, ExecuteArg arg)
        {
            substituteValue = null;

            int count = 0;
            if (!string.IsNullOrEmpty(arg.OriginalValue))
            {
                if (!int.TryParse(arg.OriginalValue, out count))
                {
                    Trace.WriteLine(string.Format("\"{0}\" is not a valid number", arg.OriginalValue, GetType().FullName));
                    return false;
                }
            }

            ++count;
            substituteValue = count.ToString();
            return true;
        }
    }
}
