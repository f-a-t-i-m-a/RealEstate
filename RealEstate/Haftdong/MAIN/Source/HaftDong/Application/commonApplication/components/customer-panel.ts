module JahanJooy.HaftDong.Components {
    appModule.directive('jjCustomerPanel',
    () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/Customer/customer-panel.html',
            scope: {
                customer: "="
            },
            controller: ($scope) => {
                $scope.showAllCustomerPanel = false;
                $scope.showNewCustomerPanel = false;

                if ($scope.customer !== undefined && $scope.customer !== null) {
                    $scope.customers = [$scope.customer];
                    $scope.customerType = "existingCustomer";
                } else {
                    $scope.customerType = "newCustomer";
                }

                $scope.$watch("customer",
                    c => {
                        if (c !== undefined && c !== null && c.PhoneNumber != null) {
                            c.Number = c.PhoneNumber;
                        } else if (c !== undefined && c !== null && c.Contact != null && c.Contact.Phones != null && c.Contact.Phones.length > 0) {
                            c.Number = Enumerable.from(c.Contact.Phones).first().NormalizedValue;
                        }
                    });

                $scope.$watch("customerType",
                    ct => {
                        if (ct === "existingCustomer") {
                            $scope.showAllCustomerPanel = true;
                            $scope.showNewCustomerPanel = false;
                            $scope.$emit('SetOriginOwner');
                        } else if (ct === "newCustomer") {
                            $scope.showNewCustomerPanel = true;
                            $scope.showAllCustomerPanel = false;
                            $scope.customer = {};
                        }
                    });
            }
        }
    });
}