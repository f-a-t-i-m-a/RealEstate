module JahanJooy.Common {

    class TristateDirectiveDefinition implements ng.IDirective {
        static $name = "jjTristate";
        static $inject = [];

        static getFactory(): ng.IDirectiveFactory {
            var factory = () => {
                return new TristateDirectiveDefinition();
            };
            factory.$inject = TristateDirectiveDefinition.$inject;
            return factory;
        }

        restrict = "A";
        link = (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
            element.addClass("tristate");

            scope.$watch(attrs[TristateDirectiveDefinition.$name],(value) => {
                if (value === undefined || value === null) {
                    element.addClass("null");
                    element.removeClass("tick");
                    element.removeClass("cross");
                } else if (value) {
                    element.removeClass("null");
                    element.addClass("tick");
                    element.removeClass("cross");
                } else {
                    element.removeClass("null");
                    element.removeClass("tick");
                    element.addClass("cross");
                }
            });
        }
    }

    ngModule.directive(TristateDirectiveDefinition.$name, TristateDirectiveDefinition.getFactory());
}
