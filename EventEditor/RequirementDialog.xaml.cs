using System.Windows;
using System.Windows.Input;

namespace EventEditor
{
    public partial class RequirementDialog
    {
        public RequirementDef Requirement;

        public RequirementDialog(RequirementDef requirement = null)
        {
            InitializeComponent();

            Requirement = new RequirementDef();

            if (requirement != null)
            {
                Requirement.Scope = requirement.Scope;
                Requirement.RequirementTags = requirement.RequirementTags;
                Requirement.ExclusionTags = requirement.ExclusionTags;
                Requirement.RequirementComparisons = requirement.RequirementComparisons;
            }

            DataContext = Requirement;
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
    }
}
