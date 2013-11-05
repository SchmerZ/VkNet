using System.Windows.Controls;
using System.Windows.Input;

using VkSync.Helpers;

namespace VkSync.Views
{
    /// <summary>
    /// Interaction logic for Audio.xaml
    /// </summary>
    public partial class Audio : UserControl
    {
        public Audio()
        {
            InitializeComponent();
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