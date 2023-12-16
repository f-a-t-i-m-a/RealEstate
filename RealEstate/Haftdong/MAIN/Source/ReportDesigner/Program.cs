using System;
using System.Windows.Forms;
using Compositional.Composer;
using Compositional.Composer.Utility;
using JahanJooy.RealEstateAgency.HaftDong.Server.Config;

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    static class Program
    {
        public static ComponentContext Composer { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetupAutoMapper();
            SetupComposer();
            StimulConfig.Configure();

            var loginForm = new LoginForm();
            Composer.InitializePlugs(loginForm);
            Application.Run(loginForm);
        }

        private static void SetupComposer()
        {
            Composer = new ComponentContext();
            Composer.RegisterAssembly("Compositional.Composer.Web");
            Composer.ProcessCompositionXmlFromResource("JahanJooy.RealEstateAgency.ReportDesigner.Composition.xml");

            ComposerLocalThreadBinder.Bind(Composer);
        }

        private static void SetupAutoMapper()
        {
            AutoMapperConfig.ConfigureAllMappers();
        }
    }
}
