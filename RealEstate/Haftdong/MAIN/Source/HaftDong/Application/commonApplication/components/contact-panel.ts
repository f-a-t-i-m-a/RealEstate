module JahanJooy.HaftDong.Components {
    appModule.directive('jjContactPanel',
    () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/Contact/contact-panel.html',
            scope: {
                contact: "=",
                showOwnerContact: "="
            },
            controller: ($scope) => {
                $scope.showOwnerPanel = false;
                $scope.showAgencyPanel = false;

                if ($scope.contact !== undefined && $scope.contact !== null && !$scope.contact.OwnerCanBeContacted) {
                    $scope.contactType = "newAgencyContact";
                } else if ($scope.contact !== undefined &&
                    $scope.contact !== null &&
                    $scope.contact.OwnerCanBeContacted &&
                    $scope.contact.OwnerContact != null) {
                    $scope.contactType = "newOwnerContact";
                } else {
                    $scope.contactType = "currentUser";
                };

                $scope.$watch("contactType",
                    ct => {
                        if (ct === "newOwnerContact") {
                            $scope.$emit('SetOriginContact');
                            $scope.showAgencyPanel = false;
                            $scope.showOwnerPanel = true;
                            $scope.contact.OwnerCanBeContacted = true;
                            if ($scope.contact.OwnerContact == null) {
                                $scope.contact.OwnerContact = {
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
                                if ($scope.contact.OwnerContact.Phones == null ||
                                    $scope.contact.OwnerContact.Phones.length === 0) {
                                    $scope.contact.OwnerContact.Phones = [
                                        {
                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                                        }
                                    ];
                                }

                                if ($scope.contact.OwnerContact.Emails == null ||
                                    $scope.contact.OwnerContact.Emails.length === 0) {
                                    $scope.contact.OwnerContact.Emails = [
                                        {
                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                                        }
                                    ];
                                }
                            }
                        } else if (ct === "newAgencyContact") {
                            $scope.$emit('SetOriginContact');
                            $scope.showAgencyPanel = true;
                            $scope.showOwnerPanel = false;
                            $scope.contact.OwnerCanBeContacted = false;
                            if ($scope.contact.AgencyContact == null) {
                                $scope.contact.AgencyContact = {
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
                                if ($scope.contact.AgencyContact.Phones == null ||
                                    $scope.contact.AgencyContact.Phones.length === 0) {
                                    $scope.contact.AgencyContact.Phones = [
                                        {
                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                                        }
                                    ];
                                }

                                if ($scope.contact.AgencyContact.Emails == null ||
                                    $scope.contact.AgencyContact.Emails.length === 0) {
                                    $scope.contact.AgencyContact.Emails = [
                                        {
                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                                        }
                                    ];
                                }

                                if ($scope.contact.AgencyContact.Addresses == null ||
                                    $scope.contact.AgencyContact.Addresses.length === 0) {
                                    $scope.contact.AgencyContact.Addresses = [
                                        {
                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
                                        }
                                    ];
                                }
                            }
                        } else {
                            $scope.showAgencyPanel = false;
                            $scope.showOwnerPanel = false;
                            $scope.contact = {};
                            $scope.$emit('SetOriginContact');
                        }
                    });

                $scope.onAddAgencyPhoneNumbers = () => {
                    var phone = {
                        Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                    };
                    $scope.contact.AgencyContact.Phones.push(phone);
                }

                $scope.onDeleteAgencyPhoneNumber = (index) => {
                    $scope.contact.AgencyContact.Phones.splice(index, 1);
                }

                $scope.onAddAgencyAddresses = () => {
                    var address = {
                        Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
                    };
                    $scope.contact.AgencyContact.Addresses.push(address);
                }

                $scope.onDeleteAgencyAddress = (index) => {
                    $scope.contact.AgencyContact.Addresses.splice(index, 1);
                }

                $scope.onAddAgencyEmails = () => {
                    var email = {
                        Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                    };
                    $scope.contact.AgencyContact.Emails.push(email);
                }

                $scope.onDeleteAgencyEmail = (index) => {
                    $scope.contact.AgencyContact.Emails.splice(index, 1);
                }

                $scope.onAddOwnerPhoneNumbers = () => {
                    var phone = {
                        Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                    };
                    $scope.contact.OwnerContact.Phones.push(phone);
                }

                $scope.onDeleteOwnerPhoneNumber = (index) => {
                    $scope.contact.OwnerContact.Phones.splice(index, 1);
                }

                $scope.onAddOwnerEmails = () => {
                    var email = {
                        Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                    };
                    $scope.contact.OwnerContact.Emails.push(email);
                }

                $scope.onDeleteOwnerEmail = (index) => {
                    $scope.contact.OwnerContact.Emails.splice(index, 1);
                }
            }
        }
    });
}