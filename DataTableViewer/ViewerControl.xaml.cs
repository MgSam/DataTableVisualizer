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
    /// Interaction logic for ViewerControl.xaml
    /// </summary>
    public partial class ViewerControl : UserControl, INotifyPropertyChanged
    {
		private const String DEFAULT_TABLE_NAME = "Table";
        private const String SEARCH_DEFAULT = "Type to search...";

        /// <summary>
        /// A compiled filter function based on the OData query specified by the user in the search box.
        /// </summary>
        private Func<NoThrowDictionary<string, object>, bool> _oDataFilter;

        /// <summary>
        /// The collection view bound to the DataGridControl used in the UI.
        /// </summary>
        public DataGridCollectionView TableView
        {
            get => _tableView;
            private set
            {
                if (Equals(value, _tableView)) return;
                _tableView = value;
                OnPropertyChanged();
            }
        }
        private DataGridCollectionView _tableView;

        /// <summary>
        /// The DataTable the Viewer is bound to.
        /// </summary>
        public DataTable Table
        {
            //get => _table;
            set
            {
                if (Equals(value, _table)) return;
                _table = value;
                TableView = _table == null ? null : new DataGridCollectionView(_table.DefaultView)
                {
                    DistinctValuesConstraint =  DistinctValuesConstraint.Filtered,
                    Filter = filter
                };
                Title = _table?.TableName ?? DEFAULT_TABLE_NAME;
                Search = "";
                PendingSearch = SEARCH_DEFAULT;
                //var columns = _table.Columns.Cast<DataColumn>()
                //    .Select(c => new KeyValuePair<String, Type>(c.ColumnName, c.DataType)).ToArray();
                //_rowType = MyTypeBuilder.CompileResultType(columns);

                OnPropertyChanged();
            }
        }
        private DataTable _table;

        /// <summary>
        /// User supplied search string. Can be either a vanilla "contains" match or an OData Query.
        /// </summary>
        public String Search
        {
            get => _search;
            set
            {
                if (value == _search) return;
                _search = value;

                _oDataFilter = getODataFilter(_search);

                FilterState = String.IsNullOrWhiteSpace(_search) || _search == SEARCH_DEFAULT 
                    ? FilterState.Empty
                    : _oDataFilter != null
                        ? FilterState.OData
                        : FilterState.Contains;

                if(this.TableView != null)this.TableView.Filter = filter;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UncommittedSearchChange));
            }
        }
        private string _search;

        public String PendingSearch
        {
            get => _pendingSearch;
            set
            {
                _pendingSearch = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UncommittedSearchChange));
            }
        }
        private String _pendingSearch;

        public bool UncommittedSearchChange => Search != (PendingSearch == SEARCH_DEFAULT ? "" : PendingSearch);

        /// <summary>Table title.</summary>
		public String Title
		{
			get => _title;
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}
		private String _title;

        public FilterState FilterState
        {
            get => _filterState;
            set
            {
                _filterState = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ImageSource));
            }
        }
        private FilterState _filterState;

        public String ImageSource => FilterState == FilterState.OData
            ? "Images/FullFilterOData.png"
            : FilterState == FilterState.Contains
                ? "Images/FullFilterContains.png"
                : "Images/EmptyFilter.png";

        /// <summary>Constructor.</summary>
		public ViewerControl()
        {
            InitializeComponent();

            copyWithHeaders(false);
        }

        /// <summary>
        /// Provides an implementation of filtering for each DataRow in the bound table.
        /// </summary>
        /// <param name="o">The DataRow to check.</param>
        /// <returns></returns>
        private bool filter(Object o)
        {
            if (String.IsNullOrWhiteSpace(Search) || Search == SEARCH_DEFAULT) return true;

            if (!(o is DataRowView rowView)) return true;
            var row = rowView.Row;

            //Try to use the OData filter if there is one
            if (_oDataFilter != null)
            {
                var dict = new NoThrowDictionary<string, object>(row.GetItemDictionary(caseInvariant: true));

                /* If something is wrong with the expression, it's likely an issue with the DataTable column type, but there's nothing 
                 * we can do about that so just ignore the exception */
                try
                {
                    if (_oDataFilter.Invoke(dict))
                    {
                        return true;
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString()); 
                }
            }
            //Otherwise, perform a simple contains match again
            else
            {
                foreach (var item in row.ItemArray)
                {
                    if (item.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) > -1)
                        return true;
                }
            }
            return false;
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

        /// <summary>
        /// Resets the text of the search box when it has lost focus and is empty.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void SearchBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(PendingSearch))
                PendingSearch = SEARCH_DEFAULT;
        }

        [DebuggerNonUserCode]
        private static Func<NoThrowDictionary<string, object>, bool> getODataFilter(String oDataExpression)
        {
            Func<NoThrowDictionary<string, object>, bool> result;
            try
            {
                var expr = new IndexerODataFilterLanguage().Parse<NoThrowDictionary<string, object>>(oDataExpression);
                
                result = expr.Compile(); ;
            }
            catch
            {
                result = null;
            }

            return result;
        }

        private void SearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    //if (String.IsNullOrWhiteSpace(PendingSearch)) PendingSearch = SEARCH_DEFAULT;

                    Search = PendingSearch == SEARCH_DEFAULT ? "" : PendingSearch;
                    break;
            }
        }

        private void CopyWithHeadersCommandExecuted(object sender, RoutedEventArgs e)
        {
            try
            {
                copyWithHeaders(true);

                CopyContextItem.Command?.Execute(null);
            }
            finally
            {
                copyWithHeaders(false);
            }
        }

        private void copyWithHeaders(bool value)
        {
            foreach (var e in TheGrid.ClipboardExporters.Values)
                e.IncludeColumnHeaders = value;
        }
    }

    public enum FilterState
    {
        Empty,
        OData,
        Contains
    }
}
