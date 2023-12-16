module JahanJooy.Common {

    export interface ILocalization {
        enums: { [enumName: string]: ITranslationData };
        errors: { [pageName: string]: ITranslationData };

        addEnumTranslations(enumName: string, enumType: any, items: IAddEnumTranslationItem[]);
        addErrorTranslations(pageName: string, items: IAddTranslationItem[]);

        translateEnum(enumName: string, value: any): string;
        translateError(pageName: string, value: any): string;
    }

    export interface IAddEnumTranslationItem {
        key: number;
        text: string;
    }

    export interface IAddTranslationItem {
        key: string;
        text: string;
    }

    export interface ITranslationDataItem {
        invariant: string;
        [locale: string]: string;
    }

    export interface ITranslationData {
        [key: number]: ITranslationDataItem;
        [key: string]: ITranslationDataItem;
    }

    export class LocalizationManager implements ILocalization {

        static rootPage: string = "$root";

        enums: { [enumName: string]: ITranslationData } = {};
        errors: { [pageName: string]: ITranslationData } = {};


        addEnumTranslations(enumName: string, enumType: any, items: IAddEnumTranslationItem[]) {
            var enumTranslation = this.enums[enumName] = this.enums[enumName] || {};
            items.forEach(input => {
                if (enumTranslation[input.key])
                    enumTranslation[input.key].invariant = input.text;
                else
                    enumTranslation[input.key] = { invariant: input.text };

                if (enumType) {
                    var itemName: string = enumType[input.key];
                    if (enumTranslation[itemName])
                        enumTranslation[itemName].invariant = input.text;
                    else
                        enumTranslation[itemName] = { invariant: input.text };
                }
            });
        }

        addErrorTranslations(pageName: string, items: IAddTranslationItem[]) {
            if (!pageName)
                pageName = LocalizationManager.rootPage;
            
            var pageErrors = this.errors[pageName] = this.errors[pageName] || {};
            items.forEach(input => {
                if (pageErrors[input.key])
                    pageErrors[input.key].invariant = input.text;
                else
                    pageErrors[input.key] = { invariant: input.text };
            });
        }

        translateEnum(enumName: string, value): string {

            if (!this.enums[enumName])
                return value;

            var translation = this.enums[enumName][value];
            if (translation)
                return translation.invariant;

            return value;
        }

        translateError(pageName: string, resourceKey, errorKey?: any): string {
            if (!pageName)
                pageName = LocalizationManager.rootPage;

            var pageErrors = this.errors[pageName];
            if (pageErrors) {
                var translation = pageErrors[resourceKey] || pageErrors[errorKey];
                if (translation) {
                    return translation.invariant;
                }
            }

            if (pageName !== LocalizationManager.rootPage)
                return this.translateError(LocalizationManager.rootPage, resourceKey, errorKey);

            return resourceKey;
        }
    }

    export var localization = new LocalizationManager();

}
