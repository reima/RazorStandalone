using System;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace RazorStandalone
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var razorEngine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create("Views"),
                builder =>
                {
                    // Insert a new phase just before the code gen phase
                    // (hopefully).
                    builder.Phases.Insert(
                        builder.Phases.Count - 1,
                        new SetTemplateTypePhase("MyTemplateType"));
                });

            var item = razorEngine.FileSystem.GetItem("Test.cshtml");
            var cSharpDocument = razorEngine.Process(item).GetCSharpDocument();

            Console.WriteLine(cSharpDocument.GeneratedCode);
        }
    }

    internal class SetTemplateTypePhase : RazorEnginePhaseBase
    {
        private readonly string _templateTypeName;

        public SetTemplateTypePhase(string templateTypeName)
        {
            _templateTypeName = templateTypeName;
        }

        protected override void ExecuteCore(RazorCodeDocument codeDocument)
        {
            var documentNode = codeDocument.GetDocumentIntermediateNode();
            ThrowForMissingDocumentDependency(documentNode);

            // Decorate the current CodeTarget to set the custom template type.
            documentNode.Target =
                new SetTemplateTypeCodeTargetDecorator(
                    documentNode.Target,
                    _templateTypeName);
        }
    }

    internal class SetTemplateTypeCodeTargetDecorator : CodeTarget
    {
        private readonly CodeTarget _target;
        private readonly string _templateTypeName;

        public SetTemplateTypeCodeTargetDecorator(
            CodeTarget target,
            string templateTypeName)
        {
            _target = target;
            _templateTypeName = templateTypeName;
        }

        public override IntermediateNodeWriter CreateNodeWriter()
        {
            var nodeWriter = _target.CreateNodeWriter();
            if (nodeWriter is RuntimeNodeWriter runtimeNodeWriter)
            {
                // Set the custom template type.
                runtimeNodeWriter.TemplateTypeName = _templateTypeName;
            }

            return nodeWriter;
        }

        public override TExtension GetExtension<TExtension>()
        {
            return _target.GetExtension<TExtension>();
        }

        public override bool HasExtension<TExtension>()
        {
            return _target.HasExtension<TExtension>();
        }
    }
}
