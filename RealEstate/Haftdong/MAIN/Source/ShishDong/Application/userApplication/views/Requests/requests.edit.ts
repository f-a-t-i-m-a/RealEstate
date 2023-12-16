module JahanJooy.ShishDong.Request {
    appModule.controller("EditRequestController",
    [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "$rootScope", "toastr", "scopes",
        ($scope,
            $timeout,
            $state: angular.ui.IStateService,
            $http,
            $stateParams,
            $rootScope,
            toastr,
            scopes) => {

            $scope.GetTitle = () => {
                return 'ویرایش درخواست';
            }
            scopes.store('EditRequestController', $scope);

            var setOwnerFirstTime = true;
            $scope.$watch("request.Contact",
                rc => {
                    if (setOwnerFirstTime) {
                        if (rc !== null && rc !== undefined && Object.getOwnPropertyNames(rc).length > 0) {
                            $scope.originContact = JSON.parse(JSON.stringify(rc));
                            setOwnerFirstTime = false;
                        } else {
                            $scope.originContact = null;
                        }
                    }
                });

            var listener = $rootScope.$on("SetOriginContact",
            () => {
                $scope.request.Contact = $scope.originContact;

                if ($scope.request.Contact.AgencyContact != null) {
                    $scope.request.Contact.AgencyContact.Phones.forEach(p => {
                        p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                    });
                    $scope.request.Contact.AgencyContact.Emails.forEach(e => {
                        e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                    });
                    $scope.request.Contact.AgencyContact.Addresses.forEach(a => {
                        a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                    });
                }

                if ($scope.request.Contact.OwnerContact != null) {
                    $scope.request.Contact.OwnerContact.Phones.forEach(p => {
                        p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                    });
                    $scope.request.Contact.OwnerContact.Emails.forEach(e => {
                        e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                    });
                    $scope.request.Contact.OwnerContact.Addresses.forEach(a => {
                        a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                    });
                }
            });

            $scope.$on("$destroy", listener);

            $scope.onSaveClick = () => {
                $scope.request.PropertyTypes = Enumerable.from<any>($scope.request.PropertyTypeObjects)
                    .where(pt => pt.isSelected)
                    .select(pt => pt.id)
                    .toArray();
                $http.post("/api/haftdong/requests/update", $scope.request)
                    .success(() => {
                        toastr.success("درخواست با موفقیت ویرایش شد.");
                        $scope.$emit('EntityUpdated');
                        $state.go("^");
                    });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }
        }
    ]);

}