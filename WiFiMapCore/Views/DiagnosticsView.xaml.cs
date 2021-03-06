﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WiFiMapCore.Views
{
    public partial class DiagnosticsView : Window
    {
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private GridViewColumnHeader? _lastHeaderClicked;

        public DiagnosticsView()
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
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                            direction = ListSortDirection.Descending;
                        else
                            direction = ListSortDirection.Ascending;
                    }

                    SortByHeader(headerClicked, direction);
                }
        }

        private void SortByHeader(GridViewColumnHeader headerClicked, ListSortDirection direction)
        {
            var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
            var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

            Sort(sortBy, direction);

            if (direction == ListSortDirection.Ascending)
                headerClicked.Column.HeaderTemplate =
                    Resources["HeaderTemplateArrowUp"] as DataTemplate;
            else
                headerClicked.Column.HeaderTemplate =
                    Resources["HeaderTemplateArrowDown"] as DataTemplate;

            // Remove arrow from previously sorted header
            if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                _lastHeaderClicked.Column.HeaderTemplate = null;

            _lastHeaderClicked = headerClicked;
            _lastDirection = direction;
        }

        private void Sort(string? sortBy, ListSortDirection direction)
        {
            var dataView =
                CollectionViewSource.GetDefaultView(lv.ItemsSource);

            dataView.SortDescriptions.Clear();
            var sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void lv_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (_lastHeaderClicked != null) SortByHeader(_lastHeaderClicked, _lastDirection);
        }
    }
}