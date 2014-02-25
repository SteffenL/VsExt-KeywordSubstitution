using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides the file's path relative to the project's directory path. </summary>
    public class FileRelativePathRule : SubstituteRule
    {
        public override string GetName()
        {
            return "FileRelativePath";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            string projectDir = Path.GetDirectoryName(arg.DataProvider.Project.FullName);
            var projectUri = new Uri(projectDir + Path.DirectorySeparatorChar);
            var fileUri = new Uri(arg.DataProvider.DocumentInfo.pbstrMkDocument);
            var relativeUri = projectUri.MakeRelativeUri(fileUri);

            substituteValue = relativeUri.ToString();
            return true;
        }
    }
}
