using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WiFiMapCore.ViewModels;

namespace WiFiMapCore.Views
{
    /// <summary>
    ///     Interaction logic for Results.xaml
    /// </summary>
    public partial class ScanView : UserControl
    {
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private GridViewColumn? _lastHeaderClickedColumn;

        public ScanView()
        {
            InitializeComponent();
        }

        private void GridViewColumnHeaderClickedHandler(object sender,
            RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked.Column != _lastHeaderClickedColumn)
                        direction = ListSortDirection.Ascending;
                    else
                        direction = _lastDirection == ListSortDirection.Ascending
                            ? ListSortDirection.Descending
                            : ListSortDirection.Ascending;

                    SortByHeader(direction, headerClicked.Column);
                }
        }

        private void SortByHeader(ListSortDirection direction, GridViewColumn headerClickedColumn)
        {
            var columnBinding = headerClickedColumn.DisplayMemberBinding as Binding;
            var sortBy = columnBinding?.Path.Path ?? headerClickedColumn.Header as string;

            Sort(sortBy, direction);

            if (direction == ListSortDirection.Ascending)
                headerClickedColumn.HeaderTemplate =
                    Resources["HeaderTemplateArrowUp"] as DataTemplate;
            else
                headerClickedColumn.HeaderTemplate =
                    Resources["HeaderTemplateArrowDown"] as DataTemplate;

            // Remove arrow from previously sorted header
            if (_lastHeaderClickedColumn != null && _lastHeaderClickedColumn != headerClickedColumn)
                _lastHeaderClickedColumn.HeaderTemplate = null;

            //_lastHeaderClicked = headerClicked;
            _lastHeaderClickedColumn = headerClickedColumn;
            _lastDirection = direction;
        }

        private void Sort(string? sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
                CollectionViewSource.GetDefaultView(lv.ItemsSource);

            dataView.SortDescriptions.Clear();
            var sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}