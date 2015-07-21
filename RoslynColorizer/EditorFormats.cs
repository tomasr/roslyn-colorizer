using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace RoslynColorizer
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.FieldFormat)]
    [Name(Constants.FieldFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynFieldFormat : ClassificationFormatDefinition
    {
        public RoslynFieldFormat() {
            this.DisplayName = "Roslyn Field";
            this.ForegroundColor = Colors.SaddleBrown;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.EnumFieldFormat)]
    [Name(Constants.EnumFieldFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynEnumFieldFormat : ClassificationFormatDefinition
    {
        public RoslynEnumFieldFormat() {
            this.DisplayName = "Roslyn Enum Field";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ExtensionMethodFormat)]
    [Name(Constants.ExtensionMethodFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynExtensionMethodFormat : ClassificationFormatDefinition
    {
        public RoslynExtensionMethodFormat() {
            this.DisplayName = "Roslyn Extension Method";
            this.IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.StaticMethodFormat)]
    [Name(Constants.StaticMethodFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynStaticMethodFormat : ClassificationFormatDefinition
    {
        public RoslynStaticMethodFormat() {
            this.DisplayName = "Roslyn Static Method";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.NormalMethodFormat)]
    [Name(Constants.NormalMethodFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynNormalMethodFormat : ClassificationFormatDefinition
    {
        public RoslynNormalMethodFormat() {
            this.DisplayName = "Roslyn Normal Method";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ConstructorFormat)]
    [Name(Constants.ConstructorFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynConstructorFormat : ClassificationFormatDefinition
    {
        public RoslynConstructorFormat() {
            this.DisplayName = "Roslyn Constructor";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.TypeParameterFormat)]
    [Name(Constants.TypeParameterFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynTypeParameterFormat : ClassificationFormatDefinition
    {
        public RoslynTypeParameterFormat() {
            this.DisplayName = "Roslyn Type Parameter";
            this.ForegroundColor = Colors.SlateGray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ParameterFormat)]
    [Name(Constants.ParameterFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynParameterFormat : ClassificationFormatDefinition
    {
        public RoslynParameterFormat() {
            this.DisplayName = "Roslyn Parameter";
            this.ForegroundColor = Colors.SlateGray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.NamespaceFormat)]
    [Name(Constants.NamespaceFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynNamespaceFormat : ClassificationFormatDefinition
    {
        public RoslynNamespaceFormat() {
            this.DisplayName = "Roslyn Namespace";
            this.ForegroundColor = Colors.LimeGreen;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.PropertyFormat)]
    [Name(Constants.PropertyFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynPropertyFormat : ClassificationFormatDefinition
    {
        public RoslynPropertyFormat() {
            this.DisplayName = "Roslyn Property";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.LocalFormat)]
    [Name(Constants.LocalFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynLocalFormat : ClassificationFormatDefinition
    {
        public RoslynLocalFormat() {
            this.DisplayName = "Roslyn Local";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.TypeSpecialFormat)]
    [Name(Constants.TypeSpecialFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynTypeSpecialFormat : ClassificationFormatDefinition
    {
        public RoslynTypeSpecialFormat() {
            this.DisplayName = "Roslyn Special Type";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.TypeNormalFormat)]
    [Name(Constants.TypeNormalFormat)]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class RoslynTypeNormalFormat : ClassificationFormatDefinition
    {
        public RoslynTypeNormalFormat() {
            this.DisplayName = "Roslyn Normal Type";
        }
    }
}
