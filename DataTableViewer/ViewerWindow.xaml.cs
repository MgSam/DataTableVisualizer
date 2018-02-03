using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataTableViewer.Annotations;
using StringToExpression.LanguageDefinitions;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace DataTableViewer
{
    /// <summary>
    /// Provides a searchable view on a DataTable.
    /// </summary>
    public sealed partial class ViewerWindow : Window, INotifyPropertyChanged
    {
        private const String DEFAULT_TITLE = "Data Table Viewer";

        /// <summary>
        /// The DataTable the Viewer is bound to.
        /// </summary>
        public DataTable Table
        {
            //get => _table;
            set
            {
                Viewer.Table = value;
                Title = String.IsNullOrWhiteSpace(value.TableName) ? DEFAULT_TITLE : value.TableName;

                OnPropertyChanged();
            }
        }

        /// <summary>Constructor.</summary>
        public ViewerWindow()
        {
            InitializeComponent();
        }

		#region INotifyPropertyChanged

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }//End class
}//End namespace
