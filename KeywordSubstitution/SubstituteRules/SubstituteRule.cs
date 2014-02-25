using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace KeywordSubstitution.SubstituteRules
{
    /// <summary>   Substitute rule base class. All specialized rules should derive from this class. </summary>
    public abstract class SubstituteRule
    {
        /// <summary>   Execute argument. </summary>
        public class ExecuteArg
        {
            /// <summary>   The rule name or alias. </summary>
            public string RuleNameOrAlias;
            /// <summary>   The original value. </summary>
            public string OriginalValue;
            /// <summary>   The data provider. </summary>
            public SubstituteRuleDataProvider DataProvider;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the default name aliases. </summary>
        ///
        /// <returns>   The default name aliases. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual string[] GetDefaultNameAliases() { return new string[] {}; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the internal name of this rule. </summary>
        ///
        /// <returns>   The name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public abstract string GetName();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Registers this rule with the manager. </summary>
        ///
        /// <param name="manager">  The rule manager. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public abstract void Register(SubstituteRuleManager manager);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the rule, which performs substitution. </summary>
        ///
        /// <param name="substituteValue">  [out] The resulting substitute value. </param>
        /// <param name="arg">              Provides the data used during execution. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public abstract bool Execute(out string substituteValue, ExecuteArg arg);
    }


}
