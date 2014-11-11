using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace RoslynColorizer {
  public static class Extensions {
    public static ClassificationSpan ToClassifiedSpan(this TextSpan span, ITextSnapshot snapshot, IClassificationType classifierType) {
      return new ClassificationSpan(
        new SnapshotSpan(snapshot, span.Start, span.Length),
        classifierType
        );
    }
  }
}
