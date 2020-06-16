﻿using System.ComponentModel;

namespace Regular.Enums
{
    public enum RuleType
    {
        [Description("Any Letter (A-Z)")]
        AnyLetter,
        [Description("Any Digit (0-9)")]
        AnyDigit,
        [Description("Free Text")]
        FreeText,
        [Description("Selection Set")]
        SelectionSet
    }
}