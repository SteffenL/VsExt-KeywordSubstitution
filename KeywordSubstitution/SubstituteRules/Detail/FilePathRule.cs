using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides the file's path. </summary>
    public class FilePathRule : SubstituteRule
    {
        public override string GetName()
        {
            return "FilePath";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            substituteValue = Uri.UnescapeDataString((new Uri(arg.DataProvider.DocumentInfo.pbstrMkDocument)).AbsolutePath);
            return true;
        }
    }
}
