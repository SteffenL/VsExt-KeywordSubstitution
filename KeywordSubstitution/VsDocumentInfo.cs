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
    ///     Information about a VS document. Pretty much the same data you get when calling
    ///     IVsRunningDocumentTable.GetDocumentInfo().
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class VsDocumentInfo
    {
        public uint docCookie;
        public uint pgrfRDTFlags;
        public uint pdwReadLocks;
        public uint pdwEditLocks;
        public string pbstrMkDocument;
        public IVsHierarchy ppHier;
        public uint pitemid;
        public IntPtr ppunkDocData;

        public bool IsSuccess { get; private set; }

        public VsDocumentInfo(uint docCookie, IVsRunningDocumentTable vsRunningDocumentTable)
        {
            IsSuccess = false;
            this.docCookie = docCookie;
            if (vsRunningDocumentTable != null)
            {
                IsSuccess = (vsRunningDocumentTable.GetDocumentInfo(
                    docCookie,
                    out pgrfRDTFlags,
                    out pdwReadLocks,
                    out pdwEditLocks,
                    out pbstrMkDocument,
                    out ppHier,
                    out pitemid,
                    out ppunkDocData) == VSConstants.S_OK);
            }
        }

        ~VsDocumentInfo()
        {
            if (ppunkDocData != IntPtr.Zero)
            {
                Marshal.Release(ppunkDocData);
            }
        }
    }
}
