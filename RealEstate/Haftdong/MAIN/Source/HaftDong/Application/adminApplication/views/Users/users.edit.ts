module JahanJooy.HaftDong.User {
    import ContactMethodType = RealEstateAgency.Domain.Enums.User.ContactMethodType;
    appModule.controller("EditUserController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, scopes) => {

            $scope.GetTitle = () => {
                return 'ویرایش کاربر';
            }
            scopes.store('EditUserController', $scope);

            $scope.isCollapsed = false;
            $scope.contact = {
                mobileNumber: "",
                address: "",
                email: ""
            }

            function initialize() {
                $http.get("/api/web/users/get/" + $stateParams.id, null).success((data: any) => {
                    $scope.user = data;
                    if ($scope.user.Contact != null && $scope.user.Contact.length !== 0) {
                        if ($scope.user.Contact.Emails != null && $scope.user.Contact.Emails.length !==0)
                            $scope.contact.email = $scope.user.Contact.Emails[0].NormalizedValue;
                        if ($scope.user.Contact.Phones != null && $scope.user.Contact.Phones.length !== 0)
                            $scope.contact.mobileNumber = $scope.user.Contact.Phones[0].NormalizedValue;
                        if ($scope.user.Contact.Addresses != null && $scope.user.Contact.Addresses.length !== 0)
                            $scope.contact.address = $scope.user.Contact.Addresses[0].NormalizedValue;
                    }
                });
            }

            $scope.onSaveClick = () => {
               
                $http.post("/api/web/users/update", $scope.user).success((data: any) => {
                    $scope.$emit('EntityUpdated');
                    $state.go("users.details", { id: data.Id });
                });
            }

            $scope.onBackClick = () => {
                window.history.back();
            }

            initialize();
        }
    ]);

}