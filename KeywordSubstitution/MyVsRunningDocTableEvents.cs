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
using EnvDTE;
using EnvDTE80;
using System.IO;
using KeywordSubstitution.SubstituteRules;

namespace KeywordSubstitution
{
    /// <summary>   My VS RDT event handlers. </summary>
    internal class MyVsRunningDocTableEvents : IVsRunningDocTableEvents3
    {
        /// <summary>   VS service provider. </summary>
        private IServiceProvider _services;
        /// <summary>   Manager for substitute rules. </summary>
        private SubstituteRules.SubstituteRuleManager _substituteRuleManager = new SubstituteRules.SubstituteRuleManager();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="services"> VS service provider. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [ImportingConstructor]
        public MyVsRunningDocTableEvents(IServiceProvider services)
        {
            _services = services;
            setupSubstituteRules();
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeSave(uint docCookie)
        {
            var dataProvider = new SubstituteRuleDataProvider()
            {
                VsRunningDocumentTable = (IVsRunningDocumentTable)_services.GetService(typeof(SVsRunningDocumentTable))
            };

            if (dataProvider.VsRunningDocumentTable == null)
            {
                MessageBox.Show("Unable to get running document table.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return VSConstants.S_OK;
            }

            dataProvider.DocumentInfo = new VsDocumentInfo(docCookie, dataProvider.VsRunningDocumentTable);
            if (!dataProvider.DocumentInfo.IsSuccess)
            {
                MessageBox.Show("Unable to get document info from RDT.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return VSConstants.S_OK;
            }

            // Filter based on the document's file extension
            /*const string FileExtensionFilter = ".cs;.txt;.xml";
            foreach (string ext in FileExtensionFilter.Split(';'))
            {
                if (Path.GetExtension(dataProvider.DocumentInfo.pbstrMkDocument) != ext)
                {
                    return VSConstants.S_OK;
                }
            }*/

            Trace.WriteLine(string.Format("Document file path: {0}", dataProvider.DocumentInfo.pbstrMkDocument), GetType().FullName);

            string filePath = dataProvider.DocumentInfo.pbstrMkDocument;
            dataProvider.TextView = VsHelper.GetIVsTextView(filePath);
            if (dataProvider.TextView == null)
            {
                Trace.WriteLine("Document has no IVsTextView; skipped.", GetType().FullName);
                //MessageBox.Show("Unable to get IVsTextView.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return VSConstants.S_OK;
            }

            dataProvider.WpfTextView = VsHelper.GetWpfTextView(dataProvider.TextView);
            if (dataProvider.WpfTextView == null)
            {
                Trace.WriteLine("Document has no IWpfTextView; skipped.", GetType().FullName);
                //MessageBox.Show("Unable to get IWpfTextView.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return VSConstants.S_OK;
            }

            // There should have been created a WPF text view for this document.
            // While I'm not sure if this is necessary, I'm staying on the safe side.
            if (!MyTextViewCreationListener.OpenDocumentsWithWpfTextView.Contains(dataProvider.WpfTextView))
            {
                Trace.WriteLine("This document's IWpfTextView was not previously registered; might have to investigate this.", GetType().FullName);
                return VSConstants.S_OK;
            }

            dataProvider.Project = VsHelper.GetProject(dataProvider.DocumentInfo.ppHier);
            if (dataProvider.Project == null)
            {
                MessageBox.Show("Unable to get the project this document belongs to.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return VSConstants.S_OK;
            }

            var textEdit = dataProvider.WpfTextView.TextBuffer.CreateEdit();

            try
            {
                _substituteRuleManager.ExecuteRules(dataProvider, textEdit);
            }
            catch (Exception ex)
            {
                // Get the inner-most exception which should have the most useful exception message
                Exception innerEx = ex;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }

                MessageBox.Show(string.Format("An unhandled exception occurred during substitute rule execution: {0}", innerEx.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return VSConstants.S_OK;
            }

            // It's probably better to abort the whole edit if
            if (textEdit.HasFailedChanges)
            {
                MessageBox.Show("Failed to apply all changes due to read-only regions; aborting.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                textEdit.Cancel();
                return VSConstants.S_OK;
            }

            // I guess there's no need to apply the edit if there were no effective changes.
            if (!textEdit.HasEffectiveChanges)
            {
                textEdit.Cancel();
                return VSConstants.S_OK;
            }

            textEdit.Apply();
            return VSConstants.S_OK;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sets up the substitute rules. For example, custom rule name aliases are registered here.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void setupSubstituteRules()
        {
            // Custom name aliases
            _substituteRuleManager.RegisterRuleNameAlias("IncrementNumber", "FileSaveCounter");
            _substituteRuleManager.RegisterRuleNameAlias("DateTime", "FileSaveDateTime");
            _substituteRuleManager.RegisterRuleNameAlias("UserName", "FileSaveUser");
            _substituteRuleManager.RegisterRuleNameAlias("MachineName", "FileSaveMachine");
            _substituteRuleManager.RegisterRuleNameAlias("ProjectDir", "FilePathRootHint");
            _substituteRuleManager.RegisterRuleNameAlias("Guid", "FileSaveGuid");
        }
    }
}
