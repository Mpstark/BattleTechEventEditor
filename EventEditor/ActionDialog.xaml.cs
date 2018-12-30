using System.Windows;

namespace EventEditor
{
    public partial class ActionDialog
    {
        public EventResultAction Result;

        public ActionDialog(EventResultAction result = null)
        {
            InitializeComponent();

            Result = new EventResultAction();

            if (result != null)
            {
                Result.value = result.value;
                Result.Type = result.Type;
                Result.additionalValues = result.additionalValues;
                Result.valueConstant = result.valueConstant;
            }

            DataContext = Result;
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
