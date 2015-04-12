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
        List<Address> Filtered = new List<Address>();
        List<Call> CallBook = new List<Call>();
        List<SMS> InBox = new List<SMS>();
        List<SMS> OutBox = new List<SMS>();
        MediaPlayer mp = new MediaPlayer();
        String CallUserNum;
        Address CallUser;
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
            using (XmlReader reader = XmlReader.Create("Call.xml"))
            {
                String time = "";
                String number = "";
                String state = "";
                String status = "";
                while(reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            status = reader.Name;
                            break;
                        case XmlNodeType.Text:
                            if (status == "Time")
                                time = reader.Value;
                            else if (status == "Number")
                                number = reader.Value;
                            else if (status == "State")
                                state = reader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Call")
                                CallBook.Add(new Call(time, number, state));
                            break;
                        default:
                            break;
                    }
                }
            }
            
            RefreshCallBookList();
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
        private void RefreshCallBookList()
        {
            CallBookList.Items.Clear();
            CallBook.Sort(delegate(Call x, Call y) //시간순으로 sort
            {
                if (x.Time == null && y.Time == null) return 0;
                else if (x.Time == null) return -1;
                else if (y.Time == null) return 1;
                else return -(x.Time.CompareTo(y.Time));
            });
            foreach (var item in CallBook) //순서대로 Listview에 넣음
            {
                CallUser = (Address)AddressBook.Find(delegate(Address o) { return o.RepNumber == item.Number; });
                String printtime = "" + item.Time[0] + item.Time[1] + item.Time[2] + item.Time[3] + "-" + item.Time[4] + item.Time[5] + "-" + item.Time[6] + item.Time[7] + " " + item.Time[8] + item.Time[9] + ":" + item.Time[10] + item.Time[11] + ":" + item.Time[12] + item.Time[13];
                if (CallUser != null)
                {
                    CallBookList.Items.Add(new Call(printtime, CallUser.Name, item.State));
                }
                else
                {
                    CallBookList.Items.Add(new Call(printtime, item.Number, item.State));
                }
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
                using (XmlTextWriter writer = new XmlTextWriter("Call.xml", System.Text.Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Calls");
                    foreach (Call call in CallBook)
                    {
                        writer.WriteStartElement("Call");
                        writer.WriteElementString("Time", call.Time);
                        writer.WriteElementString("Number", call.Number);
                        writer.WriteElementString("State", call.State);
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
                Calling.Visibility = Visibility.Collapsed;
                CallHistory.Visibility = Visibility.Collapsed;
                SMSBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                ListSelected.Visibility = Visibility.Collapsed;
                Action.Visibility = Visibility.Visible;
                AddBox.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;
                CallBox.Visibility = Visibility.Collapsed;
                ReceiveCall.Visibility = Visibility.Collapsed;
                Calling.Visibility = Visibility.Collapsed;
                CallHistory.Visibility = Visibility.Collapsed;
                SMSBox.Visibility = Visibility.Collapsed;
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
                Calling.Visibility = Visibility.Collapsed;
                CallHistory.Visibility = Visibility.Collapsed;
                SMSBox.Visibility = Visibility.Collapsed;
            }
            else if (AddBox.Visibility == Visibility.Visible)
            {
                AddBox.Visibility = Visibility.Collapsed;
                Action.Visibility = Visibility.Visible;
                ListSelected.Visibility = Visibility.Collapsed;
                EditBox.Visibility = Visibility.Collapsed;
                CallBox.Visibility = Visibility.Collapsed;
                ReceiveCall.Visibility = Visibility.Collapsed;
                Calling.Visibility = Visibility.Collapsed;
                CallHistory.Visibility = Visibility.Collapsed;
                SMSBox.Visibility = Visibility.Collapsed;
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
                    Calling.Visibility = Visibility.Collapsed;
                    CallHistory.Visibility = Visibility.Collapsed;
                    SMSBox.Visibility = Visibility.Collapsed;
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
                Calling.Visibility = Visibility.Collapsed;
                CallHistory.Visibility = Visibility.Collapsed;
                SMSBox.Visibility = Visibility.Collapsed;
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO 선택된거의 이름을 리스트에서 찾아서 삭제하게 해야함
            if (AddressBookList.SelectedItem != null)
            {
                int temp = AddressBookList.Items.IndexOf(AddressBookList.SelectedItem);
                User Selected = (User)AddressBookList.Items[temp];
                foreach (var item in AddressBook)
                {
                    if (item.Name == Selected.Name && item.RepNumber == Selected.Number)
                    {
                        AddressBook.Remove(item);
                        break;
                    }
                }
              //  AddressBookList.Items.RemoveAt(AddressBookList.Items.IndexOf(AddressBookList.SelectedItem));
               // AddressBook.RemoveAt(temp);
                RefreshListView();
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
            Calling.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            //기존에 바로 수정하던거에서 modified라는 User를 만들고 그걸 대입하게 함
            int temp = AddressBookList.Items.IndexOf(AddressBookList.SelectedItem);
            //선택된 리스트뷰의 인덱스가 결국 리스트의 인덱스니까...
           // AddressBook.RemoveAt(temp); //지우고 그냥 다시 씀
            User Selected = (User)AddressBookList.Items[temp];
            foreach(var item in AddressBook)
            {
                if(item.Name==Selected.Name&&item.RepNumber==Selected.Number)
                {
                    item.Name = EditNameBox.Text;
                    item.RepNumber = EditNumberBox.Text;
                    break;
                }
            }
          /*  User modified = new User(EditNameBox.Text, EditNumberBox.Text);
            ((User)AddressBookList.SelectedItem).Name = modified.Name;
            ((User)AddressBookList.SelectedItem).Number = modified.Number;

            AddressBook.Add(new Address(modified));*/

            RefreshListView(); //이때 재정렬됨

            AddBox.Visibility = Visibility.Collapsed;
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Visible;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
            Calling.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;
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
                    Calling.Visibility = Visibility.Collapsed;
                    CallHistory.Visibility = Visibility.Collapsed;
                    SMSBox.Visibility = Visibility.Collapsed;
                }
                else if (ListSelected.Visibility == Visibility.Visible)
                {
                    ListSelected.Visibility = Visibility.Collapsed;
                    Action.Visibility = Visibility.Visible;
                    AddBox.Visibility = Visibility.Collapsed;
                    EditBox.Visibility = Visibility.Collapsed;
                    CallBox.Visibility = Visibility.Collapsed;
                    ReceiveCall.Visibility = Visibility.Collapsed;
                    Calling.Visibility = Visibility.Collapsed;
                    CallHistory.Visibility = Visibility.Collapsed;
                    SMSBox.Visibility = Visibility.Collapsed;
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
            Calling.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;
        }

        private void SMSButton_Click(object sender, RoutedEventArgs e)
        {
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Collapsed;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
            Calling.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Visible;
        }

        private void HomeonCallButton_Click(object sender, RoutedEventArgs e)
        {
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Visible;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
            Calling.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;
        }

        private void HomeonSMSButton_Click(object sender, RoutedEventArgs e)
        {
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Visible;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
            Calling.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;
            SendBox.Visibility = Visibility.Collapsed;
            main.Visibility = Visibility.Visible;
            AddButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Visible;
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
            Calling.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;

            Random r = new Random();

            int temp = r.Next(0,CallBook.Count-1);

            CallUserNum = CallBook[temp].Number;

            CallUser = AddressBook.Find(delegate(Address o) { return o.RepNumber == CallUserNum; });

            if (CallUser != null)
            {
                CallName.Text = CallUser.Name;
            }
            else
            {
                CallName.Text = CallUserNum;
            }
            

            mp.Open(new Uri(@"Bell.mp3", UriKind.Relative));
            mp.Play();
        }

        private void RefuseButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Collapsed;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Visible;
            ReceiveCall.Visibility = Visibility.Collapsed;
            Calling.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;
        }

        private void CallCloseButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Collapsed;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
            Calling.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Visible;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;
        }

        private void ReveiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CallUser != null)
            {
                CallingName.Text = CallUser.Name;
            }
            else
            {
                CallingName.Text = CallUserNum;
            }
            Call newcall = new Call(DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + DateTime.Now.ToString("HHmmss"), CallUserNum, "R");

            CallBook.Add(newcall);

            RefreshCallBookList();

            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Collapsed;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            Calling.Visibility = Visibility.Visible;
            ReceiveCall.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Collapsed;
            SMSBox.Visibility = Visibility.Collapsed;
            mp.Stop();
            mp.Open(new Uri(@"mosimosi.mp3", UriKind.Relative));
            mp.Play();
        }

        private void listsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshListView();
        }

         private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            ListSelected.Visibility = Visibility.Collapsed;
            Action.Visibility = Visibility.Collapsed;
            AddBox.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Collapsed;
            Calling.Visibility = Visibility.Collapsed;
            ReceiveCall.Visibility = Visibility.Collapsed;
            CallBox.Visibility = Visibility.Collapsed;
            CallHistory.Visibility = Visibility.Visible;
            SMSBox.Visibility = Visibility.Collapsed;
        }

         private void CloseCallBook_Click(object sender, RoutedEventArgs e)
         {
             ListSelected.Visibility = Visibility.Collapsed;
             Action.Visibility = Visibility.Collapsed;
             AddBox.Visibility = Visibility.Collapsed;
             EditBox.Visibility = Visibility.Collapsed;
             Calling.Visibility = Visibility.Collapsed;
             ReceiveCall.Visibility = Visibility.Collapsed;
             CallBox.Visibility = Visibility.Visible;
             CallHistory.Visibility = Visibility.Collapsed;
             SMSBox.Visibility = Visibility.Collapsed;
         }

        public class Call
        {
            public Call (String time, String number, String state)
            {
                Time = time;
                Number = number;
                State = state;
            }
            public Call()
            {
                Time = "";
                Number = "";
                State = "";
            }
            public string Time {get; set;}

            public string Number { get; set; }

            public string State {get; set;}
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
        public class SMS
        {
            public SMS(String time, String number, String content)
            {
                Time = time;
                Number = number;
                Content = content;
            }
            public SMS()
            {
                Time = "";
                Number = "";
                Content = "";
            }
            public SMS(SMS sms)
            {
                Time = sms.Time;
                Number = sms.Number;
                Content = sms.Content;
            }
            public String Time { get; set; }
            public String Number { get; set; }
            public String Content { get; set; }
        }

        private void ReceiveSMSButton_Click(object sender, RoutedEventArgs e)
        {
            main.Visibility = Visibility.Collapsed;

            SendBox.Visibility = Visibility.Visible;
            AddButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;
            SendList.Items.Clear();
            using (XmlReader reader = XmlReader.Create("Receive.xml"))
            {
                String time = "";
                String number = "";
                String content = "";
                String status = "";
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            status = reader.Name;
                            break;
                        case XmlNodeType.Text:
                            if (status == "Time")
                                time = reader.Value;
                                
                            else if (status == "Number")
                                number = reader.Value;
                            else if (status == "Content")
                                content = reader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Receive")
                                InBox.Add(new SMS(time, number, content));
                            break;
                        default:
                            break;
                    }
                    InBox.Sort(delegate(SMS x, SMS y) //시간순으로 sort
                    {
                        if (x.Time == null && y.Time == null) return 0;
                        else if (x.Time == null) return -1;
                        else if (y.Time == null) return 1;
                        else return -(x.Time.CompareTo(y.Time));
                    });
                    
                }
                foreach (var item in InBox)
                {
                    String printtime = "" + item.Time[4] + item.Time[5] + "-" + item.Time[6] + item.Time[7] + " " + item.Time[8] + item.Time[9] + ":" + item.Time[10] + item.Time[11];
                    item.Time = printtime;
                    foreach (var address in AddressBook)
                    {
                        if (address.RepNumber == item.Number)
                            item.Number = address.Name;
                    }
                    SendList.Items.Add(item);
                }
                reader.Close();
            }

        }

        private void SendSMS_Click(object sender, RoutedEventArgs e)
        {
            main.Visibility = Visibility.Collapsed;
            SendBox.Visibility = Visibility.Visible;
            AddButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;
            SendList.Items.Clear();
          
            using (XmlReader reader = XmlReader.Create("Send.xml"))
            {
                String time = "";
                String number = "";
                String content = "";
                String status = "";
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            status = reader.Name;
                            break;
                        case XmlNodeType.Text:
                            if (status == "Time")
                                time = reader.Value;

                            else if (status == "Number")
                                number = reader.Value;
                            else if (status == "Content")
                                content = reader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Send")
                                InBox.Add(new SMS(time, number, content));
                            break;
                        default:
                            break;
                    }
                    InBox.Sort(delegate(SMS x, SMS y) //시간순으로 sort
                    {
                        if (x.Time == null && y.Time == null) return 0;
                        else if (x.Time == null) return -1;
                        else if (y.Time == null) return 1;
                        else return -(x.Time.CompareTo(y.Time));
                    });

                }
                foreach (var item in InBox)
                {
                    String printtime = "" + item.Time[4] + item.Time[5] + "-" + item.Time[6] + item.Time[7] + " " + item.Time[8] + item.Time[9] + ":" + item.Time[10] + item.Time[11];
                    item.Time = printtime;
                    foreach (var address in AddressBook)
                    {
                        if (address.RepNumber == item.Number)
                            item.Number = address.Name;
                    }
                    SendList.Items.Add(item);
                }
                reader.Close();
            }
       

        }

        private void SendList_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }
    }
}