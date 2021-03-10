﻿using System;
using System.Linq;
using System.Windows.Input;
using Regular.Enums;
using Regular.Models;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class SubmitRuleCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public SubmitRuleCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }
        public bool CanExecute(object parameter)
        {
            RegexRule stagingRule = ruleEditorViewModel.StagingRule;

            bool ruleNameLengthValid = stagingRule.RuleName.Length > 0;
            bool regexRulePartsCountValid = stagingRule.RegexRuleParts.Count > 0;
            bool regexStringLengthValid = !string.IsNullOrWhiteSpace(stagingRule.RegexString);
            bool targetCategoryObjectCountValid = stagingRule.TargetCategoryObjects.Count(x => x.IsChecked) > 0;
            bool trackingParameterObjectIdValid = stagingRule.TrackingParameterObject.ParameterObjectId != -1;

            // If any of the following cases is true, the new rule cannot be submitted
            return
            (
                ruleNameLengthValid &&
                regexRulePartsCountValid &&
                regexStringLengthValid &&
                targetCategoryObjectCountValid &&
                trackingParameterObjectIdValid
            );
        }

        public void Execute(object parameter)
        {
            switch (ruleEditorViewModel.RuleEditorInfo.RuleEditorType)
            {
            case RuleEditorType.CreateNewRule:
                RegexRule.Save
                (
                    ruleEditorViewModel.RuleEditorInfo.DocumentGuid,
                    ruleEditorViewModel.StagingRule
                );
                break;
            case RuleEditorType.EditingExistingRule:
                RegexRule.Update
                (
                    ruleEditorViewModel.RuleEditorInfo.DocumentGuid,
                    ruleEditorViewModel.InputRule,
                    ruleEditorViewModel.StagingRule
                );
                break;
            case RuleEditorType.DuplicateExistingRule:
                RegexRule.Save
                (
                    ruleEditorViewModel.RuleEditorInfo.DocumentGuid,
                    ruleEditorViewModel.StagingRule
                );
                break;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
