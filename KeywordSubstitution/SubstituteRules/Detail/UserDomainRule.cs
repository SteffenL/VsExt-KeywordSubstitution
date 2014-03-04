using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.ActiveDirectory;

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
            string domain = null;
            try
            {
                var cd = Domain.GetCurrentDomain();
                domain = cd.ToString();
            }
            catch (ActiveDirectoryOperationException) { }

            substituteValue = domain;
            return true;
        }
    }
}
