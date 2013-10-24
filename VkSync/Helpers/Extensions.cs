using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VkSync.Helpers
{
    public static class Extensions
    {
        public static void RemoveSelectedItem<T> (this Selector selector)
            where T : class 
        {
            var collection = selector.ItemsSource as ObservableCollection<T>;

            if (collection == null)
                throw new InvalidCastException("Cannot cast ItemsSource collection to observable collection.");

            var selectedItem = (T)selector.SelectedItem;
            var selectedItemIndex = selector.SelectedIndex;

            collection.Remove(selectedItem);

            if (!selector.Items.IsEmpty)
            {
                if (selectedItemIndex == 0)
                    selector.SelectedIndex = 0;
                else
                    selector.SelectedIndex = selectedItemIndex - 1;
            }
        }

        public static void AddRoutedCommandToButton(ButtonBase button, ExecutedRoutedEventHandler handler, Func<bool> validator)
		{
			var routedCommand = new RoutedCommand();
			button.Command = routedCommand;

			var commandBinding = new CommandBinding(
				routedCommand,
				handler,
				(sender, args) => { args.CanExecute = validator(); });

			button.CommandBindings.Remove(commandBinding);
			button.CommandBindings.Add(commandBinding);
		}

        public static bool IsValidEmail(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            var emailRegex =
                new Regex("^[_a-z0-9-]+(.[a-z0-9-]+)@[a-z0-9-]+(.[a-z0-9-]+)*(\\.[a-z]{2,4})$");

            return emailRegex.IsMatch(value);
        }
    }
}