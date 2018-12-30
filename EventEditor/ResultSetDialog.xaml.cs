using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace EventEditor
{
    public partial class ResultSetDialog
    {
        public EventResultSet ResultSet;

        public ResultSetDialog(EventResultSet resultSet = null)
        {
            InitializeComponent();

            ResultSet = new EventResultSet();

            if (resultSet != null)
            {
                ResultSet.Description = resultSet.Description;
                ResultSet.Results = resultSet.Results;
                ResultSet.Weight = resultSet.Weight;
            }

            DataContext = ResultSet;
        }

        // Only numbers allowed
        private readonly Regex _regex = new Regex("[^0-9]+");
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void TextBox_EnterClears_OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AcceptButton.Focus();
            }
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

        // Results
        private void AddResult_OnClick(object sender, RoutedEventArgs e)
        {
            var resultDialog = new ResultDialog { Owner = this };
            if (resultDialog.ShowDialog() != true)
                return;

            ResultSet.Results.Add(resultDialog.Results);
            ResultsListBox.SelectedItem = resultDialog.Results;
        }

        private void EditResult_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(ResultsListBox.SelectedItem is EventResult selectedObject))
                return;

            var resultDialog = new ResultDialog(selectedObject) { Owner = this };
            if (resultDialog.ShowDialog() != true)
                return;

            selectedObject.Scope = resultDialog.Results.Scope;
            selectedObject.ResultDuration = resultDialog.Results.ResultDuration;
            selectedObject.TemporaryResult = resultDialog.Results.TemporaryResult;
            selectedObject.Requirements = resultDialog.Results.Requirements;
            selectedObject.AddedTags = resultDialog.Results.AddedTags;
            selectedObject.RemovedTags = resultDialog.Results.RemovedTags;
            selectedObject.EditorItems = resultDialog.Results.EditorItems;

            ResultsListBox.Items.Refresh();
        }

        private void RemoveResult_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(ResultsListBox.SelectedItem is EventResult selectedObject))
                return;

            ResultSet.Results.Remove(selectedObject);
        }

        private void Results_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is EventResult)
            {
                EditResult_OnClick(sender, new RoutedEventArgs());
            }
        }
    }
}
