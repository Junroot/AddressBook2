﻿using System;
using System.Collections.Generic;
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
        }


        private void AddressBookList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User u = new User() { };
            if (AddressBookList.SelectedItem != null)   u = (User)AddressBookList.SelectedItem;
            NameBlock.Text = u.Name;
            NumberBlock.Text = u.Number;
            ListSelected.Visibility = Visibility.Visible;
            Action.Visibility = Visibility.Collapsed;
            AddBox.Visibility = Visibility.Collapsed;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddBox.Visibility == Visibility.Collapsed)
            {
                AddBox.Visibility = Visibility.Visible;
                Action.Visibility = Visibility.Collapsed;
                ListSelected.Visibility = Visibility.Collapsed;
            }
            else if (AddBox.Visibility == Visibility.Visible)
            {
                AddBox.Visibility = Visibility.Collapsed;
                Action.Visibility = Visibility.Visible;
                ListSelected.Visibility = Visibility.Collapsed;
                NameBox.Text = String.Empty;
                NumberBox.Text = String.Empty;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

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
            AddressBookList.Items.Add(new User() { Name = NameBox.Text , Number = NumberBox.Text });
            if (temp != -1)
            {
                AddressBookList.SelectedItem = AddressBookList.Items[temp];
            }
            NameBox.Text = String.Empty;
            NumberBox.Text = String.Empty;
            AddBox.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Visible;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                if (ListSelected.Visibility == Visibility.Collapsed )
                {
                    ListSelected.Visibility = Visibility.Visible;
                    Action.Visibility = Visibility.Collapsed;
                    AddBox.Visibility = Visibility.Collapsed;
                }
                else if (ListSelected.Visibility == Visibility.Visible)
                {
                    ListSelected.Visibility = Visibility.Collapsed;
                    Action.Visibility = Visibility.Visible;
                    AddBox.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    public class User
    {
        public string Name { get; set; }

        public string Number { get; set; }
    }
}