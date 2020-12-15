﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Regular.Models;

namespace Regular.Services
{
    public static class DmTriggerServices
    {
        internal static void AddAllTriggers(string documentGuid, ObservableCollection<RegexRule> regexRules)
        {
            foreach (RegexRule regexRule in regexRules) AddTrigger(documentGuid, regexRule);
        }
        
        public static void AddTrigger(string documentGuid, RegexRule regexRule)
        {
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);

            List<ElementId> targetCategoryIds = regexRule.TargetCategoryObjects.Select(x => new ElementId(Convert.ToInt32(x.CategoryObjectId))).ToList();
            List<Category> targetCategories = targetCategoryIds.Select(x => Category.GetCategory(document, x)).ToList();
            List<BuiltInCategory> targetBuiltInCategories = targetCategories.Select(CategoryServices.GetBuiltInCategoryFromCategory).ToList();
            ElementId trackingParameterId = new ElementId(regexRule.TrackingParameterObject.ParameterObjectId);

            UpdaterRegistry.AddTrigger(
                DmUpdaters.AllUpdaters[documentGuid].GetUpdaterId(),
                document,
                new ElementMulticategoryFilter(targetBuiltInCategories),
                Element.GetChangeTypeParameter(trackingParameterId));
        }

        public static void DeleteTrigger(string documentGuid, RegexRule ruleToRemoveTriggerFrom)
        {
            // There is no way to delete a specific trigger so we must remove all document-based triggers
            // and recreate them minus the one trigger we're removing.
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
            UpdaterId updaterId = DmUpdaters.AllUpdaters[documentGuid].GetUpdaterId();
            UpdaterRegistry.RemoveDocumentTriggers(updaterId, document);
            foreach (RegexRule regexRule in RegexRules.AllRegexRules[documentGuid])
            {
                // We don't add back the trigger for the rule we're removing
                if (regexRule.RuleGuid == ruleToRemoveTriggerFrom.RuleGuid) continue;
                AddTrigger(documentGuid, regexRule);
            }
        }

        public static void UpdateAllTriggers(string documentGuid)
        {
            // There is no way to delete a specific trigger so we must remove all document-based triggers
            // and recreate all of them one by one, but with the new RegexRuleInfo
            Document document = DocumentGuidServices.GetRevitDocumentByGuid(documentGuid);
            UpdaterId updaterId = DmUpdaters.AllUpdaters[documentGuid].GetUpdaterId();
            UpdaterRegistry.RemoveDocumentTriggers(updaterId, document);
            foreach (RegexRule regexRule in RegexRules.AllRegexRules[documentGuid])
            {
                // We recreate all of the triggers
                AddTrigger(documentGuid, regexRule);
            }
        }
    }
}
