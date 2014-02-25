using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;

namespace KeywordSubstitution
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Various methods that increase convenience when dealing with anything for VS.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class VsHelper
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Returns an IVsTextView for the given file path, if the given file is open in Visual
        ///     Studio.
        /// </summary>
        ///
        /// <remarks>   Ref.: http://stackoverflow.com/a/2427368. </remarks>
        ///
        /// <param name="filePath"> Full Path of the file you are looking for. </param>
        ///
        /// <returns>   The IVsTextView for this file, if it is open, null otherwise. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Microsoft.VisualStudio.TextManager.Interop.IVsTextView GetIVsTextView(string filePath)
        {
            var dte2 = (EnvDTE80.DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte2;
            Microsoft.VisualStudio.Shell.ServiceProvider serviceProvider = new Microsoft.VisualStudio.Shell.ServiceProvider(sp);

            Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy uiHierarchy;
            uint itemID;
            Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame windowFrame;
            //Microsoft.VisualStudio.Text.Editor.IWpfTextView wpfTextView = null;
            if (Microsoft.VisualStudio.Shell.VsShellUtilities.IsDocumentOpen(serviceProvider, filePath, Guid.Empty,
                                            out uiHierarchy, out itemID, out windowFrame))
            {
                // Get the IVsTextView from the windowFrame.
                return Microsoft.VisualStudio.Shell.VsShellUtilities.GetTextView(windowFrame);
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Returns an IWpfTextView for the given IVsTextView. </summary>
        ///
        /// <remarks>   Ref.: http://stackoverflow.com/a/6603233. </remarks>
        ///
        /// <param name="vTextView">    The text view. </param>
        ///
        /// <returns>   The WPF text view. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static IWpfTextView GetWpfTextView(IVsTextView vTextView)
        {
            IWpfTextView view = null;
            IVsUserData userData = vTextView as IVsUserData;

            if (null != userData)
            {
                IWpfTextViewHost viewHost;
                object holder;
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                viewHost = (IWpfTextViewHost)holder;
                view = viewHost.TextView;
            }

            return view;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the project in the IVsHierarchy. </summary>
        ///
        /// <param name="hier"> The hier. </param>
        ///
        /// <returns>   The project. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static EnvDTE.Project GetProject(IVsHierarchy hier)
        {
            object project;
            hier.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out project);
            return (project as EnvDTE.Project);
        }
    }
}
