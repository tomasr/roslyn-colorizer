using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace RoslynColorizer {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.ParameterFormat)]
  [Name(Constants.ParameterFormat)]
  [UserVisible(true)]
  [Order(After = Priority.Default)]
  internal sealed class RoslynParameterFormat : ClassificationFormatDefinition {
    public RoslynParameterFormat() {
      this.DisplayName = "Roslyn Parameter";
      this.ForegroundColor = Colors.SlateGray;
    }
  }

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Constants.FieldFormat)]
  [Name(Constants.FieldFormat)]
  [UserVisible(true)]
  [Order(After = Priority.Default)]
  internal sealed class RoslynFieldFormat : ClassificationFormatDefinition {
    public RoslynFieldFormat() {
      this.DisplayName = "Roslyn Field";
      this.ForegroundColor = Colors.SaddleBrown;
    }
  }
}
