module JahanJooy.HaftDong.Customer {
    appModule.controller("NewCustomerController", [
        '$scope', '$timeout', '$state', '$http', 'toastr', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'مشتری جدید';
            }
            scopes.store('NewCustomerController', $scope);

            $scope.customer = {
                DisplayName: "",
                ContactInfos: [],
                Emails: [],
                PhoneNumbers: [],
                Description: "",
                IsMarried: "",
                NameOfFather: '',
                Identification: null,
                IssuedIn: '',
                SocialSecurityNumber: '',
                DateOfBirth: null,
                Deputy: {
                    DisplayName: '',
                    NameOfFather: '',
                    Identification: null,
                    SocialSecurityNumber: '',
                    DateOfBirth:null
                }
            };
          
            $scope.onSaveClick = () => {
                $http.post("/api/web/customers/save", $scope.customer).success((data: any) => {
                    toastr.success("مشتری با موفقیت ثبت شد.");
                    $scope.$emit('EntityUpdated');
                    $state.go("^.details", { id: data.ID });
                });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }
        }
    ]);

}