module JahanJooy.HaftDong.Customer {
    appModule.controller("EditRequestController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "$rootScope", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, $rootScope, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'ویرایش درخواست';
            }
            scopes.store('EditRequestController', $scope);

            var setOwnerFirstTime = true;
            $scope.$watch("request.Contact",
                rc => {
                    if (setOwnerFirstTime) {
                        if (rc !== null && rc !== undefined && Object.getOwnPropertyNames(rc).length > 0) {
                            $scope.prepareContacts();
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
                $http.post("/api/web/requests/update", $scope.request).success(() => {
                    toastr.success("درخواست با موفقیت ویرایش شد.");
                    $scope.$emit('EntityUpdated');
                    $state.go("^");
                });
            }

            $scope.prepareContacts = () => {
                if ($scope.request.Contact.AgencyContact == null) {
                    $scope.request.Contact.AgencyContact = {
                        Phones: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ],
                        Emails: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ],
                        Addresses: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
                            }
                        ]
                    };
                } else {
                    if ($scope.request.Contact.AgencyContact.Phones == null ||
                        $scope.request.Contact.AgencyContact.Phones.length === 0) {
                        $scope.request.Contact.AgencyContact.Phones = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ];
                    }

                    if ($scope.request.Contact.AgencyContact.Emails == null ||
                        $scope.request.Contact.AgencyContact.Emails.length === 0) {
                        $scope.request.Contact.AgencyContact.Emails = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ];
                    }

                    if ($scope.request.Contact.AgencyContact.Addresses == null ||
                        $scope.request.Contact.AgencyContact.Addresses.length === 0) {
                        $scope.request.Contact.AgencyContact.Addresses = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
                            }
                        ];
                    }
                }


                if ($scope.request.Contact.OwnerContact == null) {
                    $scope.request.Contact.OwnerContact = {
                        Phones: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ],
                        Emails: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ],
                        Addresses: []
                    };
                } else {
                    if ($scope.request.Contact.OwnerContact.Phones == null ||
                        $scope.request.Contact.OwnerContact.Phones.length === 0) {
                        $scope.request.Contact.OwnerContact.Phones = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ];
                    }

                    if ($scope.request.Contact.OwnerContact.Emails == null ||
                        $scope.request.Contact.OwnerContact.Emails.length === 0) {
                        $scope.request.Contact.OwnerContact.Emails = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ];
                    }
                }
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }
        }
    ]);

}