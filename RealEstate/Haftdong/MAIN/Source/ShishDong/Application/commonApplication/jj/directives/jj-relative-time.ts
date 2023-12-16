module JahanJooy.Common {
    import Moment = moment.Moment;

    class RelativeTimeDirectiveInstance {

        private origin: Moment;
        private timeoutId: ng.IPromise<any>;
        private element: JQuery;

        constructor(private $interval: ng.IIntervalService) {
        }

        link(scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) {
            this.element = $("<span></span>");
            this.element.appendTo(element);

            scope.$watch(attrs["origin"],(value) => {
                this.onOriginChanged(value);
            });

            var update = false;
            if (attrs["update"])
                update = scope.$eval(attrs["update"]);

            if (update) {
                this.timeoutId = this.$interval(() => {
                    this.updateTime();
                }, 30000);
            }

            element.on("$destroy",() => {
                if (this.timeoutId)
                    this.$interval.cancel(this.timeoutId);
            });
        }

        private updateTime() {
            if (this.origin)
                this.element.text(this.origin.isValid() ? this.origin.fromNow() : "?");
            else 
                this.element.text("");
            
        }

        private onOriginChanged(value: any) {
            var m: Moment = undefined;
            if (value) {
                if (moment.isMoment(value))
                    m = value;
                else
                    m = moment(value);
            }
            this.origin = m;
            this.updateTime();
        }

    }

    class RelativeTimeDirectiveDefinition implements ng.IDirective {
        static $name = "jjRelativeTime";
        static $inject = ["$interval"];
        constructor(private $interval: ng.IIntervalService) {
        }

        static getFactory(): ng.IDirectiveFactory {
            var factory = ($interval) => new RelativeTimeDirectiveDefinition($interval);
            factory.$inject = RelativeTimeDirectiveDefinition.$inject;
            return factory;
        }

        restrict = "E";
        link = (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
            new RelativeTimeDirectiveInstance(this.$interval).link(scope, element, attrs);
        }
    }

    ngModule.directive(RelativeTimeDirectiveDefinition.$name, RelativeTimeDirectiveDefinition.getFactory());

}
