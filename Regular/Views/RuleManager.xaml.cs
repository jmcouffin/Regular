﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Regular.Models;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Regular.Views
{
    /// <summary>
    /// Interaction logic for RuleManager.xaml
    /// </summary>
    public partial class RuleManager : Window
    {
        public RuleManager(List<Entity> entityList)
        {
            InitializeComponent();
            ResultsListBox.ItemsSource = regularRegexRules;
        }

        private ObservableCollection<RegexRule> regularRegexRules = new ObservableCollection<RegexRule>();

    }
}
