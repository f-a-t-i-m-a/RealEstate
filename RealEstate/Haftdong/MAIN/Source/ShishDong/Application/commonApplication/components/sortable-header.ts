module JahanJooy.ShishDong.Components {
    appModule.directive('jjSortableHeader', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-directives/sortable-header.html',
            scope: {
                title: "=",
                sortColumnName: "=",
                sortColumn: "=",
                sortDirection: "="
            },
            controller: () => {
            }
        }
    });
}