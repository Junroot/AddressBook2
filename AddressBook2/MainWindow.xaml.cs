using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AddressBook2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddressBookList.Items.Add(new User() { Name = "John Doe", Number = "01012345678" });
            AddressBookList.Items.Add(new User() { Name = "Jane Doe", Number = "01012345679" });
            AddressBookList.Items.Add(new User() { Name = "Sammy Doe", Number = "01012345680" });

            AddressBookList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name",System.ComponentModel.ListSortDirection.Ascending));
            AddressBookList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Number", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show("Do you want to close this window?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void AddressBookList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User u = new User() { };
            if (AddressBookList.SelectedItem != null)
            {
                u = (User)AddressBookList.SelectedItem;
                NameBlock.Text = u.Name;
                NumberBlock.Text = u.Number;
                ListSelected.Visibility = Visibility.Visible;
                Action.Visibility = Visibility.Collapsed;
                AddBox.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                ListSelected.Visibility = Visibility.Collapsed;
                Action.Visibility = Visibility.Visible;
                AddBox.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddBox.Visibility == Visibility.Collapsed)
            {
                AddBox.Visibility = Visibility.Visible;
                Action.Visibility = Visibility.Collapsed;
                ListSelected.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;

            }
            else if (AddBox.Visibility == Visibility.Visible)
            {
                AddBox.Visibility = Visibility.Collapsed;
                Action.Visibility = Visibility.Visible;
                ListSelected.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;
                NameBox.Text = String.Empty;
                NumberBox.Text = String.Empty;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditBox.Visibility == Visibility.Collapsed)
            {
                if (AddressBookList.SelectedItem != null)
                {
                    User u = (User)AddressBookList.SelectedItem;
                    AddBox.Visibility = Visibility.Collapsed;
                    Action.Visibility = Visibility.Collapsed;
                    ListSelected.Visibility = Visibility.Collapsed;
                    EditBox.Visibility = Visibility.Visible;
                    EditNameBox.Text = u.Name;
                    EditNumberBox.Text = u.Number;
                }

            }
            else if (EditBox.Visibility == Visibility.Visible)
            {
                AddBox.Visibility = Visibility.Collapsed;
                Action.Visibility = Visibility.Visible;
                ListSelected.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;

            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddressBookList.SelectedItem != null)
            {
                int temp = AddressBookList.Items.IndexOf(AddressBookList.SelectedItem);
                AddressBookList.Items.RemoveAt(AddressBookList.Items.IndexOf(AddressBookList.SelectedItem));
                if (AddressBookList.Items.Count > 0)
                {
                    if (temp >= AddressBookList.Items.Count) AddressBookList.SelectedItem = AddressBookList.Items[temp - 1];
                    else AddressBookList.SelectedItem = AddressBookList.Items[temp];
                }
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            int temp = AddressBookList.SelectedIndex;
            AddressBookList.Items.Add(new User() { Name = NameBox.Text, Number = NumberBox.Text });
            if (temp != -1)
            {
                AddressBookList.SelectedItem = AddressBookList.Items[temp];
            }
            NameBox.Text = String.Empty;
            NumberBox.Text = String.Empty;
            AddressBookList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            AddressBookList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Number", System.ComponentModel.ListSortDirection.Ascending));
            AddBox.Visibility = Visibility.Collapsed;
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Visible;
            EditBox.Visibility = Visibility.Collapsed;
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            ((User)AddressBookList.SelectedItem).Name = EditNameBox.Text;
            ((User)AddressBookList.SelectedItem).Number = EditNumberBox.Text;
            AddressBookList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            AddressBookList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Number", System.ComponentModel.ListSortDirection.Ascending));
            AddressBookList.Items.Refresh();
            AddBox.Visibility = Visibility.Collapsed;
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Visible;
            EditBox.Visibility = Visibility.Collapsed;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                if (ListSelected.Visibility == Visibility.Collapsed)
                {
                    ListSelected.Visibility = Visibility.Visible;
                    Action.Visibility = Visibility.Collapsed;
                    AddBox.Visibility = Visibility.Collapsed;
                    EditBox.Visibility = Visibility.Collapsed;
                }
                else if (ListSelected.Visibility == Visibility.Visible)
                {
                    ListSelected.Visibility = Visibility.Collapsed;
                    Action.Visibility = Visibility.Visible;
                    AddBox.Visibility = Visibility.Collapsed;
                    EditBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public class User
        {
            public string Name { get; set; }

            public string Number { get; set; }
        }
    }
}