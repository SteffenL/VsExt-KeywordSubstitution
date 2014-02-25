using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides the project's directory path. </summary>
    public class ProjectDirRule : SubstituteRule
    {
        public override string GetName()
        {
            return "ProjectDir";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            string projectDir = Path.GetDirectoryName(arg.DataProvider.Project.FullName);
            var projectDirUri = new Uri(projectDir + Path.DirectorySeparatorChar);

            substituteValue = Uri.UnescapeDataString(projectDirUri.AbsolutePath);
            return true;
        }
    }
}
