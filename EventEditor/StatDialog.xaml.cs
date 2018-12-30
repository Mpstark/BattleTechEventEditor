using System.Windows;

namespace EventEditor
{
    public partial class StatDialog
    {
        public Stat Stat;

        public StatDialog(Stat stat = null)
        {
            InitializeComponent();

            Stat = new Stat();

            if (stat != null)
            {
                Stat.set = stat.set;
                Stat.name = stat.name;
                Stat.typeString = stat.typeString;
                Stat.value = stat.value;
                Stat.valueConstant = stat.valueConstant;
            }

            DataContext = Stat;
        }

        private void Accept_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
