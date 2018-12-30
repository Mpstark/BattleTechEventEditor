using System.Windows;
using System.Windows.Input;

namespace EventEditor
{
    public partial class OptionDialog
    {
        public EventOption Option;

        public OptionDialog(EventOption option = null)
        {
            InitializeComponent();

            Option = new EventOption();

            if (option != null)
            {
                Option.Description = option.Description;
                Option.RequirementList = option.RequirementList;
                Option.ResultSets = option.ResultSets;
            }

            DataContext = Option;
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


        // Requirements
        private void AddRequirement_OnClick(object sender, RoutedEventArgs e)
        {
            var requirementDialog = new RequirementDialog { Owner = this };
            if (requirementDialog.ShowDialog() != true)
                return;

            Option.RequirementList.Add(requirementDialog.Requirement);
            RequirementListBox.SelectedItem = requirementDialog.Requirement;
        }

        private void EditRequirement_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(RequirementListBox.SelectedItem is RequirementDef selectedObject))
                return;

            var requirementDialog = new RequirementDialog(selectedObject) { Owner = this };
            if (requirementDialog.ShowDialog() != true)
                return;

            selectedObject.Scope = requirementDialog.Requirement.Scope;
            selectedObject.RequirementTags = requirementDialog.Requirement.RequirementTags;
            selectedObject.ExclusionTags = requirementDialog.Requirement.ExclusionTags;
            selectedObject.RequirementComparisons = requirementDialog.Requirement.RequirementComparisons;

            RequirementListBox.Items.Refresh();
        }

        private void RemoveRequirement_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(RequirementListBox.SelectedItem is RequirementDef selectedObject))
                return;

            Option.RequirementList.Remove(selectedObject);
        }

        private void Requirements_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is RequirementDef)
            {
                EditRequirement_OnClick(sender, new RoutedEventArgs());
            }
        }


        // Results
        private void AddResult_OnClick(object sender, RoutedEventArgs e)
        {
            var resultDialog = new ResultSetDialog { Owner = this };
            if (resultDialog.ShowDialog() != true)
                return;

            Option.ResultSets.Add(resultDialog.ResultSet);
            ResultsListBox.SelectedItem = resultDialog.ResultSet;
        }

        private void EditResult_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(ResultsListBox.SelectedItem is EventResultSet selectedObject))
                return;

            var resultDialog = new ResultSetDialog (selectedObject) { Owner = this };
            if (resultDialog.ShowDialog() != true)
                return;

            selectedObject.Description = resultDialog.ResultSet.Description;
            selectedObject.Results = resultDialog.ResultSet.Results;
            selectedObject.Weight = resultDialog.ResultSet.Weight;

            ResultsListBox.Items.Refresh();
        }

        private void RemoveResult_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(ResultsListBox.SelectedItem is EventResultSet selectedObject))
                return;

            Option.ResultSets.Remove(selectedObject);
        }

        private void Results_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is EventResultSet)
            {
                EditResult_OnClick(sender, new RoutedEventArgs());
            }
        }
    }
}
