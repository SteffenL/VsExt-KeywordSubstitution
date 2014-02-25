using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides a new GUID. </summary>
    public class GuidRule : SubstituteRule
    {
        public override string GetName()
        {
            return "Guid";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            var guid = Guid.NewGuid();
            substituteValue = guid.ToString();
            return true;
        }
    }
}
