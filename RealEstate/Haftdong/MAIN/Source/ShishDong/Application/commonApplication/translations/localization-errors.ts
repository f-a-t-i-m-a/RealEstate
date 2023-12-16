module JahanJooy.RealEstateAgency {
    import Common = JahanJooy.Common; // 500 errors

    Common.localization.addErrorTranslations(null, [
        { key: "CanNotConnectToServer", text: "لطفاً از اتصال خود به اینترنت اطمینان حاصل کنید." },
        { key: "UnknownServerError", text: "خطای نامشخص در سرویس دهنده. لطفاً با مدیر سیستم تماس بگیرید" },
        { key: "ServerResourceNotFound", text: "خطا: منبع مورد درخواست یافت نشد. لطفاً با مدیر سیستم تماس بگیرید" },
        { key: "AccessDenied", text: "کاربر فعلی دسترسی کافی برای اجرای این دستور را ندارد" },
        { key: "UnknownValidationError", text: "داده های ورودی ناقص است یا شرط های سیستم را نقض می کند. برای جزئیات با مدیریت سیستم تماس بگیرید" },
        { key: "UnknownError", text: "خطای نامشخص" },
        { key: "NotFound", text: "چنین آیتمی پیدا نشد" }
    ]);

    // Authentication errors

    Common.localization.addErrorTranslations(null, [
        { key: "invalid_grant", text: "نام کاربری یا رمز عبور صحیح نیست" }
    ]);

    //ConfigureDataItems

    Common.localization.addErrorTranslations(null, [
        { key: "ConfigurationDataItem_UnexpectedError", text: "خطای نامشخصی رخ داده است" },
        { key: "ConfigurationDataItem_NotFound", text: "داده پیکربندی مورد نظر یافت نشد" },
        { key: "ConfigurationDataItem_NotModified", text: "داده پیکربندی مورد نظر ویرایش نشد" },
        { key: "ConfigurationDataItem_Identifier_ValueNotSpecified", text: "کلید داده پیکربندی خالی است" }
    ]);

    Common.localization.addErrorTranslations(null, [
        { key: "Contract_UnexpectedError", text: "خطای نامشخصی رخ داده است" },
        { key: "Contract_State_IsNotValid", text: "وضعیت قرارداد مورد نظر معتبر نیست" },
        { key: "Contract_NotFound", text: "قرارداد مورد نظر یافت نشد" },
        { key: "Contract_NotModified", text: "قرارداد مورد نظر ویرایش نشد" },
        { key: "Contract_SellerReference_ValueNotSpecified", text: "فروشنده مشخص نشده است" },
        { key: "Contract_BuyerReference_ValueNotSpecified", text: "خریدار مشخص نشده است" }
    ]);

    Common.localization.addErrorTranslations(null, [
        { key: "Supply_UnexpectedError", text: "خطای نامشخصی رخ داده است" },
        { key: "Supply_AccessDenied", text: "شما به این آگهی دسترسی ندارید" },
        { key: "Supply_State_IsNotValid", text: "وضعیت آگهی مورد نظر معتبر نیست" },
        { key: "Supply_NotFound", text: "آگهی مورد نظر یافت نشد" },
        { key: "Supply_Owner_NotFound", text: "مالک ملک آگهی مورد نظر یافت نشد" },
        { key: "Supply_NotModified", text: "آگهی مورد نظر ویرایش نشد" },
        { key: "Supply_MatchedAndModifiedAreNotEqual", text: "همه آگهی های مورد نظر ویرایش نشدند" },
        { key: "Supply_PriceSpecificationType_ValueNotSpecified", text: "شیوه ی تعیین قیمت مشخص نشده است" },
        { key: "Supply_Price_ValueNotSpecified", text: "قیمت مشخص نشده است" },
        { key: "Supply_Price_IsNotCalculated", text: "قیمت به درستی محاسبه نشده است" },
        { key: "Supply_ContactInfo_ValueNotSpecified", text: "اطلاعات مشخص نشده است" },
        { key: "Supply_Mortgage_ValueNotSpecified", text: "میزان ودیعه مشخص نشده است" },
        { key: "Supply_Rent_ValueNotSpecified", text: "میزان اجاره مشخص نشده است" },
        { key: "Supply_IntentionOfOwner_ValueNotSpecified", text: "نوع آگهی مشخص نشده است" },
        { key: "Supply_ExpirationTime_NotValid", text: "تاریخ اتمام عمومی بودن آگهی معتبر نیست" },
        { key: "Supply_Phones_ValueNotSpecified", text: "برای این آگهی حداقل باید یک شماره تماس ثبت شود" }
    ]);

    Common.localization.addErrorTranslations(null, [
        { key: "Request_UnexpectedError", text: "خطای نامشخصی رخ داده است" },
        { key: "Request_State_IsNotValid", text: "وضعیت درخواست مورد نظر معتبر نیست" },
        { key: "Request_AccessDenied", text: "شما به این درخواست دسترسی ندارید" },
        { key: "Request_UsageType_IsNotValid", text: "کاربری درخواست مورد نظر معتبر نیست" },
        { key: "Request_IntentionOfCustomer_IsNotValid", text: "قصد مشتری در درخواست مورد نظر معتبر نیست" },
        { key: "Request_Owner_IsNotValid", text: "مالک درخواست مورد نظر معتبر نیست" },
        { key: "Request_Owner_ValueNotSpecified", text: "مالک درخواست مورد نظر مشخص نشده است" },
        { key: "Request_ContactInfo_ValueNotSpecified", text: "اطلاعات درخواست مورد نظر مشخص نشده است" },
        { key: "Request_NotFound", text: "درخواست مورد نظر یافت نشد" },
        { key: "Request_NotModified", text: "درخواست مورد نظر ویرایش نشد" },
        { key: "Request_MatchedAndModifiedAreNotEqual", text: "همه درخواست های مورد نظر ویرایش نشدند" },
        { key: "Request_IsNotValid", text: "درخواست معتبر نیست" },
        { key: "Request_UsageType_IsNotValid", text: "کاربری درخواست معتبر نیست" },
        { key: "Request_IntentionOfCustomer_IsNotValid", text: "نوع معامله ی درخواست معتبر نیست" },
        { key: "Request_Owner_IsNotValid", text: "مشتری درخواست معتبر نیست" },
        { key: "Request_Phones_ValueNotSpecified", text: "برای این درخواست حداقل باید یک شماره تماس ثبت شود" }
    ]);

    Common.localization.addErrorTranslations(null, [
        { key: "Property_UnexpectedError", text: "خطای نامشخصی رخ داده است" },
        { key: "Property_AccessDenied", text: "شما به این ملک دسترسی ندارید" },
        { key: "Property_State_IsNotValid", text: "وضعیت ملک مورد نظر معتبر نیست" },
        { key: "Property_ID_IsNotValid", text: "ملک مورد نظر معتبر نیست" },
        { key: "Property_NotFound", text: "ملک مورد نظر یافت نشد" },
        { key: "Property_NotModified", text: "ملک مورد نظر ویرایش نشد" },
        { key: "Property_IsNotCalculated", text: "قیمت ملک مورد نظر محاسبه نشد" },
        { key: "Property_MatchedAndModifiedAreNotEqual", text: "همه ملک های مورد نظر ویرایش نشدند" },
        { key: "Property_UsageType_ValueNotSpecified", text: "کاربری ملک مشخص نشده است" },
        { key: "Property_PropertyType_ValueNotSpecified", text: "نوع ملک مشخص نشده است" },
        { key: "Property_Owner_ValueNotSpecified", text: "مالک مشخص نشده است" },
        { key: "Property_Owner_NotFound", text: "مالک مورد نظر یافت نشد" }
    ]);

    Common.localization.addErrorTranslations(null, [
        { key: "ReportTemplate_UnexpectedError", text: "خطای نامشخصی رخ داده است" },
        { key: "ReportTemplate_NotFound", text: "گزارش مورد نظر یافت نشد" },
        { key: "ReportTemplate_NotModified", text: "گزارش مورد نظر ویرایش نشد" }
    ]);

    Common.localization.addErrorTranslations(null, [
        { key: "Vicinity_UnexpectedError", text: "خطای نامشخصی رخ داده است" },
        { key: "Vicinity_HasChild", text: "محله مورد نظر دارای محله های زیرمجموعه است" },
        { key: "Vicinity_NotFound", text: "محله مورد نظر یافت نشد" },
        { key: "Vicinity_NotModified", text: "محله مورد نظر ویرایش نشد" },
        { key: "Vicinity_MatchedAndModifiedAreNotEqual", text: "همه محله های مورد نظر ویرایش نشدند" },
        { key: "Vicinity_MatchedAndDeletedAreNotEqual", text: "همه محله های مورد نظر حذف نشدند" }
    ]);

    Common.localization.addErrorTranslations(null, [
        { key: "User_UnexpectedError", text: "خطای نامشخصی رخ داده است" },
        { key: "User_AccessDenied", text: "شما به این کاربر دسترسی ندارید" },
        { key: "User_ContactMethod_IsTooFrequentContactMethodVerification", text: "برای این اطلاعات تماس، اخیراً پیغام فعال سازی ارسال شده است. لطفاً چند دقیقه منتظر بمانید و سپس مجدداً سعی کنید" },
        { key: "User_ContactMethod_UserContactMethodVerification_ExpirationTime_VerificationDeadlineExpired", text: "مهلت استفاده از رمز فعال سازی به اتمام رسیده، لطفا فعال سازی را مجدداً آغاز کنید" },
        { key: "User_ContactMethod_UserContactMethodVerification_VerificationSecret_InvalidSecret", text: "رمز وارد شده درست نیست" },
        { key: "User_ContactMethod_ValueNotSpecified", text: "اطلاعات تماس مشخص نشده است" },
        { key: "User_ContactMethod_NotFound", text: "اطلاعات تماس یافت نشده است" },
        { key: "User_ContactMethod_NotValid", text: "اطلاعات تماس معتبر نیست" },
        { key: "User_NotFound", text: "چنین کاربری پیدا نشد" },
        { key: "User_NotModified", text: "کاربر مورد نظر ویرایش نشد" },
        { key: "User_DisplayName_ValueNotSpecified", text: "نام نمایشی مشخص نشده است" },
        { key: "User_Contact_Phones_ValueNotSpecified", text: "تلفن مشخص نشده است" },
        { key: "User_Contact_Phones_ContactMethodShouldBeVerifyable", text: "لطفا شماره موبایل وارد کنید" },
        { key: "User_UserName_ValueNotSpecified", text: "نام کاربری مشخص نشده است" },
        { key: "User_UserName_ValueDoesNotHaveAppropriateLength", text: "طول نام کاربری نباید کمتر از 4 حرف و بیشتر از 50 حرف باشد" },
        { key: "User_Password_ValueDoesNotHaveAppropriateLength", text: "طول کلمه ی عبور نباید کمتر از 6 حرف باشد" },
        { key: "User_Password_ValueNotSpecified", text: "رمز عبور مشخص نشده است" },
        { key: "User_UnexpectedValidationError", text: "خطای غیرمنتظره ای اتفاق افتاده است ، پیغام خطا: {0}" },
        { key: "User_CreatedByID_UnexpectedError", text: "خطای غیرمنتظره ای اتفاق افتاده است ، پیغام خطا: {0}" },
        { key: "User_Password_UnexpectedError", text: "خطای غیرمنتظره ای در ذخیره رمز عبور اتفاق افتاده است ، پیغام خطا: {0}" },
        { key: "User_Password_NotValid", text: "رمز عبور درست وارد نشده است" },
        { key: "UserNameIsAlreadyTaken", text: "نام کاربری تکراری است" },
        { key: "PasswordCantContainUserName", text: "کلمه ی عبور نباید شامل نام کاربری باشد" },
        { key: "PasswordCantContainDisplayName", text: "کلمه ی عبور نباید شامل نام نمایشی باشد" },
        { key: "PasswordCantContainFullName", text: "کلمه ی عبور نباید شامل نام کامل باشد" },
        { key: "TooFrequentRequests", text: "برای این کاربر، اخیراً درخواست دیگری ثبت شده است. لطفاً بعد از چند ساعت مجدداً سعی کنید" },
        { key: "PasswordResetTokensDontMatch", text: "رمز ارائه شده برای این کاربر نامعتبر است" },
        { key: "RequestHasExpired", text: "مهلت اجرای درخواست به پایان رسیده است. لطفاً مجدداً از ابتدا شروع کنید" },
        { key: "OnlyMobileNumbersAllowed", text: "فقط شماره تلفن های همراه برای ارسال پیام کوتاه قابل قبول است" },
        { key: "InvalidEmailAddress", text: "آدرس ایمیل وارد شده صحیح نیست" },
        { key: "ContactMethodShouldBeVerifyable", text: "قابلیت ارسال پیامک به این شماره تلفن وجود ندارد" }
    ]);

    Common.localization.addErrorTranslations(null,
        [
            { key: "Phone_NotValid", text: "شماره تلفن معتبر نیست" },
            { key: "Email_NotValid", text: "ایمیل معتبر نیست" },
            { key: "Address_NotValid", text: "آدرس معتبر نیست" }
        ]);
}