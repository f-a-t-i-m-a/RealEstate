module JahanJooy.HaftDong {
    import ContactMethodType = RealEstateAgency.Domain.Enums.User.ContactMethodType;
    appModule.controller("SignupController", [
        '$scope', '$timeout', '$state', '$http', 'authService', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, authService, scopes) => {

            $scope.GetTitle = () => {
                return 'ثبت نام';
            }
            scopes.store('SignupController', $scope);

            $scope.showIcon = false;

            $scope.user = {};


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

            $scope.onBackClick = () => {
                window.history.back();
            }

            $scope.onSaveClick = () => {
                var emailInfo = {
                    Type:ContactMethodType.Email,
                    Value :$scope.email
                }
                var mobileInfo = {
                    Type: ContactMethodType.Phone,
                    Value: $scope.mobileNumber
                }
                var addressInfo = {
                    Type: ContactMethodType.Address,
                    Value: $scope.address
                }

                $scope.user.ContactInfos = [];
                $scope.user.ContactInfos.push(emailInfo);
                $scope.user.ContactInfos.push(mobileInfo);
                $scope.user.ContactInfos.push(addressInfo);
                   
                $http.post("/api/web/account/signup", $scope.user)
                    .success(() => {
                        authService.attemptLogin($scope.user.UserName, $scope.user.Password, false)
                            .then(() => {
                                $state.go("profile");
                            });
                    });
            }
        }
    ]);

}