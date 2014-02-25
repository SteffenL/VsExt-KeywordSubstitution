using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides the name of the machine (computer name). </summary>
    public class MachineNameRule : SubstituteRule
    {
        public override string GetName()
        {
            return "MachineName";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            substituteValue = Environment.MachineName;
            return true;
        }
    }
}
