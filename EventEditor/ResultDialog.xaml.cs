using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace EventEditor
{
    public partial class ResultDialog
    {
        public EventResult Results;

        public ResultDialog(EventResult results = null)
        {
            InitializeComponent();

            Results = new EventResult();

            if (results != null)
            {
                Results.Scope = results.Scope;
                Results.ResultDuration = results.ResultDuration;
                Results.TemporaryResult = results.TemporaryResult;
                Results.Requirements = results.Requirements;
                Results.AddedTags = results.AddedTags;
                Results.RemovedTags = results.RemovedTags;
                Results.EditorItems = results.EditorItems;
            }

            DataContext = Results;
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

        private void TextBox_EnterClears_OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AcceptButton.Focus();
            }
        }

        // Only numbers allowed
        private readonly Regex _regex = new Regex("[^0-9]+");
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }


        private void AddResultItem_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void EditResultItem_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void RemoveResultItem_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ResultItems_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
