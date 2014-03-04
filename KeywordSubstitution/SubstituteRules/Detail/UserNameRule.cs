﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A rule that provides the domain of the person currently logged on Windows. If the user
    ///     is impersonated, the impersonated domain is used.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

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
            substituteValue = Environment.UserName;
            return true;
        }
    }
}
