module JahanJooy.ShishDong.Components {
    ngModule.directive('bsHasError', [() => {
        return {
            restrict: "A",
            link: (scope, element, attrs, ctrl) => {
                var input = element.find('input[ng-model]');
                if (input) {
                    scope.$watch(
                        () => input.hasClass('ng-invalid'),
                        isInvalid => { element.toggleClass('has-error', isInvalid); });
                }
            }
        };
    }]);
}


