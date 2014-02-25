using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

namespace KeywordSubstitution.SubstituteRules
{
    public class SubstituteRuleManager
    {
        /// <summary>   List of rules that are currently registered. </summary>
        private List<SubstituteRule> _rules = new List<SubstituteRule>();
        /// <summary>   Fast-access cache of the names of registered rules, to avoid calling each rule for their name again. </summary>
        private HashSet<string> _ruleNameCache = new HashSet<string>();
        /// <summary>   Rule name aliases mapped to real names. </summary>
        private Dictionary<string, string> _ruleNameAliases = new Dictionary<string, string>();

        public SubstituteRuleManager()
        {
            registerRules();
        }

        /// <summary>   Registers all of the rules that are to be managed. </summary>
        private void registerRules()
        {
            // Find all classes in a namespace
            string @namespace = "KeywordSubstitution.SubstituteRules.Detail";
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == @namespace && t.BaseType == typeof(SubstituteRule)
                    select t;
            q.ToList().ForEach(t =>
            {
                Trace.WriteLine(string.Format("Registering rule: {0}...", t.Name), GetType().FullName);
                var rule = (SubstituteRule)Activator.CreateInstance(t.UnderlyingSystemType);
                rule.Register(this);
            });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Normalize rule name. You always get the real name, regardless of whether you provide the
        ///     real name or an alias.
        /// </summary>
        ///
        /// <exception cref="Exception">
        ///     Thrown when an alias could not be resolved to a real name, because of a mis-spelling or
        ///     the rule was not registered.
        /// </exception>
        ///
        /// <param name="nameOrAlias">  The name or alias. </param>
        ///
        /// <returns>   The normalized/real rule name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string NormalizeRuleName(string nameOrAlias)
        {
            if (_ruleNameCache.Contains(nameOrAlias))
            {
                return nameOrAlias;
            }

            string resolvedName;
            if (!_ruleNameAliases.TryGetValue(nameOrAlias, out resolvedName))
            {
                throw new Exception(string.Format("Unable to resolve rule name: {0}", nameOrAlias));
            }
            
            return resolvedName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Registers a rule name alias. </summary>
        ///
        /// <param name="name">     Real name of the rule. </param>
        /// <param name="alias">    The alias. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RegisterRuleNameAlias(string name, string alias)
        {
            _ruleNameAliases[alias] = name;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Registers a rule to be managed. </summary>
        ///
        /// <param name="rule"> The rule. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RegisterRule(SubstituteRule rule)
        {
            string ruleName = rule.GetName();
            // Add name to cache
            _ruleNameCache.Add(ruleName);
            // Register default name aliases
            foreach (var alias in rule.GetDefaultNameAliases())
            {
                RegisterRuleNameAlias(ruleName, alias);
            }

            _rules.Add(rule);
        }

        /// <summary>   Information about a match during keyword substitution. </summary>
        private class MatchInfo
        {
            public int MatchIndex;
            public int MatchValueLength;
            public string WholeSubstitute;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs keyword substitution on text for a rule. </summary>
        ///
        /// <param name="output">
        ///     [out] The output after substitution was done on the input.
        /// </param>
        /// <param name="matchInfoArray">   [out] Array of match information. </param>
        /// <param name="rule">             The rule. </param>
        /// <param name="ruleNameOrAlias">  The rule name or alias. </param>
        /// <param name="input">            The input. </param>
        /// <param name="dataProvider">     The data provider. </param>
        ///
        /// <returns>   true if any substitution was done, false if none. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool performSubstitution(out string output, out MatchInfo[] matchInfoArray, SubstituteRule rule, string ruleNameOrAlias, string input, SubstituteRuleDataProvider dataProvider)
        {
            const string TokenSeparator = ":";
            const string BlanksAfterTokenSeparator = " ";
            
            bool couldReplace = false;
            var internalMatchInfoList = new List<MatchInfo>();
            var rx = new Regex(string.Format(@"(\$)({0})(\s*{1}\s*)([^$]*?)(\s*\$)", ruleNameOrAlias, TokenSeparator), RegexOptions.Multiline);
            output = rx.Replace(
                input,
                delegate(Match match)
                {
                    string matchedName = match.Groups[2].Value;
                    string tokenSeparatorWithBlanks = string.Format("{0}{1}", match.Groups[3].Value.Trim(), BlanksAfterTokenSeparator);
                    string matchedValue = match.Groups[4].Value;

                    var executeArg = new SubstituteRule.ExecuteArg()
                    {
                        RuleNameOrAlias = matchedName,
                        OriginalValue = matchedValue,
                        DataProvider = dataProvider
                    };

                    string substituteValue;
                    if (!rule.Execute(out substituteValue, executeArg))
                    {
                        // Return original matched string
                        return match.Value;
                    }

                    var wholeSubstituteBuilder = new StringBuilder();
                    wholeSubstituteBuilder.Append(match.Groups[1].Value);
                    wholeSubstituteBuilder.Append(matchedName);
                    wholeSubstituteBuilder.Append(tokenSeparatorWithBlanks);
                    wholeSubstituteBuilder.Append(substituteValue);
                    wholeSubstituteBuilder.Append(match.Groups[5].Value);

                    string wholeSubstitute = wholeSubstituteBuilder.ToString();

                    var matchInfo = new MatchInfo()
                    {
                        MatchIndex = match.Index,
                        MatchValueLength = match.Value.Length,
                        WholeSubstitute = wholeSubstitute
                    };

                    internalMatchInfoList.Add(matchInfo);
                    couldReplace = true;
                    return wholeSubstitute;
                }
            );

            matchInfoArray = internalMatchInfoList.ToArray();
            return couldReplace;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs keyword substitution on a VS ITextEdit for a rule. </summary>
        ///
        /// <param name="rule">             The rule. </param>
        /// <param name="ruleNameOrAlias">  The rule name or alias. </param>
        /// <param name="textEdit">         The text edit. </param>
        ///
        /// <returns>   true if any substitution was done, false if none. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void performSubstitution(SubstituteRule rule, string ruleNameOrAlias, SubstituteRuleDataProvider dataProvider, ITextEdit textEdit)
        {
            string output;
            MatchInfo[] matchInfoArray;
            performSubstitution(out output, out matchInfoArray, rule, ruleNameOrAlias, textEdit.Snapshot.GetText(), dataProvider);

            // Replace in editor
            matchInfoArray.ToList().ForEach(mi => textEdit.Replace(mi.MatchIndex, mi.MatchValueLength, mi.WholeSubstitute));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes all of the managed rules on a VS ITextEdit. </summary>
        ///
        /// <param name="dataProvider"> The data provider. </param>
        /// <param name="textEdit">     The text edit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ExecuteRules(SubstituteRuleDataProvider dataProvider, ITextEdit textEdit)
        {
            foreach (var rule in _rules)
            {
                string ruleName = rule.GetName();
                // Execute once for the rule name
                performSubstitution(rule, ruleName, dataProvider, textEdit);
                // ... and once per alias
                foreach (var alias in _ruleNameAliases.Where(kvp => kvp.Value == ruleName).Select(kvp => kvp.Key))
                {
                    performSubstitution(rule, alias, dataProvider, textEdit);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes all of the managed rules on text. </summary>
        ///
        /// <param name="output">       [out] The output after substitution was done on the input. </param>
        /// <param name="input">        The input. </param>
        /// <param name="dataProvider"> The data provider. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ExecuteRules(out string output, string input, SubstituteRuleDataProvider dataProvider)
        {
            string buf = input;

            foreach (var rule in _rules)
            {
                string ruleName = rule.GetName();
                MatchInfo[] matchInfoArray;
                // Execute once for the rule name
                performSubstitution(out buf, out matchInfoArray, rule, ruleName, buf, dataProvider);
                // ... and once per alias
                foreach (var alias in _ruleNameAliases.Where(kvp => kvp.Value == ruleName).Select(kvp => kvp.Key))
                {
                    performSubstitution(out buf, out matchInfoArray, rule, alias, buf, dataProvider);
                }
            }

            output = buf;
        }
    }
}
