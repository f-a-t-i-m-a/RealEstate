module JahanJooy.Common {

    class JalaaliDatepickerPopupDefinition implements ng.IDirective {
        static $name = "jjJalaaliDatepickerPopup";
        static $inject = [];

        static getFactory(): ng.IDirectiveFactory {
            var factory = () => {
                return new JalaaliDatepickerPopupDefinition();
            };
            factory.$inject = JalaaliDatepickerPopupDefinition.$inject;
            return factory;
        }

        restrict = "E";
        template = "<div class='input-group'>" +
        "<input type='text' class='form-control' datepicker-popup-persian='jYYYY/jM/jD' ng-model='jjModel' max-date='maxDate' min-date='minDate' is-open='datePopupIsOpen' datepicker-options='{formatYear: \"yy\", startingDay: 6}' current-text='امروز' clear-text='خالی' close-text='بسته' placeholder='13__/__/__' ng-required='required' style='direction: ltr; text-align: right;' />" +
            "<span class='input-group-btn'>" +
            "<button type='button' class='btn btn-default' ng-click='openDateFilter($event)'><i class='fa fa-calendar'></i></button>" +
            "</span>" +
            "</div>";

        scope = {
            jjModel: "=",
            required: "=",
            minDate: "=",
            maxDate: "="
        };

        controller = ($scope) => {
            $scope.openDateFilter = ($event) => {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.datePopupIsOpen = true;
            };
        };
    }

    ngModule.directive(JalaaliDatepickerPopupDefinition.$name, JalaaliDatepickerPopupDefinition.getFactory());
}