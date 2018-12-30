using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverQueried.Global

namespace EventEditor
{
    public enum EventPublishState
    {
        UNPUBLISHED,
        READY_FOR_REVIEW,
        PUBLISHED
    }

    public enum SimEventType
    {
        NORMAL,
        MORALE,
        UNSELECTABLE,
        FUNERAL
    }

    public enum EventScope
    {
        None = -1,
        Company,
        MechWarrior,
        Mech,
        Commander,
        StarSystem,
        SecondaryMechWarrior,
        SecondaryMech,
        AllMechWarriors,
        AllMechs,
        TertiaryMechWarrior,
        RandomMech,
        DeadMechWarrior,
        Map
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ComparisonDef
    {
        [JsonIgnore]
        private Regex formatRegex = new Regex(@"^([^=!><]+)([=!><]+)(.+)$");

        [JsonIgnore]
        private static readonly Dictionary<string, Operator> strToOp = new Dictionary<string, Operator>
        {
            {"==", Operator.Equal},
            {"!=", Operator.NotEqual},
            {"<", Operator.LessThan},
            {">", Operator.GreaterThan},
            {"<=", Operator.LessThanOrEqual},
            {">=", Operator.GreaterThanOrEqual}
        };

        public enum Operator
        {
            Equal,
            NotEqual,
            LessThan,
            GreaterThan,
            LessThanOrEqual,
            GreaterThanOrEqual
        }

        public string obj { get; set; }
        public Operator op { get; set; }
        public float val { get; set; }
        public string valueConstant => val.ToString(CultureInfo.InvariantCulture);

        public override string ToString()
        {
            var opString = "inv";

            switch (op)
            {
                case Operator.Equal:
                    opString = "==";
                    break;
                case Operator.NotEqual:
                    opString = "!=";
                    break;
                case Operator.LessThan:
                    opString = "<";
                    break;
                case Operator.GreaterThan:
                    opString = ">";
                    break;
                case Operator.LessThanOrEqual:
                    opString = "<=";
                    break;
                case Operator.GreaterThanOrEqual:
                    opString = ">=";
                    break;
            }

            return $"{obj} {opString} {val}";
        }

        public bool FromString(string str)
        {
            var match = formatRegex.Match(str);

            if (!match.Success)
                return false;

            var _obj = match.Groups[1].Value;
            var _op = match.Groups[2].Value;
            var _val = match.Groups[3].Value;

            if (!strToOp.ContainsKey(_op) || !float.TryParse(_val, out var _value))
                return false;

            obj = _obj.Trim();
            op = strToOp[_op];
            val = _value;

            return true;
        }
    }

    public class EventObject
    {
        public EventScope Scope;
        public RequirementDef Requirements = new RequirementDef();

        public override string ToString()
        {
            return $"{Requirements}";
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Stat : ResultItem
    {
        public string typeString;
        public string name;
        public string value;
        public bool set;
        public string valueConstant;

        public override string ToString()
        {
            var op = "+";
            var val = value;

            if (set)
            {
                op = "=";
            }
            else if (float.TryParse(value, out var num) && num < 0)
            {
                op = "-";
                val = (num * -1).ToString(CultureInfo.InvariantCulture);
            }

            return $"{name} {op} {val}";
        }
    }

    public class DescriptionDef
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Icon { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class TagSet
    {
        public string tagSetSourceFile { get; set; }
        public HashSet<string> items { get; set; } = new HashSet<string>();

        [JsonIgnore]
        public string EditorItems
        {
            get
            {
                if (items == null || items.Count == 0)
                    return "";

                return string.Join(", ", items);
            }
            set
            {
                var strs = value.Split(',');
                items = new HashSet<string>();

                foreach (var str in strs)
                {
                    var trimmed = str.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed) && !items.Contains(trimmed))
                        items.Add(trimmed);
                }
            }
        }

        public override string ToString()
        {
            if (items == null || items.Count == 0)
                return "";

            return string.Join(", ", items);
        }
    }

    public class RequirementDef
    {
        public EventScope Scope { get; set; }
        public TagSet RequirementTags { get; set; } = new TagSet();
        public TagSet ExclusionTags { get; set; } = new TagSet();
        public List<ComparisonDef> RequirementComparisons { get; set; } = new List<ComparisonDef>();

