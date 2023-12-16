module JahanJooy.HaftDong.User {
    import UserType = RealEstateAgency.Domain.Enums.User.UserType;
    import EnumExtentions = Common.EnumExtentions;
    import ContactMethodType = RealEstateAgency.Domain.Enums.User.ContactMethodType;
    appModule.directive('jjUser', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/adminApplication/views/Users/users.directive.html',
            scope: {
                user: "=",
                onBackClick: '&',
                onSaveClick: '&'
            },
            controller: ($scope) => {
               
                $scope.showIcon = false;
                $scope.contact = {
                    mobileNumber: "",
                    address: "",
                    email: ""
                }


                $scope.$watch("user", u => {
                    if (u != null) {
                        $scope.inEditMode = !(u.Id == null);
                        $scope.contact.mobileNumber = u.Contact.Phones[0].NormalizedValue;
                        $scope.contact.email = u.Contact.Emails[0].NormalizedValue;
                        $scope.contact.address = u.Contact.Addresses[0].NormalizedValue;
                    }
                });

                $scope.checkPassword = () => {
                    if ($scope.user.Password == null ||
                        ($scope.user.Password != null && $scope.user.Password.length < 6))
                        $scope.PasswordLengthMinimumRequirement = true;
                    else
                        $scope.PasswordLengthMinimumRequirement = false;

                    if ($scope.user.Password != null && $scope.user.Password !== "")
                        $scope.showIcon = true;
                    else
                        $scope.showIcon = false;
                }
                
                $scope.allUserTypes = EnumExtentions.getValues(UserType).select(i => { return { id: i, text: Common.localization.translateEnum("UserType", i) } }).toArray();

                $scope.onSave = () => {
                    var emailInfo = {
                        Type: ContactMethodType.Email,
                        Value: $scope.contact.email
                    }
                    var mobileInfo = {
                        Type: ContactMethodType.Phone,
                        Value: $scope.contact.mobileNumber
                    }
                    var addressInfo = {
                        Type: ContactMethodType.Address,
                        Value: $scope.contact.address
                    }

                    $scope.user.ContactInfos = [];
                    $scope.user.ContactInfos.push(emailInfo);
                    $scope.user.ContactInfos.push(mobileInfo);
                    $scope.user.ContactInfos.push(addressInfo);

                    $scope.onSaveClick();
                }
            }
        }
    });
}