﻿using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Regular.Enums;
using Regular.Models;

namespace Regular.Utilities
{
    public static class RuleExecutionUtils
    {
        public static List<Element> GetTargetedElements(string documentGuid, string ruleGuid)
        {
            RegexRule regexRule = RegularApp.RegexRuleCacheService.GetRegexRule(documentGuid, ruleGuid);
            if (regexRule == null) return null;

            Document document = RegularApp.DocumentCacheService.GetDocument(documentGuid);

            ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(
                regexRule.TargetCategoryObjects
                .Where(x => x.IsChecked)
                .Select(x => Category.GetCategory(document, new ElementId(x.CategoryObjectId)))
                .Select(CategoryUtils.GetBuiltInCategoryFromCategory)
                .ToList());

            List<Element> targetedElements = new FilteredElementCollector(document)
                .WhereElementIsNotElementType()
                .WherePasses(elementMulticategoryFilter)
                .Where(x => x.GroupId == ElementId.InvalidElementId)
                .ToList();

            return targetedElements;
        }
        
        public static RuleValidationResult ExecuteRegexRule(string documentGuid, string ruleGuid, Element element)
        {
            RegexRule regexRule = RegularApp.RegexRuleCacheService.GetRegexRule(documentGuid, ruleGuid);
            if (regexRule == null) return RuleValidationResult.NotApplicable;
            string parameterValue = ParameterUtils.GetTrackingParameterValue(documentGuid, ruleGuid, element);
            if (string.IsNullOrWhiteSpace(parameterValue)) return RuleValidationResult.NotApplicable;
            string regexString = regexRule.RegexString;
            if (string.IsNullOrWhiteSpace(regexString)) return RuleValidationResult.NotApplicable;
            Regex regex = new Regex(regexRule.RegexString);
            return regex.IsMatch(parameterValue) ? RuleValidationResult.Valid : RuleValidationResult.Invalid;
        }
    }
}
