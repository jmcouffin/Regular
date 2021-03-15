﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Regular.Annotations;
using Regular.Enums;
using Regular.Utilities;

namespace Regular.Models
{
    public class RuleValidationOutput : INotifyPropertyChanged
    {
        // Private Members & Defaults
        private string validationText = Enums.ValidationResult.Invalid.GetEnumDescription();
        private string compliantExample = "";
        private string trackingParameterValue = "";
        
        // Public Members & NotifyPropertyChanged
        public ElementId ElementId { get; } = ElementId.InvalidElementId;
        public string ElementName { get; } = "";
        public string ValidationText
        {
            get => validationText;
            set
            {
                validationText = value;
                NotifyPropertyChanged();
            }
        }
        public string CompliantExample
        {
            get => compliantExample;
            set
            {
                compliantExample = value;
                NotifyPropertyChanged();
            }
        }
        public string TrackingParameterValue
        {
            get => trackingParameterValue;
            set
            {
                trackingParameterValue = value;
                NotifyPropertyChanged();
            }
        }

        public ValidationResult ValidationResult { get; set; } = ValidationResult.Invalid;

        public RuleValidationOutput(RuleValidationInfo ruleValidationInfo)
        {
            ElementId = ruleValidationInfo.Element.Id;
            ElementName = ruleValidationInfo.Element.Name;

            ValidationResult = RuleExecutionUtils.ExecuteRegexRule
            (
                ruleValidationInfo.DocumentGuid,
                ruleValidationInfo.RegexRule.RuleGuid,
                ruleValidationInfo.Element
            );
            
            validationText = ValidationResult.GetEnumDescription();
            
            if(ValidationResult == ValidationResult.Invalid)
            {
                CompliantExample = RegexAssemblyUtils.GenerateRandomExample(ruleValidationInfo.RegexRule.RegexRuleParts);
            }
            
            TrackingParameterValue = ParameterUtils.GetTrackingParameterValue
            (
                ruleValidationInfo.DocumentGuid,
                ruleValidationInfo.RegexRule.RuleGuid,
                ruleValidationInfo.Element
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}