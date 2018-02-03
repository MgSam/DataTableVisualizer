using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataTableViewer;
using NamedPipeWrapper;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Package = Microsoft.VisualStudio.Shell.Package;

namespace ShineTools
{
    // Either add the following to SomeType's definition to see this visualizer when debugging instances of SomeType or add to the autoexp.cs file.
    //  [DebuggerVisualizer(typeof(ShineTools.DataTableVisualizer))]
    //  [Serializable]
    //  public class SomeType
    //  {
    //   ...
    //  }
    // 

    /// <summary>
    /// A Visualizer for DataTables.  
    /// </summary>
    public class DataTableVisualizer : DialogDebuggerVisualizer
    {
        private const int CONNECTION_TIMEOUT = 5_000;
        private const int OVERALL_TIMEOUT = 10_000;

        /// <summary>
        /// Shows the debugger window. This method MUST complete everything it needs sychronously before ending.
        /// </summary>
        /// <param name="windowService"></param>
        /// <param name="objectProvider"></param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException(nameof(windowService));
            if (objectProvider == null)
                throw new ArgumentNullException(nameof(objectProvider));

            if(!(objectProvider.GetObject() is DataTable table)) throw new InvalidOperationException("Only works on DataTables");

            sendToVsToolWindow(table).Wait(OVERALL_TIMEOUT);
        }

        private async Task sendToVsToolWindow(DataTable table)
        {
            //Send messages to the VS tool window
            var client = new NamedPipeClient<DataTable>("DataTablePipe");
            client.Start();

            showToolWindow();
            var success = await WaitForConnectionAsync(client, CONNECTION_TIMEOUT).ConfigureAwait(false);
            if (success)
                client.PushMessage(table);
        }

        private async Task<bool> WaitForConnectionAsync<T>(NamedPipeClient<T> client, int millisecondTimeout) where T : class
        {
            var success = false;
            var waitTask = Task.Run(() =>
            {
                client.WaitForConnection();
                success = true;
            });
            await Task.WhenAny(Task.Delay(millisecondTimeout), waitTask).ConfigureAwait(false);

            return success;
        }

        private void showToolWindow()
        {
            var vsUiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));
            var guid = Guid.Parse(
                "e3bfb80d-d50a-4a85-bacd-64daca9095f7"); //This is the exact Guid declared by the DataTableVisualizerToolWindow
            var result = vsUiShell.FindToolWindow((uint) __VSFINDTOOLWIN.FTW_fFindFirst, ref guid,
                out var windowFrame); // Find MyToolWindow

            if (result != VSConstants.S_OK)
                result = vsUiShell.FindToolWindow((uint) __VSFINDTOOLWIN.FTW_fForceCreate, ref guid,
                    out windowFrame); // Crate MyToolWindow if not found

            if (result == VSConstants.S_OK) // Show MyToolWindow
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
