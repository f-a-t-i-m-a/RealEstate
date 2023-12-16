module JahanJooy.HaftDong.Customer {
    appModule.controller("EditCustomerController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "$rootScope", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, $rootScope, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'ویرایش مشتری';
            }
            scopes.store('EditCustomerController', $scope);

            $scope.$watch("customer",
                c => {
                    if (c != null) {
                        c.Contact.Phones.forEach(p => {
                            p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                        });
                        c.Contact.Emails.forEach(e => {
                            e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                        });
                        c.Contact.Addresses.forEach(e => {
                            e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                        });
                    }
                });

            $scope.onSaveClick = () => {
                $http.post("/api/web/customers/update", $scope.customer).success((data: any) => {
                    toastr.success("مشتری با موفقیت ویرایش شد.");
                    $state.go("^");
                    $scope.$emit('EntityUpdated');
                });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }

        }
    ]);

}