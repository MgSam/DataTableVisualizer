namespace DataTableVisualizerExtension
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("e3bfb80d-d50a-4a85-bacd-64daca9095f7")]
    public class DataTableVisualizerToolWindow : ToolWindowPane
    {
        private DataTableVisualizerToolWindowControl _visualizerToolWindowControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableVisualizerToolWindow"/> class.
        /// </summary>
        public DataTableVisualizerToolWindow() : base(null)
        {
            this.Caption = "DataTable Visualizer";

            _visualizerToolWindowControl = new DataTableVisualizerToolWindowControl();
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = _visualizerToolWindowControl;
            
        }

        protected override void OnClose()
        {
            base.OnClose();

            _visualizerToolWindowControl.Dispose();
        }
    }
}
