using JahanJooy.RealEstateAgency.Util.Report.Functions;
using Stimulsoft.Report.Dictionary;

namespace JahanJooy.RealEstateAgency.Util.Report
{
    public static class StiRealEstateUtils
    {
        public static void RegisterCustomFunctions()
        {
//            RegisterAttachmentFunctions();
//            RegisterEntityFunctions();
            RegisterLocalizationFunctions();
        }

//        private static void RegisterAttachmentFunctions()
//        {
//            StiFunctions.AddFunction("Attachment", "CustomerAttachmentFullSize", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//            StiFunctions.AddFunction("Attachment", "CustomerAttachmentMediumSize", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//            StiFunctions.AddFunction("Attachment", "CustomerAttachmentThumbnail", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//
//            StiFunctions.AddFunction("Attachment", "ContestAttachmentFullSize", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//            StiFunctions.AddFunction("Attachment", "ContestAttachmentMediumSize", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//            StiFunctions.AddFunction("Attachment", "ContestAttachmentThumbnail", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//
//            StiFunctions.AddFunction("Attachment", "FinancialAttachmentFullSize", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//            StiFunctions.AddFunction("Attachment", "FinancialAttachmentMediumSize", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//            StiFunctions.AddFunction("Attachment", "FinancialAttachmentThumbnail", "",
//                typeof(StiAttachmentFunctions), typeof(Image), "", new[] { typeof(long) }, new[] { "attachmentId" });
//        }

//        private static void RegisterEntityFunctions()
//        {
//            StiFunctions.AddFunction("Entity", "LoadContest", "", typeof(StiEntityFunctions), typeof(Contest), "", new[] { typeof(long) }, new[] { "contestId" });
//        }
//
        private static void RegisterLocalizationFunctions()
        {
            StiFunctions.AddFunction("Localization", "TranslateEnum", "Translated an enum member to appropriate display string",
                typeof(StiLocalizationFunctions), typeof(string), "", new[] { typeof(object) }, new[] { "value" });

            StiFunctions.AddFunction("Localization", "TranslateEnum",
                "Translated an enum member to appropriate display string",
                typeof(StiLocalizationFunctions), typeof(string), "", new[] { typeof(string), typeof(object) },
                new[] { "enumTypeName", "value" });
        }
    }
}