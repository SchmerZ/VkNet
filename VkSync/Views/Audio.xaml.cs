using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using VkSync.Helpers;

namespace VkSync.Views
{
    public partial class Audio : UserControl
    {
        public Audio()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            if (DataContext is IDisposable)
                ((IDisposable)DataContext).Dispose();
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;
            var row = VisualTreeHelpers.FindAncestor<DataGridRow>(cell);

            if (!cell.IsEditing)
            {
                // enables editing on single click
                if (!cell.IsFocused)
                    cell.Focus();

                if (!cell.IsSelected)
                {
                    //cell.IsSelected = true;
                    row.IsSelected = true;
                }
            }
        }
    }
}