using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides the full name of the file. </summary>
    public class FileNameRule : SubstituteRule
    {
        public override string GetName()
        {
            return "FileName";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            substituteValue = Path.GetFileName(arg.DataProvider.DocumentInfo.pbstrMkDocument);
            return true;
        }
    }
}
