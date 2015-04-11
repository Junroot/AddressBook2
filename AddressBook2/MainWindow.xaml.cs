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
using System.IO;
using System.Xml;

namespace AddressBook2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Address> AddressBook = new List<Address>();
<<<<<<< HEAD
        List<Address> Filtered = new List<Address>();
=======
<<<<<<< HEAD
        List<Address> Filtered = new List<Address>();
=======
>>>>>>> origin/master
>>>>>>> origin/master
        MediaPlayer mp = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
            using (XmlReader reader = XmlReader.Create("Address.xml"))
            {
                String name = "";
                String number = "";
                String status = "";
                while(reader.Read())
                {
                    /* Element들의 구조가
                     * Addresses - Address - Name
                     *                     - Number
                     *                    (- size of number list)
                     *                    (- number list)
                     * 이런 구조로 되어있으므로
                     * 현재 Element가 status에 뭔지 구분해서
                     * 각 알맞는 것에 추가한 후
                     * Address로 만들어서 AddressBook에 추가
                     * 
                     * xml이라서 유니코드도 지원하고
                     * C#에서 다른 언어 섞여있어도 잘 정렬해준다 */
                    switch(reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            status = reader.Name;
                            break;
                        case XmlNodeType.Text:
                            if (status == "Name")
                                name = reader.Value;
                            else if (status == "Number")
                                number = reader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            if(reader.Name == "Address")
                                AddressBook.Add(new Address(name, number));
                            break;
                        default:
                            break;
                    }
                }
            }
            RefreshListView();
        }
        private void RefreshListView() //리스트뷰 (AddressBookList) 새로고침
        {
            AddressBookList.Items.Clear(); //ListView 내용 초기화

            AddressBook.Sort(delegate(Address x, Address y) //이름순으로 sort
            {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });

            foreach (var item in AddressBook) //순서대로 Listview에 넣음
            {
                if (item.Name.ToLower().IndexOf(listsearch.Text.ToLower()) != -1 || item.RepNumber.ToLower().IndexOf(listsearch.Text.ToLower()) != -1)
                AddressBookList.Items.Add(new User(item.Name, item.RepNumber));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show("변경 내용을 저장하시겠습니까?", "AddressBook", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (result == MessageBoxResult.Yes)//닫힐 때 파일 저장
            {
                using (XmlTextWriter writer = new XmlTextWriter("Address.xml", System.Text.Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Addresses");
                    foreach (Address address in AddressBook)
                    {
                        writer.WriteStartElement("Address");
                        writer.WriteElementString("Name", address.Name);
                        writer.WriteElementString("Number", address.RepNumber);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        private void AddressBookList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User u = new User();
            if (AddressBookList.SelectedItem != null)
            {
                u = (User)AddressBookList.SelectedItem;
                NameBlock.Text = u.Name;
                NumberBlock.Text = u.Number;
                ListSelected.Visibility = Visibility.Visible;
                Action.Visibility = Visibility.Collapsed;
                AddBox.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;
                CallBox.Visibility = Visibility.Collapsed;
                ReceiveCall.Visibility = Visibility.Collapsed;
            }
            else
            {
                ListSelected.Visibility = Visibility.Collapsed;
                Action.Visibility = Visibility.Visible;
                AddBox.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;
                CallBox.Visibility = Visibility.Collapsed;
                ReceiveCall.Visibility = Visibility.Collapsed;
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
                CallBox.Visibility = Visibility.Collapsed;
                ReceiveCall.Visibility = Visibility.Collapsed;

            }
            else if (AddBox.Visibility == Visibility.Visible)
            {
                AddBox.Visibility = Visibility.Collapsed;
                Action.Visibility = Visibility.Visible;
                ListSelected.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;
                CallBox.Visibility = Visibility.Collapsed;
                ReceiveCall.Visibility = Visibility.Collapsed;
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
                    CallBox.Visibility = Visibility.Collapsed;
                    ReceiveCall.Visibility = Visibility.Collapsed;
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
                CallBox.Visibility = Visibility.Collapsed;
                ReceiveCall.Visibility = Visibility.Collapsed;
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO 선택된거의 이름을 리스트에서 찾아서 삭제하게 해야함
            if (AddressBookList.SelectedItem != null)
            {
                int temp = AddressBookList.Items.IndexOf(AddressBookList.SelectedItem);
                AddressBookList.Items.RemoveAt(AddressBookList.Items.IndexOf(AddressBookList.SelectedItem));
                AddressBook.RemoveAt(temp);
                if (AddressBookList.Items.Count > 0)
                {
                    if (temp >= AddressBookList.Items.Count) AddressBookList.SelectedItem = AddressBookList.Items[temp - 1];
                    else AddressBookList.SelectedItem = AddressBookList.Items[temp];
                }
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            User new_user;
            int temp = AddressBookList.SelectedIndex;
            AddressBookList.Items.Add(new_user = new User() { Name = NameBox.Text, Number = NumberBox.Text });
            //입력 받음
            AddressBook.Add(new Address(new_user));
            //받으면 소트도 해야겠지
            AddressBook.Sort(delegate(Address x, Address y)
            {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });
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
            CallBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            //기존에 바로 수정하던거에서 modified라는 User를 만들고 그걸 대입하게 함
            int temp = AddressBookList.Items.IndexOf(AddressBookList.SelectedItem);
            //선택된 리스트뷰의 인덱스가 결국 리스트의 인덱스니까...
            AddressBook.RemoveAt(temp); //지우고 그냥 다시 씀
            User modified = new User(EditNameBox.Text, EditNumberBox.Text);
            ((User)AddressBookList.SelectedItem).Name = modified.Name;
            ((User)AddressBookList.SelectedItem).Number = modified.Number;

            AddressBook.Add(new Address(modified));

            RefreshListView(); //이때 재정렬됨

            AddBox.Visibility = Visibility.Collapsed;
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Visible;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
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
                    CallBox.Visibility = Visibility.Collapsed;
                    ReceiveCall.Visibility = Visibility.Collapsed;
                }
                else if (ListSelected.Visibility == Visibility.Visible)
                {
                    ListSelected.Visibility = Visibility.Collapsed;
                    Action.Visibility = Visibility.Visible;
                    AddBox.Visibility = Visibility.Collapsed;
                    EditBox.Visibility = Visibility.Collapsed;
                    CallBox.Visibility = Visibility.Collapsed;
                    ReceiveCall.Visibility = Visibility.Collapsed;
                }
            }
        }

        void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Collapsed;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Visible;
            ReceiveCall.Visibility = Visibility.Collapsed;
        }

        private void HomeonCallButton_Click(object sender, RoutedEventArgs e)
        {
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Visible;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
        }

        private MediaPlayer mediaPlayer = new MediaPlayer();

        private void ReceiveButton_Click(object sender, RoutedEventArgs e)
        {
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Collapsed;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Visible;
            
            mp.Open(new Uri(@"Bell.mp3", UriKind.Relative));
            mp.Play();
        }

        private void RefuseButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
            CallBox.Visibility = Visibility.Visible;
            ReceiveCall.Visibility = Visibility.Collapsed;
        }

        private void CallCloseButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
            Calling.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Visible;
        }

        private void ReveiveButton_Click(object sender, RoutedEventArgs e)
        {
            Calling.Visibility = Visibility.Visible;
            ReceiveCall.Visibility = Visibility.Collapsed;
            mp.Stop();
            mp.Open(new Uri(@"mosimosi.mp3", UriKind.Relative));
            mp.Play();
        }

        public class User
        {
            public User (String name, String number)
            {
                Name = name;
                Number = number;
            }
            public User ()
            {
                Name = "";
                Number = "";
            }
            public User(User user)
            {
                Name = user.Name;
                Number = user.Number;
            }
            public string Name { get; set; }

            public string Number { get; set; }
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> origin/master
        private void listsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshListView();
        }

<<<<<<< HEAD
=======
=======
>>>>>>> origin/master
>>>>>>> origin/master
        
    }
}