﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Regular.Enums;
using Regular.Models;
using Regular.UI.RuleEditor.ViewModel;
using Regular.Utilities;

namespace Regular.UI.RuleEditor.View
{
    public partial class RuleEditorView
    {
        public RuleEditorViewModel RuleEditorViewModel { get; set; }
        public RuleEditorView(RuleEditorInfo ruleEditorInfo)
        {
            InitializeComponent();
            RuleEditorViewModel = new RuleEditorViewModel(ruleEditorInfo);
            DataContext = RuleEditorViewModel;

            ComboBoxTrackingParameterInput.SelectedItem = RuleEditorViewModel.PossibleTrackingParameterObjects
                .FirstOrDefault(x => x.ParameterObjectId == RuleEditorViewModel.StagingRule.TrackingParameterObject.ParameterObjectId);

            TextBoxNameYourRuleInput.Focus();
        }
        private void ScrollViewerRuleParts_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        
        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();
        
        private void TextBoxNameYourRuleInput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Once touched, we can display an error message if the value is invalid
            RuleEditorViewModel.RuleNameInputDirty = true;
            RuleEditorViewModel.Title = $"{RuleEditorViewModel.TitlePrefix}: {RuleEditorViewModel.StagingRule.RuleName}";

            // Gathering other RegexRule names to ensure the user inputs a unique name
            List<RegexRule> otherRegexRules = RegularApp.RegexRuleCacheService
                    .GetDocumentRules(RuleEditorViewModel.RuleEditorInfo.DocumentGuid)
                    .Where(x => x.IsStagingRule == false)
                    .ToList();

            string ruleNameInputFeedback = InputValidationServices.ValidateRuleName(RuleEditorViewModel.StagingRule, otherRegexRules);

            // Sets ellipse colours and feedback visibility
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length < 1)
            {
                if (RuleEditorViewModel.RuleNameInputDirty)
                {
                    EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorRed"];
                    RuleEditorViewModel.UserFeedbackText = ruleNameInputFeedback;
                    RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Visible;
                }
                else
                {
                    EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGray"];
                    RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Hidden;
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(ruleNameInputFeedback))
            {
                EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorGreen"];
                RuleEditorViewModel.UserFeedbackText = "";
                RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Hidden;
            }
            else
            {
                EllipseNameYourRuleInput.Fill = (SolidColorBrush)this.Resources["EllipseColorRed"];
                RuleEditorViewModel.UserFeedbackText = ruleNameInputFeedback;
                RuleEditorViewModel.UserFeedbackTextVisibility = Visibility.Visible;
            }
        }
        
        private void RuleEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            RuleEditorViewModel.OutputParameterNameInputDirty = false;
            RuleEditorViewModel.RuleNameInputDirty = false;
        }

        private void ButtonControl_OnClick(object sender, RoutedEventArgs e)
        {
            // Jumping through hoops to determine if the sender RegexRulePart is 
            // of the CustomText kind
            if (!(sender is Button editButton)) return;
            if (!(editButton.DataContext is IRegexRulePart regexRulePart)) return;
            if (regexRulePart.RuleType != RuleType.CustomText) return;
            Grid grid = VisualTreeUtils.FindParent<Grid>(editButton);
            UIElementCollection uiElementCollection = grid.Children;
            TextBox textBox = uiElementCollection
                .OfType<TextBox>()
                .FirstOrDefault(x => x.Name == "RawUserInputValueTextBox");
            if (textBox == null) return;
            
            // Sets focus to the textbox and highlights and text found in it
            textBox.IsEnabled = true;
            textBox.Focus();
            textBox.Select(0, textBox.Text.Length);
        }

        private void RawUserInputValueTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            // Lets us close the window by hitting the Escape key
            PreviewKeyDown += (s, eventHandler) =>
            {
                if (eventHandler.Key != Key.Escape && eventHandler.Key != Key.Enter) return;
                ListBoxItem listBoxItem = VisualTreeUtils.FindParent<ListBoxItem>(textBox);
                listBoxItem?.Focus();
            };
            RuleEditorViewModel.GenerateCompliantExampleCommand.Execute(null);
        }

        // If the user is allowed to submit the rule, we close the window to prevent them 
        // creating the same rule as many times as they like
        private void ButtonOk_OnClick(object sender, RoutedEventArgs e) => Close();

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid)) return;
            UIElementCollection children = grid.Children;
            foreach (UIElement uiElement in children)
            {
                switch (uiElement)
                {
                    case Rectangle rectangle:
                    {
                        Color rolloutColor = (Color)ColorConverter.ConvertFromString("#E5E5E5");
                        rectangle.Fill = new SolidColorBrush(rolloutColor);
                        break;
                    }
                    case Button button:
                    {
                        Color rolloverColor = (Color) ColorConverter.ConvertFromString("#CCCCCC");
                        button.Background = new SolidColorBrush(rolloverColor);
                        button.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    }
                    case TextBox textBox:
                    {
                        Color rolloverColor = (Color)ColorConverter.ConvertFromString("#E5E5E5");
                        textBox.Background = new SolidColorBrush(rolloverColor);
                        break;
                    }
                }
            }
        }
        
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid)) return;
            UIElementCollection children = grid.Children;
            foreach (UIElement uiElement in children)
            {
                switch (uiElement)
                {
                    case Rectangle rectangle:
                    {
                        Color rolloverColor = (Color)ColorConverter.ConvertFromString("#CCCCCC");
                        rectangle.Fill = new SolidColorBrush(rolloverColor);
                        break;
                    }
                    case Button button:
                    {
                        Color rolloverColor = (Color) ColorConverter.ConvertFromString("#B2B2B2");
                        button.Background = new SolidColorBrush(rolloverColor);
                        break;
                    }
                    case TextBox textBox:
                    {
                        Color rolloverColor = (Color)ColorConverter.ConvertFromString("#CCCCCC");
                        textBox.Background = new SolidColorBrush(rolloverColor);
                        break;
                    }
                }
            }
        }

        private void RawUserInputValueTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.IsEnabled = false;
        }
    }
}
 