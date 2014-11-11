using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Classification;

namespace RoslynColorizer {

  [Export(typeof(IClassifierProvider))]
  [ContentType("CSharp")]
  [ContentType("Basic")]
  internal class RoslynColorizerProvider : IClassifierProvider {
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

    public IClassifier GetClassifier(ITextBuffer buffer) {
      return buffer.Properties.GetOrCreateSingletonProperty<RoslynColorizer>(delegate {
        return new RoslynColorizer(buffer, ClassificationRegistry);
      });
    }
  }

  class RoslynColorizer : IClassifier {
    private IClassificationType parameterType;
    private IClassificationType fieldType;
    private ITextBuffer theBuffer;

    internal RoslynColorizer(ITextBuffer buffer, IClassificationTypeRegistryService registry) {
      theBuffer = buffer;
      parameterType = registry.GetClassificationType(Constants.ParameterFormat);
      fieldType = registry.GetClassificationType(Constants.FieldFormat);
    }

    public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
      var workspace = theBuffer.GetWorkspace();
      var docId = workspace.GetDocumentIdInCurrentContext(theBuffer.AsTextContainer());
      var doc = workspace.CurrentSolution.GetDocument(docId);
      return GetClassificationSpansInt(workspace, doc, span);
    }

    private IList<ClassificationSpan> GetClassificationSpansInt(
          Workspace workspace, Document doc, SnapshotSpan span) {
      var list = new List<ClassificationSpan>();
      SemanticModel model;
      if ( !doc.TryGetSemanticModel(out model) ) {
        return list;
      }
      SyntaxNode treeRoot;
      if ( !doc.TryGetSyntaxRoot(out treeRoot) ) {
        return null;
      }
      var textSpan = TextSpan.FromBounds(span.Start, span.End);
      var classifiedSpans = Classifier.GetClassifiedSpans(model, textSpan, workspace);
      var comparer = StringComparer.InvariantCultureIgnoreCase;

      var identifiers = from cs in classifiedSpans
                        where comparer.Compare(cs.ClassificationType, "identifier") == 0
                        select cs;
      foreach ( var id in identifiers ) {
        var node = treeRoot.FindNode(id.TextSpan);
        var info = model.GetSymbolInfo(node);
        if ( info.Symbol == null )
          continue;
        switch ( info.Symbol.Kind ) {
          case SymbolKind.Parameter:
            list.Add(id.TextSpan.ToClassifiedSpan(span.Snapshot, parameterType));
            break;
          case SymbolKind.Field:
            list.Add(id.TextSpan.ToClassifiedSpan(span.Snapshot, fieldType));
            break;
        }
      }
      return list;
    }

#pragma warning disable 67
    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
  }
}
