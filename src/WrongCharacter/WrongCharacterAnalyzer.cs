using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace WrongCharacter
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WrongCharacterAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DXMAUI0001";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalizeClass, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalizeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(AnalizeProperty, SymbolKind.Property);
            context.RegisterSymbolAction(AnalizeField, SymbolKind.Field);
            context.RegisterSymbolAction(AnalizeEvent, SymbolKind.Event);
            context.RegisterSymbolAction(AnalizeParameter, SymbolKind.Parameter);
            context.RegisterSymbolAction(AnalizeNamespace, SymbolKind.Namespace);

            context.RegisterSyntaxNodeAction(AnalizeLocal, SyntaxKind.LocalDeclarationStatement);
        }

        void AnalizeLocal(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LocalDeclarationStatementSyntax vs)
            {
                foreach (var local in vs.Declaration.Variables)
                {
                    if (context.SemanticModel.GetDeclaredSymbol(local) is ILocalSymbol resolvedLocal)
                    {
                        AnalizeName(resolvedLocal, d => context.ReportDiagnostic(d));
                    }
                }
            }
        }
        static void AnalizeNamespace(SymbolAnalysisContext context)
        {
            AnalizeName(context);
        }
        static void AnalizeParameter(SymbolAnalysisContext context)
        {
            AnalizeName(context);
        }
        static void AnalizeEvent(SymbolAnalysisContext context)
        {
            AnalizeName(context);
        }
        static void AnalizeField(SymbolAnalysisContext context)
        {
            AnalizeName(context);
        }
        static void AnalizeProperty(SymbolAnalysisContext context)
        {
            AnalizeName(context);
        }
        static void AnalizeClass(SymbolAnalysisContext context)
        {
            AnalizeName(context);
        }
        static void AnalizeMethod(SymbolAnalysisContext context)
        {
            AnalizeName(context);
        }
        static void AnalizeName(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol;
            AnalizeName(symbol, d => context.ReportDiagnostic(d));
        }
        static void AnalizeName(ISymbol symbol, Action<Diagnostic> reportDiagnostic)
        {
            if (symbol.Name.ToCharArray().Any(x => Char.IsLetter(x) && !IsLatinLetter(x)))
            {
                var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
                reportDiagnostic(diagnostic);
            }
        }   
        static bool IsLatinLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
    }
}
