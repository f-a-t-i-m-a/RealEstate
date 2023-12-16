module JahanJooy.HaftDong.Customer {

    appModule.directive('jjCustomer',
    () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/userApplication/views/Customers/customers.directive.html',
            scope: {
                customer: "=",
                onBackClick: '&',
                onSaveClick: '&'
            },

            controller: ($scope) => {

                $scope.moreInfo = true;
                $scope.hasDeputy = true;

                if ($scope.customer.Contact == null) {
                    $scope.customer.Contact = {
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
                    if ($scope.customer.Contact.Phones == null ||
                        $scope.customer.Contact.Phones.length === 0) {
                        $scope.customer.Contact.Phones = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ];
                    }

                    if ($scope.customer.Contact.Emails == null ||
                        $scope.customer.Contact.Emails.length === 0) {
                        $scope.customer.Contact.Emails = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ];
                    }

                    if ($scope.customer.Contact.Addresses == null ||
                        $scope.customer.Contact.Addresses.length === 0) {
                        $scope.customer.Contact.Addresses = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
                            }
                        ];
                    }
                }

                $scope.onAddPhoneNumbers = () => {
                    var phone = {
                        Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                    };
                    $scope.customer.Contact.Phones.push(phone);
                }

                $scope.onDeletePhoneNumber = (index) => {
                    $scope.customer.Contact.Phones.splice(index, 1);
                }

                $scope.onAddAddresses = () => {
                    var address = {
                        Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
                    };
                    $scope.customer.Contact.Addresses.push(address);
                }

                $scope.onDeleteAddress = (index) => {
                    $scope.customer.Contact.Addresses.splice(index, 1);
                }

                $scope.onAddEmails = () => {
                    var email = {
                        Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                    };
                    $scope.customer.Contact.Emails.push(email);
                }

                $scope.onDeleteEmail = (index) => {
                    $scope.customer.Contact.Emails.splice(index, 1);
                }

                $scope.onSave = () => {
                    $scope.onSaveClick();
                }
            }
        }
    });

    appModule.directive('autoNext',
    () => {
        return {
            restrict: 'A',
            link(scope, element, attr) {
                var tabindex = parseInt(attr['tabindex']);
                var maxLength = parseInt(attr['ngMaxlength']);
                element.on('keypress',
                    e => {
                        if (element.val().length >= maxLength - 1) {
                            var next = angular.element(document.body).find('[tabindex=' + (tabindex + 1) + ']');
                            if (next.length > 0) {
                                next.focus();
                                return null;
                            } else {
                                return false;
                            }
                        }
                        return true;
                    });
            }
        }
    });

}