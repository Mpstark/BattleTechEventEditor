using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace EventEditor
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitEventDef();
        }


        // EventDef that we're editing
        private EventDef _openEvent;
        private void InitEventDef(string path = null)
        {
            if (path != null)
            {
                var json = File.ReadAllText(path);
                _openEvent = JsonConvert.DeserializeObject<EventDef>(json);
            }

            if (path == null || _openEvent == null)
            {
                _openEvent = new EventDef()
                {
                    Description =
                    {
                        Name = "Event Name",
                        Details = "This appears in the body of the event. Put the story and context here.",
                        Icon = "uixTxrSpot_DailyBriefing.png"
                    },
                    Weight = 10,
                    Requirements = new RequirementDef { Scope = EventScope.Company }
                };
            }

            DataContext = _openEvent;
        }


        // Commands
        private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InitEventDef();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"};

            if (openFileDialog.ShowDialog() != true)
                return;

            InitEventDef(openFileDialog.FileName);
            Title = $"BattleTech Event Editor -- [{Path.GetFileName(openFileDialog.FileName)}]";
        }


        // Only numbers allowed
        private readonly Regex _regex = new Regex("[^0-9]+");
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }


        // Requirements
        private void AddRequirement_OnClick(object sender, RoutedEventArgs e)
        {
            var requirementDialog = new RequirementDialog {Owner = this};
            if (requirementDialog.ShowDialog() != true)
                return;

            _openEvent.EditorRequirements.Add(requirementDialog.Requirement);
            RequirementListBox.SelectedItem = requirementDialog.Requirement;
        }

        private void EditRequirement_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(RequirementListBox.SelectedItem is RequirementDef selectedObject))
                return;

            var requirementDialog = new RequirementDialog(selectedObject) {Owner = this};
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

            _openEvent.EditorRequirements.Remove(selectedObject);
        }

        private void Requirements_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is RequirementDef)
            {
                EditRequirement_OnClick(sender, new RoutedEventArgs());
            }
        }


        // Additional Objects
        private void AddAdditionalObject_OnClick(object sender, RoutedEventArgs e)
        {
            var requirementDialog = new RequirementDialog { Owner = this };
            if (requirementDialog.ShowDialog() != true)
                return;

            var newObject = new EventObject
            {
                Scope = requirementDialog.Requirement.Scope,
                Requirements = requirementDialog.Requirement
            };

            _openEvent.AdditionalObjects.Add(newObject);
            AdditionalObjectsListBox.SelectedItem = newObject;
        }

        private void EditAdditionalObject_OnClick(object sender, RoutedEventArgs e)
        {
            // get the selected eventobject from the additional objects list box
            var selectedObject = AdditionalObjectsListBox.SelectedItem as EventObject;
            if (selectedObject == null)
                return;

            var requirementDialog = new RequirementDialog(selectedObject.Requirements) { Owner = this };
            if (requirementDialog.ShowDialog() != true)
                return;

            selectedObject.Requirements = requirementDialog.Requirement;
            selectedObject.Scope = requirementDialog.Requirement.Scope;

            AdditionalObjectsListBox.Items.Refresh();
        }

        private void RemoveAdditionalObject_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(AdditionalObjectsListBox.SelectedItem is EventObject selectedObject))
                return;

            _openEvent.AdditionalObjects.Remove(selectedObject);
        }

        private void AdditionalObjects_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is EventObject)
            {
                EditAdditionalObject_OnClick(sender, new RoutedEventArgs());
            }
        }


        // Options
        private void AddOption_OnClick(object sender, RoutedEventArgs e)
        {
            var optionDialog = new OptionDialog() { Owner = this };
            if (optionDialog.ShowDialog() != true)
                return;

            _openEvent.Options.Add(optionDialog.Option);
            OptionsListBox.SelectedItem = optionDialog.Option;
        }

        private void EditOption_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(OptionsListBox.SelectedItem is EventOption selectedObject))
                return;

            var optionDialog = new OptionDialog(selectedObject) { Owner = this };
            if (optionDialog.ShowDialog() != true)
                return;

            selectedObject.Description = optionDialog.Option.Description;
            selectedObject.RequirementList = optionDialog.Option.RequirementList;
            selectedObject.ResultSets = optionDialog.Option.ResultSets;

            OptionsListBox.Items.Refresh();
        }

        private void RemoveOption_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(OptionsListBox.SelectedItem is EventOption selectedObject))
                return;

            _openEvent.Options.Remove(selectedObject);
        }

        private void Options_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is EventOption)
            {
                EditOption_OnClick(sender, new RoutedEventArgs());
            }
        }
    }
}
