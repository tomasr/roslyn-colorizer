using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis;

namespace RoslynColorizer
{
    public static class VBExtensions
    {
        public static SyntaxKind VBKind(this SyntaxNode node) {
            return node.Kind();
        }
    }
}
