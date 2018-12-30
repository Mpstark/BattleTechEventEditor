using System.Windows;

namespace EventEditor
{
    public partial class ForcedEventDialog
    {
        public ForcedEvent ForceEvent;

        public ForcedEventDialog(ForcedEvent forced = null)
        {
            InitializeComponent();

            ForceEvent = new ForcedEvent();

            if (forced != null)
            {
                ForceEvent.Scope = forced.Scope;
                ForceEvent.EventID = forced.EventID;
                ForceEvent.MinDaysWait = forced.MinDaysWait;
                ForceEvent.MaxDaysWait = forced.MaxDaysWait;
                ForceEvent.Probability = forced.Probability;
                ForceEvent.RetainPilot = forced.RetainPilot;
            }

            DataContext = ForceEvent;
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
