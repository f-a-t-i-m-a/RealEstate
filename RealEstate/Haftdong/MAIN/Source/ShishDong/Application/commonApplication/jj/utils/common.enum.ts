module JahanJooy.Common {
    export class EnumExtentions {
        static getNames(e: any): linqjs.IEnumerable<string> {
            return Enumerable.from(e)
                .where((pair: { key: string; value: any }) => isNaN(+pair.value))
                .select((pair: { key: string; value: any }) => <string>pair.value);
        }

        static getValues(e: any): linqjs.IEnumerable<number> {
            return Enumerable.from(e)
                .select((pair: { key: string; value: any }) => +pair.value)
                .where(val => !isNaN(val));
        }
    }
}