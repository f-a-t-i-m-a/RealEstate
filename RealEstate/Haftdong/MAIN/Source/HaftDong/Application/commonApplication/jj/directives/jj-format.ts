module JahanJooy.Common {

    class FormatDirectiveDefinition implements ng.IDirective {
        static $name = "jjFormat";
        static $allowFloat = "jjAllowFloat";
        static $inject = ["$filter"];

        static getFactory(): ng.IDirectiveFactory {
            var factory = ($filter) => {
                return new FormatDirectiveDefinition($filter);
            };
            factory.$inject = FormatDirectiveDefinition.$inject;
            return factory;
        }

        constructor(private $filter: ng.IFilterService) {
            
        }

        restrict = "A";
        require = "?ngModel";
        link = (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: ng.INgModelController) => {
            if (!ctrl)
                return;

            var filterFn = this.$filter(attrs[FormatDirectiveDefinition.$name]);
            var formatter: ng.IModelFormatter = () => { return <any>(filterFn(ctrl.$modelValue)); };
            var parser: ng.IModelParser = (viewValue: string) => {
                var plainNumber = viewValue.replace(/[^\d|\-+|\.+]/g, "");

                var caretDist = viewValue.length - element.caret();
                var newViewValue;
                if (attrs[FormatDirectiveDefinition.$allowFloat]) {
                    if (viewValue[viewValue.length - 1] === "." && caretDist === 0)
                        newViewValue = plainNumber;
                    else
                        newViewValue = this.$filter(attrs[FormatDirectiveDefinition.$name])(plainNumber);
                } else {
                    newViewValue = this.$filter(attrs[FormatDirectiveDefinition.$name])(plainNumber);
                }

                element.val(newViewValue);
                element.caret(newViewValue.length - caretDist);

                return plainNumber;                
            };

            ctrl.$formatters.unshift(formatter);
            ctrl.$parsers.unshift(parser);
        }
    }

    ngModule.directive(FormatDirectiveDefinition.$name, FormatDirectiveDefinition.getFactory());


}
