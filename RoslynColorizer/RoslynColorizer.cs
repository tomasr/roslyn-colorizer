using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using CSharp = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

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
    private IClassificationType extensionMethodType;
    private ITextBuffer theBuffer;
    private RoslynDocument cache;
#pragma warning disable CS0067
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore CS0067

    internal RoslynColorizer(ITextBuffer buffer, IClassificationTypeRegistryService registry) {
      theBuffer = buffer;
      parameterType = registry.GetClassificationType(Constants.ParameterFormat);
      fieldType = registry.GetClassificationType(Constants.FieldFormat);
      extensionMethodType = registry.GetClassificationType(Constants.ExtensionMethodFormat);
    }

    public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
      if ( spans.Count == 0 ) {
        return Enumerable.Empty<ITagSpan<IClassificationTag>>();
      }
      if ( this.cache == null || this.cache.Snapshot != spans[0].Snapshot ) {
        // this makes me feel dirty, but otherwise it will not
        // work reliably, as TryGetSemanticModel() often will return false
        // should make this into a completely async process somehow
        var task = RoslynDocument.Resolve(theBuffer, spans[0].Snapshot);
        task.Wait();
        if ( task.IsFaulted ) {
          // TODO: report this to someone.
          return Enumerable.Empty<ITagSpan<IClassificationTag>>();
        }
        cache = task.Result;
      }
      return GetTagsImpl(this.cache, spans);
    }

    private IEnumerable<ITagSpan<IClassificationTag>> GetTagsImpl(
          RoslynDocument doc,
          NormalizedSnapshotSpanCollection spans) {
      var snapshot = spans[0].Snapshot;

      IEnumerable<ClassifiedSpan> identifiers = 
        GetIdentifiersInSpans(doc.Workspace, doc.SemanticModel, spans);

      foreach ( var id in identifiers ) {
        var node = doc.SyntaxRoot.FindNode(id.TextSpan);
        var symbol = doc.SemanticModel.GetSymbolInfo(GetExpression(node)).Symbol;
        if ( symbol == null ) {
          continue;
        }
        switch ( symbol.Kind ) {
          case SymbolKind.Parameter:
            yield return id.TextSpan.ToTagSpan(snapshot, parameterType);
            break;
          case SymbolKind.Field:
            if ( symbol.ContainingType.TypeKind != TypeKind.Enum ) {
              yield return id.TextSpan.ToTagSpan(snapshot, fieldType);
            }
            break;
          case SymbolKind.Method:
            if ( IsExtensionMethod(symbol) ) {
              yield return id.TextSpan.ToTagSpan(snapshot, extensionMethodType);
            }
            break;
        }
      }
    }

    private bool IsExtensionMethod(ISymbol symbol) {
      var method = (IMethodSymbol)symbol;
      return method.IsExtensionMethod;
    }

    private SyntaxNode GetExpression(SyntaxNode node) {
      if ( node.CSharpKind() == CSharp.SyntaxKind.Argument ) {
        return ((CSharp.Syntax.ArgumentSyntax)node).Expression;
      } else if ( node.VBKind() == VB.SyntaxKind.SimpleArgument ) {
        return ((VB.Syntax.SimpleArgumentSyntax)node).Expression;
      }
      return node;
    }

    private IEnumerable<ClassifiedSpan> GetIdentifiersInSpans(
          Workspace workspace, SemanticModel model,
          NormalizedSnapshotSpanCollection spans) {
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

    public class RoslynDocument {
      public Workspace Workspace { get; private set; }
      public Document Document { get; private set; }
      public SemanticModel SemanticModel { get; private set; }
      public SyntaxNode SyntaxRoot { get; private set; }
      public ITextSnapshot Snapshot { get; private set; }

      private RoslynDocument() {
      }

      public static async Task<RoslynDocument> Resolve(ITextBuffer buffer, ITextSnapshot snapshot) {
        var workspace = buffer.GetWorkspace();
        var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
        // the ConfigureAwait() calls are important,
        // otherwise we'll deadlock VS
        var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
        var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
        return new RoslynDocument {
          Workspace = workspace,
          Document = document,
          SemanticModel = semanticModel,
          SyntaxRoot = syntaxRoot,
          Snapshot = snapshot
        };
      }
    }
  }
}
