module JahanJooy.Common {
    export class EnumExtentions {
        static getNames(e: any): linqjs.Enumerable {
            return Enumerable.from(e)
                .where((pair: { key: string; value: any }) => isNaN(+pair.value))
                .select((pair: { key: string; value: any }) => <string>pair.value);
        }

        static getValues(e: any): linqjs.Enumerable {
            return Enumerable.from(e)
                .select((pair: { key: string; value: any }) => +pair.value)
                .where(val => !isNaN(val));
        }
    }
}