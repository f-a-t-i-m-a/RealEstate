module JahanJooy.HaftDong.File {

    appModule.controller("EditSupplyModalController", [
        '$scope', '$timeout', '$state', '$http', "toastr", "$modalInstance", "$rootScope",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, toastr, $modalInstance, $rootScope) => {

            $scope.$watch("supply", s => {
                if (s != null && s.PriceSpecificationType === 1) {
                    s.Price = s.TotalPrice;
                } else if (s != null && s.PriceSpecificationType === 2) {
                    s.Price = s.PricePerEstateArea;
                } else if (s != null &&s.PriceSpecificationType === 3) {
                    s.Price = s.PricePerUnitArea;
                }
            });

            var setOwnerFirstTime = true;
            $scope.$watch("supply.Contact",
                rc => {
                    if (setOwnerFirstTime && $scope.supply.IsPublic) {
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
                    $scope.supply.Contact = $scope.originContact;

                    if ($scope.supply.Contact.AgencyContact != null) {
                        $scope.supply.Contact.AgencyContact.Phones.forEach(p => {
                            p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                        });
                        $scope.supply.Contact.AgencyContact.Emails.forEach(e => {
                            e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                        });
                        $scope.supply.Contact.AgencyContact.Addresses.forEach(a => {
                            a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                        });
                    }

                    if ($scope.supply.Contact.OwnerContact != null) {
                        $scope.supply.Contact.OwnerContact.Phones.forEach(p => {
                            p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                        });
                        $scope.supply.Contact.OwnerContact.Emails.forEach(e => {
                            e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                        });
                        $scope.supply.Contact.OwnerContact.Addresses.forEach(a => {
                            a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                        });
                    }
                });

            $scope.$on("$destroy", listener);

            $scope.onSaveClick = () => {
                if ($scope.supply.Contact !== null) {
                    $scope.supply.OwnerCanBeContacted = $scope.supply.Contact.OwnerCanBeContacted;
                    if ($scope.supply.Contact.OwnerCanBeContacted === true) {
                        $scope.supply.OwnerContact = $scope.supply.Contact.OwnerContact;
                    } else if ($scope.supply.Contact.OwnerCanBeContacted === false) {
                        $scope.supply.AgencyContact = $scope.supply.Contact.AgencyContact;
                    }
                }

                $http.post("/api/web/supplies/update", $scope.supply).success(() => {
                    toastr.success("آگهی با موفقیت ویرایش شد.");
                    $modalInstance.close();
                });
            }

            $scope.onBackClick = () => {
                $modalInstance.dismiss('cancel');
            }
        }
    ]);

}