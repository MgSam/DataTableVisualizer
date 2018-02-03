using System;
using System.Data;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using NamedPipeWrapper;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DataTableVisualizerExtension
{
    /// <summary>
    /// Interaction logic for DataTableVisualizerToolWindowControl.
    /// </summary>
    public partial class DataTableVisualizerToolWindowControl : UserControl, IDisposable
    {
        private readonly NamedPipeServer<DataTable> _server;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableVisualizerToolWindowControl"/> class.
        /// </summary>
        public DataTableVisualizerToolWindowControl()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;

            _server = new NamedPipeServer<DataTable>("DataTablePipe");
            _server.ClientMessage += ServerOnClientMessage;
            _server.Start();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Debug.Write(nameof(OnLoaded));
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Debug.Write(nameof(OnUnloaded));
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DataTableViewer.Table = null;
            }));
        }

        private void ServerOnClientMessage(NamedPipeConnection<DataTable, DataTable> connection, DataTable message)
        {
            showToolWindow();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                DataTableViewer.Table = message;
            }));
        }

        private void showToolWindow()
        {
            var vsUiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
            var guid = typeof(DataTableVisualizerToolWindow).GUID;
            var result = vsUiShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFindFirst, ref guid, out var windowFrame);   // Find MyToolWindow

            if (result != VSConstants.S_OK)
                result = vsUiShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guid, out windowFrame); // Crate MyToolWindow if not found

            if (result == VSConstants.S_OK)                                                                           // Show MyToolWindow
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        public void Dispose()
        {
            if (_disposed) return;

            _server.Stop();
            DataTableViewer.Table = null;

            _disposed = true;
        }
    }
}