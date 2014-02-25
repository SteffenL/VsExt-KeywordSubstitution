using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KeywordSubstitution
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Provides data that is and can be used during rule execution. Whether this is a good name
    ///     for this class can probably be discussed.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SubstituteRuleDataProvider
    {
        public IVsRunningDocumentTable VsRunningDocumentTable;
        public VsDocumentInfo DocumentInfo;
        public IVsTextView TextView;
        public IWpfTextView WpfTextView;
        public EnvDTE.Project Project;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A more dumb version of the data provider used during unit testing. Sadly, VS services are
    ///     not available, so not everything can be tested properly.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SubstituteRuleTestDataProvider : SubstituteRuleDataProvider
    {

        public SubstituteRuleTestDataProvider()
        {
            DocumentInfo = new VsDocumentInfo(0, null);
        }
    }
}
