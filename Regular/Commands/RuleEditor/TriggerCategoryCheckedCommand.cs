﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Regular.Models;
using Regular.Utilities;
using Regular.ViewModels;

namespace Regular.Commands.RuleEditor
{
    public class TriggerCategoryCheckedCommand : ICommand
    {
        private readonly RuleEditorViewModel ruleEditorViewModel;

        public TriggerCategoryCheckedCommand(RuleEditorViewModel ruleEditorViewModel)
        {
            this.ruleEditorViewModel = ruleEditorViewModel;
        }

        public bool CanExecute(object parameter) => ruleEditorViewModel.CategoriesPanelExpanded;

        public void Execute(object parameter)
        {
            // Lets the user multi-select and multi-deselect items
            if (!(parameter is CheckBox checkBox)) return;
            ListBox categoriesListBox = VisualTreeUtils.FindParent<ListBox>(checkBox);
            List<CategoryObject> selectedItems = categoriesListBox.SelectedItems.Cast<CategoryObject>().ToList();

            if(selectedItems.Count > 1)
            {
                foreach (CategoryObject categoryObject in selectedItems) categoryObject.IsChecked = checkBox.IsChecked == true;
            }

            // We update the number of categories ticked
            ruleEditorViewModel.NumberCategoriesSelected = ruleEditorViewModel.StagingRule.TargetCategoryObjects
                .Count(x => x.IsChecked);
            
            // And need to ascertain which parameters are now valid for the selection
            ruleEditorViewModel.PossibleTrackingParameterObjects = ParameterUtils.
            GetParametersOfCategories
            (
                ruleEditorViewModel.DocumentGuid,
                ruleEditorViewModel.StagingRule.TargetCategoryObjects
            );

            // Refreshing the list of possible tracking parameter objects
            ParameterObject selectedTrackingParameterObject = ruleEditorViewModel.StagingRule.TrackingParameterObject;
            
            // If the parameter is still available after categories selection has ended, we don't need to change anything
            if (selectedTrackingParameterObject.ParameterObjectId != -1 && ruleEditorViewModel.PossibleTrackingParameterObjects.Contains(selectedTrackingParameterObject)) return;
            
            // However if the previously selected tracking parameter is no longer available, we'll default to the first item in the list
            if (selectedTrackingParameterObject.ParameterObjectId == -1 && ruleEditorViewModel.PossibleTrackingParameterObjects.Count > 0)
            {
                ruleEditorViewModel.StagingRule.TrackingParameterObject = ruleEditorViewModel.PossibleTrackingParameterObjects.First();
                return;
            }

            // Updating the tracking parameter combobox prompt
            if (ruleEditorViewModel.NumberCategoriesSelected == 0)
            {
                ruleEditorViewModel.ComboBoxTrackingParameterText = "Select Categories";
            }
            else if (ruleEditorViewModel.PossibleTrackingParameterObjects == null || ruleEditorViewModel.PossibleTrackingParameterObjects.Count < 1)
            {
                ruleEditorViewModel.ComboBoxTrackingParameterText = "No Common Parameters Found";
            }
            else
            {
                ruleEditorViewModel.ComboBoxTrackingParameterText = "ERROR";
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
