using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace RoslynColorizer {
  internal static class ClassificationTypes {
    [Export(typeof(ClassificationTypeDefinition))]
    [Name(Constants.ParameterFormat)]
    internal static ClassificationTypeDefinition ParameterType = null;

    [Export(typeof(ClassificationTypeDefinition))]
    [Name(Constants.FieldFormat)]
    internal static ClassificationTypeDefinition FieldType = null;

    [Export(typeof(ClassificationTypeDefinition))]
    [Name(Constants.ExtensionMethodFormat)]
    internal static ClassificationTypeDefinition ExtensionMethodType = null;
  }
}
