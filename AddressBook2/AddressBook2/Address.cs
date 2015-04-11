using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Address의 기본 클래스
namespace AddressBook2
{
    // TODO 클래스 내부에 List<String> Numbers를 만들어서
    //      전화번호 여러개를 저장할 수 있게 만들어야함
    class Address
    {
        public Address()
        {
            _Name = "";
            _RepNumber = "";
        }
        public Address(String name, String number)
        {
            _Name = name;
            _RepNumber = number;
        }
        public Address(MainWindow.User user)
        {
            _Name = user.Name;
            _RepNumber = user.Number;
        }
        private String _Name;
        private String _RepNumber;

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string RepNumber
        {
            get { return _RepNumber; }
            set { _RepNumber = value; }
        }
    }
}
