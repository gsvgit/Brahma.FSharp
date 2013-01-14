using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Brahma.Reference
{
    public sealed class ComputeProvider: Brahma.ComputeProvider
    {
        private readonly CSharpCodeProvider _codeProvider;

        public ComputeProvider()
        {
            _codeProvider = new CSharpCodeProvider();
        }

        public override void Dispose()
        {
            _codeProvider.Dispose();
        }
    }
}
