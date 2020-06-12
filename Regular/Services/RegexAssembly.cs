﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Regular.Model;
using Regular.ViewModel;

namespace Regular.Services
{
    public static class RegexAssembly
    {
        private static readonly List<string> SpecialCharacters = new List<string>() { @".", @"\", @"*", @"+", @"?", @"|", @"(", @")", @"[", @"]", @"^", @"{", @"}" };
        private static string GetRegexPartFromRuleType(RegexRulePart regexRulePart)
        {
            // TODO: Need to handle case sensitive and optional booleans.
            // For optional we can append ? to each returned string.
            // For non case-sensitive (i.e. case match) we can append the (?i) modifier after the string
            switch (regexRulePart.RuleType)
            {
                case RuleTypes.FreeText:
                    return SanitizeWord(regexRulePart.RawUserInputValue);
                case RuleTypes.SelectionSet:
                    // We'll need to break these up somehow
                    return "Test";
                case RuleTypes.AnyLetter:
                    if (regexRulePart.IsCaseSensitive) { }
                    // How do we handle case-sensitive?
                    return @"[a-zA-Z]";
                case RuleTypes.AnyDigit:
                    return @"\d";
                default:
                    return null;
            }
        }
        private static string SanitizeCharacter(string character)
        {
            if (SpecialCharacters.Contains(character)) return $@"\{character}";
            return character;
        }
        private static string SanitizeWord(string word)
        {
            if (String.IsNullOrEmpty(word)) return word;
            string outputString = "";
            foreach(char character in word)
            {
                outputString += SanitizeCharacter(character.ToString());
            }
            return outputString;
        }
        public static string AssembleRegexString(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            string regexString = "";
            foreach(RegexRulePart regexRulePart in regexRuleParts) { regexString += GetRegexPartFromRuleType(regexRulePart); }
            // TODO: We need to finalize the regex string. 
            // Are we using a re.match or a re.search? contain $ or ^? Or we could dynamically switch out the method
            return regexString;
        }

        public static char[] Letters = new[] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l','m', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        public static int[] Numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public static string GenerateRandomExample(ObservableCollection<RegexRulePart> regexRuleParts)
        {
            Random random = new Random();
            string randomExampleString = "Example: ";
            foreach (RegexRulePart regexRulePart in regexRuleParts)
            {
                switch (regexRulePart.RuleType)
                {
                    case RuleTypes.AnyLetter:
                        randomExampleString += Letters[random.Next(Letters.Length)];
                        break;
                    case RuleTypes.AnyDigit:
                        randomExampleString += Numbers[random.Next(Numbers.Length)];
                        break;
                    case RuleTypes.FreeText:
                        randomExampleString += regexRulePart.RawUserInputValue;
                        break;
                    case RuleTypes.SelectionSet:
                        randomExampleString += regexRulePart.RawUserInputValue;
                        break;
                    default:
                        randomExampleString += "";
                        break;
                }
            }
            return randomExampleString;
        }
    }
}
