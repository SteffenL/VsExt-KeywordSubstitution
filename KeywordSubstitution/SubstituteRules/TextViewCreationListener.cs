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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

namespace KeywordSubstitution.SubstituteRules
{
    /// <summary>   Listens for text view creation. </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class TextViewCreationListener : IWpfTextViewCreationListener
    {
        /// <summary>   VS service provider. </summary>
        private IServiceProvider _services;
        /// <summary>   RDT events. </summary>
        private VsRunningDocTableEventSink _myVsRunningDocTableEvents;

        /// <summary>   Open documents (in VS) that have a WPF text view. </summary>
        public static HashSet<IWpfTextView> OpenDocumentsWithWpfTextView = new HashSet<IWpfTextView>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="services"> VS service provider. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [ImportingConstructor]
        public TextViewCreationListener([Import(typeof(SVsServiceProvider))] IServiceProvider services)
        {
            _services = services;
            _myVsRunningDocTableEvents = new VsRunningDocTableEventSink(_services);

            var vsRunningDocumentTable = (IVsRunningDocumentTable)_services.GetService(typeof(SVsRunningDocumentTable));
            uint rdtEventsCookie;
            vsRunningDocumentTable.AdviseRunningDocTableEvents(_myVsRunningDocTableEvents, out rdtEventsCookie);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Called when a text view having matchine roles is created over a text data model having a
        ///     matching content type.
        /// </summary>
        ///
        /// <param name="textView"> The text view. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void TextViewCreated(IWpfTextView textView)
        {
            OpenDocumentsWithWpfTextView.Add(textView);

            textView.Closed += delegate(object sender, EventArgs e)
            {
                OpenDocumentsWithWpfTextView.Remove(textView);
            };
        }
    }
}
