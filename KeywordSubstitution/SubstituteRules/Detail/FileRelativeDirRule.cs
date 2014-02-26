using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace KeywordSubstitution.SubstituteRules.Detail
{
    /// <summary>   A rule that provides the file's directory path relative to the project's directory path. </summary>
    public class FileRelativeDirRule : SubstituteRule
    {
        public override string GetName()
        {
            return "FileRelativeDir";
        }

        public override void Register(SubstituteRuleManager manager)
        {
            manager.RegisterRule(this);
        }

        public override bool Execute(out string substituteValue, SubstituteRule.ExecuteArg arg)
        {
            string projectDir = Path.GetDirectoryName(arg.DataProvider.Project.FullName);
            var projectDirUri = new Uri(projectDir + Path.DirectorySeparatorChar);
            var fileDirUri = new Uri(Path.GetDirectoryName(arg.DataProvider.DocumentInfo.pbstrMkDocument) + Path.DirectorySeparatorChar);
            var relativeUri = "./" + projectDirUri.MakeRelativeUri(fileDirUri);

            substituteValue = relativeUri.ToString();
            return true;
        }
    }
}
