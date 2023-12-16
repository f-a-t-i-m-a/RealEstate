module JahanJooy.Common {

    ngModule.run(() => {
        localization.addEnumTranslations("boolean", null, [
            { key: -1, text: "نامشخص" },
            { key: 0, text: "خیر" },
            { key: 1, text: "بله" }
        ]);

        localization.addEnumTranslations("boolean_authorized", null, [
            { key: -1, text: "نامشخص" },
            { key: 0, text: "غیر مجاز" },
            { key: 1, text: "مجاز" }
        ]);

        localization.addEnumTranslations("boolean_has", null, [
            { key: -1, text: "نامشخص" },
            { key: 0, text: "ندارد" },
            { key: 1, text: "دارد" }
        ]);

        localization.addEnumTranslations("boolean_exists", null, [
            { key: -1, text: "نامشخص" },
            { key: 0, text: "ناموجود" },
            { key: 1, text: "موجود" }
        ]);

        localization.addEnumTranslations("boolean_successful", null, [
            { key: -1, text: "نامشخص" },
            { key: 0, text: "ناموفق" },
            { key: 1, text: "موفق" }
        ]);

        localization.addEnumTranslations("boolean_correct", null, [
            { key: -1, text: "نامشخص" },
            { key: 0, text: "غلط" },
            { key: 1, text: "صحیح" }
        ]);
    });

    ngModule.filter("boolean",() => (input, category: string) => {
        var enumName = "boolean" + (category ? "_" + category : "");
        var key = (input === undefined || input === null) ? -1 : (input ? 1 : 0);
        return Common.localization.translateEnum(enumName, key);
    });
}
