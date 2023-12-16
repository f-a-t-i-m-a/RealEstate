using System;
using System.Windows.Forms;
using Compositional.Composer;
using Compositional.Composer.Utility;
using JahanJooy.RealEstateAgency.TestPreparationService.Forms;

namespace JahanJooy.RealEstateAgency.TestPreparationService
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
            SetupComposer();

            var mainForm = new MainForm();
            Composer.InitializePlugs(mainForm);
            Application.Run(mainForm);
        }

        private static void SetupComposer()
        {
            Composer = new ComponentContext();
            Composer.RegisterAssembly("JahanJooy.RealEstateAgency.TestPreparationService");
            Composer.ProcessCompositionXmlFromResource("JahanJooy.RealEstateAgency.TestPreparationService.Composition.xml");

            ComposerLocalThreadBinder.Bind(Composer);
        }
    }
}
