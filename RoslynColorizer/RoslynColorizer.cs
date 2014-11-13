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
using Microsoft.VisualStudio.Text.Tagging;

namespace RoslynColorizer {

  [Export(typeof(ITaggerProvider))]
  [ContentType("CSharp")]
  [ContentType("Basic")]
  [TagType(typeof(IClassificationTag))]
  internal class RoslynColorizerProvider : ITaggerProvider {
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      return (ITagger<T>)new RoslynColorizer(buffer, ClassificationRegistry);
    }
  }

  class RoslynColorizer : ITagger<IClassificationTag> {
    private IClassificationType parameterType;
    private IClassificationType fieldType;
    private ITextBuffer theBuffer;
#pragma warning disable CS0067
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore CS0067

    internal RoslynColorizer(ITextBuffer buffer, IClassificationTypeRegistryService registry) {
      theBuffer = buffer;
      parameterType = registry.GetClassificationType(Constants.ParameterFormat);
      fieldType = registry.GetClassificationType(Constants.FieldFormat);
    }

    public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count == 0 ) {
        return Enumerable.Empty<ITagSpan<IClassificationTag>>();
      }
      var workspace = theBuffer.GetWorkspace();
      var docId = workspace.GetDocumentIdInCurrentContext(theBuffer.AsTextContainer());
      var doc = workspace.CurrentSolution.GetDocument(docId);
      return GetTagsImpl(workspace, doc, spans);
    }

    private IEnumerable<ITagSpan<IClassificationTag>> GetTagsImpl(
          Workspace workspace, Document doc,
          NormalizedSnapshotSpanCollection spans) {
      SemanticModel model;
      if ( !doc.TryGetSemanticModel(out model) ) {
        yield break;
      }
      SyntaxNode treeRoot;
      if ( !doc.TryGetSyntaxRoot(out treeRoot) ) {
        yield break;
      }
      var snapshot = spans[0].Snapshot;

      IEnumerable<ClassifiedSpan> identifiers = GetIdentifiersInSpans(workspace, model, spans);

      foreach ( var id in identifiers ) {
        var node = treeRoot.FindNode(id.TextSpan);
        var info = model.GetSymbolInfo(node);
        if ( info.Symbol == null )
          continue;
        switch ( info.Symbol.Kind ) {
          case SymbolKind.Parameter:
            yield return id.TextSpan.ToTagSpan(snapshot, parameterType);
            break;
          case SymbolKind.Field:
            yield return id.TextSpan.ToTagSpan(snapshot, fieldType);
            break;
        }
      }
    }

    private IEnumerable<ClassifiedSpan> GetIdentifiersInSpans(Workspace workspace, SemanticModel model, NormalizedSnapshotSpanCollection spans) {
      var comparer = StringComparer.InvariantCultureIgnoreCase;
      var classifiedSpans =
        spans.SelectMany(span => {
          var textSpan = TextSpan.FromBounds(span.Start, span.End);
          return Classifier.GetClassifiedSpans(model, textSpan, workspace);
        });

      return from cs in classifiedSpans
             where comparer.Compare(cs.ClassificationType, "identifier") == 0
             select cs;
    }
  }
}