        [JsonIgnore]
        public IList<EventScope> EventScopes => Enum.GetValues(typeof(EventScope)).Cast<EventScope>().ToList();

        [JsonIgnore]
        public string EditorComparisons
        {
            get
            {
                if (RequirementComparisons == null || RequirementComparisons.Count == 0)
                    return "";

                return string.Join(", ", RequirementComparisons);
            }
            set
            {
                var strs = value.Split(',');
                RequirementComparisons = new List<ComparisonDef>();

                foreach (var str in strs)
                {
                    var trimmed = str.Trim();

                    if (string.IsNullOrWhiteSpace(trimmed))
                        continue;

                    var comparison = new ComparisonDef();
                    if (comparison.FromString(trimmed))
                        RequirementComparisons.Add(comparison);
                }
            }
        }

        public override string ToString()
        {
            var str = $"{Scope}\t";

            if (RequirementComparisons != null && RequirementComparisons.Count > 0)
                str += $" [{string.Join(", ", RequirementComparisons)}]";

            if (RequirementTags.items != null && RequirementTags.items.Count > 0)
                str += $" +[{RequirementTags}]";

            if (ExclusionTags.items != null && ExclusionTags.items.Count > 0)
                str += $" -[{ExclusionTags}]";

            if (str == $"{Scope}\t")
                str += " ANY";

            return str;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class EventResultAction : ResultItem
    {
        public enum ActionType
        {
            MechWarrior_AddRoster,
            MechWarrior_AddHiring,
            MechWarrior_Kill,
            MechWarrior_Fire,
            MechWarrior_Callsign,
            MechWarrior_HealAll,
            MechWarrior_SetTimeout,
            Mech_RepairAll,
            Mech_Damage_Location,
            Mech_AddRoster,
            Contract_IgnoreMissionResults,
            Company_TravelTime,
            Company_TravelTo,
            Equipment_AddRandom_POSTLAUNCH,
            Ship_SetRepairState,
            StarSystem_SetActiveDef,
            System_PlayVideo,
            System_StartContract,
            System_AddContract,
            System_StartNonProceduralContract,
            System_CreateCommander,
            System_SimGameCharacterVisible,
            System_UpdateMilestones,
            System_SetContractScope,
            System_SetDropship,
            System_SetTargetBreadcrumbSystem,
            System_SetObjective,
            System_StartConversation,
            System_AddStartingRoster,
            System_PauseNotification,
            System_StartCredits,
            System_ResetContracts,
            System_ShowCampaignResults,
            System_ShowTitleCard,
            StarSystem_SetCurBreadcrumbOverride,
            Ship_AddUpgrade,
            System_ToggleIgnoredContractTargets,
            System_ToggleIgnoredContractEmployers,
            System_AddDisplayedFaction,
            System_RemoveDisplayedFaction
        }

        public ActionType Type;
        public string value { get; set; }
        public string valueConstant { get; set; }
        public string[] additionalValues { get; set; }

        public override string ToString()
        {
            var str = $"{Type}";

            if (!string.IsNullOrWhiteSpace(value))
                str += $" {value}";

            if (additionalValues != null && additionalValues.Length > 0)
                str += $" {string.Join(", ", additionalValues)}";

            return str;
        }
    }

    public class ForcedEvent : ResultItem
    {
        public EventScope Scope { get; set; }
        public string EventID { get; set; }
        public int MinDaysWait { get; set; }
        public int MaxDaysWait { get; set; }
        public int Probability { get; set; }
        public bool RetainPilot { get; set; }

        public override string ToString()
        {
            return $"Force Event -- {Scope}:{EventID} ({Probability}) in [{MinDaysWait} - {MaxDaysWait}] days";
        }
    }

    public class ResultItem {}

    public class EventResult
    {
        public EventScope Scope { get; set; }
        public bool TemporaryResult { get; set; }
        public int ResultDuration { get; set; }

        public RequirementDef Requirements { get; set; }

        public TagSet AddedTags { get; set; }
        public TagSet RemovedTags { get; set; }

        [JsonIgnore]
        public ObservableCollection<ResultItem> EditorItems { get; set; } = new ObservableCollection<ResultItem>();

        public Stat[] Stats
        {
            get => EditorItems.OfType<Stat>().ToArray();
            set
            {
                if (value == null || value.Length <= 0)
                    return;

                foreach (var stat in value)
                {
                    EditorItems.Add(stat);
                }
            }
        }
        public EventResultAction[] Actions
        {
            get => EditorItems.OfType<EventResultAction>().ToArray();
            set
            {
                if (value == null || value.Length <= 0)
                    return;

                foreach (var action in value)
                {
                    EditorItems.Add(action);
                }
            }
        }
        public ForcedEvent[] ForceEvents
        {
            get => EditorItems.OfType<ForcedEvent>().ToArray();
            set
            {
                if (value == null || value.Length <= 0)
                    return;

                foreach (var forcedEvent in value)
                {
                    EditorItems.Add(forcedEvent);
                }
            }
        }

        public override string ToString()
        {
            var str = $"{Scope}";

            if (AddedTags.items != null && AddedTags.items.Count > 0)
                str += $" +[{AddedTags}]";

            if (RemovedTags.items != null && RemovedTags.items.Count > 0)
                str += $" -[{RemovedTags}]";

            if (EditorItems != null && EditorItems.Count > 0)
            {
                foreach (var item in EditorItems)
                {
                    str += $" [{item}]";
                }
            }

            if (TemporaryResult)
                str += $" (Temp: {ResultDuration} days)";

            return str;
        }

        [JsonIgnore]
        public IList<EventScope> EventScopes => Enum.GetValues(typeof(EventScope)).Cast<EventScope>().ToList();
    }

    public class EventResultSet
    {
        public DescriptionDef Description { get; set; } = new DescriptionDef();
        public int Weight { get; set; }
        public ObservableCollection<EventResult> Results { get; set; } = new ObservableCollection<EventResult>();

        public override string ToString()
        {
            return $"({Results.Count}) {Description.Name} ({Weight})";
        }
    }

    public class EventOption
    {
        public DescriptionDef Description { get; set; } = new DescriptionDef();
        public ObservableCollection<RequirementDef> RequirementList { get; set; } = new ObservableCollection<RequirementDef>();
        public ObservableCollection<EventResultSet> ResultSets { get; set; } = new ObservableCollection<EventResultSet>();

        public override string ToString()
        {
            return $"({ResultSets.Count}) {Description.Details}: {Description.Name}";
        }
    }

    public class EventDef
    {
        public SimEventType EventType { get; set; }
        public EventPublishState PublishState { get; set; }
        public EventScope Scope { get; set; }
        public DescriptionDef Description { get; set; } = new DescriptionDef();
        public int Weight { get; set; }

        public ObservableCollection<EventObject> AdditionalObjects { get; set; } = new ObservableCollection<EventObject>();

        [JsonIgnore]
        public ObservableCollection<RequirementDef> EditorRequirements { get; set; } = new ObservableCollection<RequirementDef>();

        public RequirementDef Requirements
        {
            get => EditorRequirements.Count > 0 ? EditorRequirements[0] : null;
            set
            {
                if (EditorRequirements.Count > 0)
                {
                    EditorRequirements[0] = value;
                }
                else
                {
                    EditorRequirements.Add(value);
                }
            }
        }
        public RequirementDef[] AdditionalRequirements
        {
            get => EditorRequirements.ToList().GetRange(1, EditorRequirements.Count - 1).ToArray();
            set
            {
                RequirementDef requirement = null;

                if (EditorRequirements.Count > 0)
                    requirement = EditorRequirements[0];

                EditorRequirements = new ObservableCollection<RequirementDef> {requirement};

                foreach (var requirementDef in value)
                    EditorRequirements.Add(requirementDef);
            }
        }

        public ObservableCollection<EventOption> Options { get; set; } = new ObservableCollection<EventOption>();

        [JsonIgnore]
        public IList<SimEventType> EventTypes => Enum.GetValues(typeof(SimEventType)).Cast<SimEventType>().ToList();

        [JsonIgnore]
        public IList<EventScope> EventScopes => Enum.GetValues(typeof(EventScope)).Cast<EventScope>().ToList();
    }
}
