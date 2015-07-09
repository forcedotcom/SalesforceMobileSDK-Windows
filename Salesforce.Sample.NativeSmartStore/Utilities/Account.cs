using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Salesforce.Sample.NativeSmartStoreSample.utilities
{
    public class Account : INotifyPropertyChanged
    {

        private string _name;
        private string _id;
        private string _ownerId;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        public string OwnerId
        {
            get { return _ownerId; }
            set
            {
                _ownerId = value;
                NotifyPropertyChanged("OwnerId");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
