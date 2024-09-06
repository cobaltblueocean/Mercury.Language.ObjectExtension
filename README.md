# Mercury.Language.ObjectExtension

This package provides an extension method for the Object class. This extension method compares two Objects and evaluates to True if each property has the same value.
Normally, C# evaluates each Object reference and determines that they are "not the same" even if each property has the same value, but this extension method determines that they are "equivalent objects" if all properties have the same value.
