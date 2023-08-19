using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WrongCharacter
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WrongCharacterCodeFixProvider)), Shared]
    public class WrongCharacterCodeFixProvider : CodeFixProvider
    {
        readonly Dictionary<string, string> cyrillicToLatin = new Dictionary<string, string>()
        {
            { "а", "a" },
            { "б", "b" },
            { "в", "v" },
            { "г", "g" },
            { "д", "d" },
            { "е", "e" },
            { "ё", "e" },
            { "ж", "zh" },
            { "з", "z" },
            { "и", "i" },
            { "й", "y" },
            { "к", "k" },
            { "л", "l" },
            { "м", "m" },
            { "н", "n" },
            { "о", "o" },
            { "п", "p" },
            { "р", "r" },
            { "с", "s" },
            { "т", "t" },
            { "у", "u" },
            { "ф", "f" },
            { "х", "h" },
            { "ц", "c" },
            { "ч", "ch" },
            { "ш", "sh" },
            { "щ", "sch" },
            { "ъ", "" },
            { "ы", "y" },
            { "ь", "" },
            { "э", "e" },
            { "ю", "yu" },
            { "я", "ya" },
        };
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(WrongCharacterAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            //var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();
            SyntaxNode declaration = root.FindToken(diagnosticSpan.Start).Parent;

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedSolution: c => MakeUppercaseAsync(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        async Task<Solution> MakeUppercaseAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            string tokenName = GetTokenName(node as CSharpSyntaxNode);
            var newName = ProcessName(tokenName);

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            ISymbol typeSymbol = GetDeclaredSymbos(node, semanticModel, cancellationToken);
            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }

        private static ISymbol GetDeclaredSymbos(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ISymbol typeSymbol;
            do
            {
                typeSymbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);
                node = node.Parent;
            } while (typeSymbol == null || node == null);
            return typeSymbol;
        }

        string GetTokenName(CSharpSyntaxNode node) {
            if (node is IdentifierNameSyntax identifierNameSyntax)
            {
                return identifierNameSyntax.Identifier.Text;
            }
            if (node is MethodDeclarationSyntax methodDeclarationSyntax)
            {
                return methodDeclarationSyntax.Identifier.Text;
            }
            if (node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                return classDeclarationSyntax.Identifier.Text;
            }
            if (node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
            {
                return interfaceDeclarationSyntax.Identifier.Text;
            }
            if (node is StructDeclarationSyntax structDeclarationSyntax)
            {
                return structDeclarationSyntax.Identifier.Text;
            }
            if (node is EnumDeclarationSyntax enumDeclarationSyntax)
            {
                return enumDeclarationSyntax.Identifier.Text;
            }
            if (node is DelegateDeclarationSyntax delegateDeclarationSyntax)
            {
                return delegateDeclarationSyntax.Identifier.Text;
            }
            if (node is VariableDeclaratorSyntax variableDeclaratorSyntax)
            {
                return variableDeclaratorSyntax.Identifier.Text;
            }
            if (node is ParameterSyntax parameterSyntax)
            {
                return parameterSyntax.Identifier.Text;
            }
            if (node is PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                return propertyDeclarationSyntax.Identifier.Text;
            }
            if (node is FieldDeclarationSyntax fieldDeclarationSyntax)
            {
                return fieldDeclarationSyntax.Declaration.Variables.First().Identifier.Text;
            }
            if (node is EventDeclarationSyntax eventDeclarationSyntax)
            {
                return eventDeclarationSyntax.Identifier.Text;
            }
            if (node is EventFieldDeclarationSyntax eventFieldDeclarationSyntax)
            {
                return eventFieldDeclarationSyntax.Declaration.Variables.First().Identifier.Text;
            }
            if (node is EnumMemberDeclarationSyntax enumMemberDeclarationSyntax)
            {
                return enumMemberDeclarationSyntax.Identifier.Text;
            }
            if (node is TypeParameterSyntax typeParameterSyntax)
            {
                return typeParameterSyntax.Identifier.Text;
            }
            if (node is ConstructorDeclarationSyntax constructorDeclarationSyntax)
            {
                return constructorDeclarationSyntax.Identifier.Text;
            }
            if (node is DestructorDeclarationSyntax destructorDeclarationSyntax)
            {
                return destructorDeclarationSyntax.Identifier.Text;
            }
            if (node is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
            {
                return namespaceDeclarationSyntax.Name.ToString();
            }
            return null;
        }
        string ProcessName(string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (!Char.IsLetter(c) || IsLatinLetter(c))
                {
                    sb.Append(c);
                    continue;
                }
                if (cyrillicToLatin.TryGetValue(c.ToString().ToLower(), out string latin))
                {
                    if (Char.IsUpper(c))
                    {
                        latin = latin.ToUpper();
                    }
                    sb.Append(latin);
                } else
                {
                    sb.Append("_");
                }
            }
            return sb.ToString();
        }
        bool IsLatinLetter(char c)
        {
           if (c >= 'a' && c <= 'z')
            {
                return true;
            }
            if (c >= 'A' && c <= 'Z')
            {
                return true;
            }
            return false;
        }
    }
}
