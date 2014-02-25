using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides the logged-on Windows user's name. </summary>
    public class UserNameRule : SubstituteRule
    {
        public override string GetName()
        {
            return "UserName";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            // Ref.: http://stackoverflow.com/a/1240379
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            substituteValue = userName;
            return true;
        }
    }
}
