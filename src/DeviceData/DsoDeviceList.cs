
namespace DSO138Device
{
    public class DsoDeviceList : INotify
    {
        private string name_;
        private string id_;

        public DsoDeviceList() { }
        public DsoDeviceList(string name, string id) {
            Name = name;
            Id = id;
        }
        public string Name
        {
            get { return name_;  }
            set
            {
                if (value != name_)
                {
                    name_ = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string Id
        {
            get { return id_; }
            set
            {
                if (value != id_)
                {
                    id_ = value;
                    OnPropertyChanged("Id");
                }
            }
        }
    }
}
