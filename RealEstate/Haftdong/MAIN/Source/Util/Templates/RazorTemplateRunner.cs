using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Razor;
using AspNet.Identity.MongoDB;
using Compositional.Composer;
using Compositional.Composer.Web;
using JahanJooy.Common.Util;
using Microsoft.CSharp;
using ServiceStack;
using AssemblyPointer = JahanJooy.RealEstateAgency.Util.Properties.AssemblyPointer;

namespace JahanJooy.RealEstateAgency.Util.Templates
{
    [Contract]
    [Component]
    public class RazorTemplateRunner
    {
        [ComponentPlug]
        public IComposer Composer { get; set; }

        private RazorEngineHost _host;
        private RazorTemplateEngine _engine;
        private object _engineSync;

        private readonly ConcurrentDictionary<IRazorTemplate, Type> _templateTypeCache;

        #region Initialization

        public RazorTemplateRunner()
        {
            _templateTypeCache = new ConcurrentDictionary<IRazorTemplate, Type>();
        }

        [OnCompositionComplete]
        public void Initialize()
        {
            _host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = typeof (TemplateBase).FullName,
                DefaultNamespace = "RazorTemplateRunner",
                DefaultClassName = "Template"
            };

            _host.NamespaceImports.Add("System");

            _engine = new RazorTemplateEngine(_host);
            _engineSync = new object();
        }

        #endregion

        public string ApplyTemplate(IRazorTemplate template, object model)
        {
            Type templateType = GetTemplateType(template);
            var templateInstance = Activator.CreateInstance(templateType) as TemplateBase;

            if (templateInstance == null)
                throw new ArgumentException("Template type does not extend TemplateBase.");

            Composer.InitializePlugs(templateInstance, templateType);

            templateInstance.SetModel(model);
            templateInstance.Execute();
            return templateInstance.Buffer.ToString();
        }

        private Type GetTemplateType(IRazorTemplate template)
        {
            if (_templateTypeCache.ContainsKey(template))
                return _templateTypeCache[template];

            lock (_engineSync)
            {
                if (_templateTypeCache.ContainsKey(template))
                    return _templateTypeCache[template];

                using (TextReader reader = template.GetReader())
                {
                    return _templateTypeCache[template] = BuildType(reader);
                }
            }
        }

        private Type BuildType(TextReader template)
        {
            GeneratorResults razorResult = _engine.GenerateCode(template);

            if (razorResult == null)
                throw new ArgumentException("Could not generate code from template.");

            if (razorResult.ParserErrors != null && razorResult.ParserErrors.Count > 0)
                throw new ArgumentException("Error parsing razor template: " + razorResult.ParserErrors[0].Message +
                                            " @ " + razorResult.ParserErrors[0].Location);

            if (!razorResult.Success)
                throw new ArgumentException("Parsing razor template was not successful.");

            var codeProvider = new CSharpCodeProvider();
            var compilerResults = codeProvider.CompileAssemblyFromDom(
                new CompilerParameters(new[]
                {
                    GetType().Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
//											   typeof(ConfigurationDataItem).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
//											   typeof(ServiceContext).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (ObjectExtensions).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (GeneralResources).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (ComposerWebUtil).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (ComponentContext).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (IComposer).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (IHtmlString).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (TextExtensions).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (AssemblyPointer).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (Domain.Properties.AssemblyPointer).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (IdentityUser).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\"),
                    typeof (Microsoft.AspNet.Identity.IdentityExtensions).Assembly.CodeBase.Replace("file:///", "")
                        .Replace("/", "\\")
                }),
                razorResult.GeneratedCode);

            if (compilerResults == null)
                throw new ArgumentException("Could not build assembly from razor-generated code");

            if (compilerResults.Errors != null && compilerResults.Errors.HasErrors)
            {
                CompilerError err = compilerResults.Errors.OfType<CompilerError>().First(ce => !ce.IsWarning);
                throw new ArgumentException("Error compiling razor-generated code: " + err.ErrorText);
            }

            if (compilerResults.CompiledAssembly == null)
                throw new ArgumentException("Could not find the assembly created by compiling razor-generated code");

            var result = compilerResults.CompiledAssembly.GetType("RazorTemplateRunner.Template");
            if (result == null)
                throw new ArgumentException("Could not find expected Type from the razor-based compiled assembly.");

            return result;
        }
    }

    internal class GeneralResources
    {
    }
}