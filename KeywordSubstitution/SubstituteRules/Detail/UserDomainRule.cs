using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A rule that provides the username of the person currently logged on Windows. If the user
    ///     is impersonated, the impersonated username is used.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class UserDomainRule : SubstituteRule
    {
        public override string GetName()
        {
            return "UserDomain";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            // NOTE: In a somewhat large network with a PDC, and the computer is not a member of a domain, GetCurrentDomain() may hang for seconds to minutes
            // As a workaround, I've attempted to check if the computer is a member of a domain before calling GetCurrentDomain()
            // TODO: Check if this is correct and reliable
            substituteValue = WindowsDomain.GetDomainName();
            return true;
        }
    }
}
