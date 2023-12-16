module JahanJooy.HaftDong.Contract {

    appModule.controller("NewContractController", [
        '$scope', '$timeout', '$state', '$http', 'toastr', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'قرارداد جدید';
            }
            scopes.store('NewContractController', $scope);

            $scope.contract = {
                Seller: {
                    ID: null,
                    DisplayName: "",
                    NameOfFather: "",
                    Identification: null,
                    IssuedIn:"",
                    DateOfBirth: null,
                    SocialSecurityNumber: "",
                    Address: "",
                    PhoneNumber: "",
                    Deputy: {
                        DisplayName: "",
                        NameOfFather: "",
                        Identification:null
                    }
                },
                Buyer: {
                    ID:null,
                    DisplayName: "",
                    NameOfFather: "",
                    Identification: null,
                    IssuedIn: "",
                    DateOfBirth: null,
                    SocialSecurityNumber: "",
                    Address: "",
                    PhoneNumber: "",
                    Deputy: {
                        DisplayName: "",
                        NameOfFather: "",
                        Identification: null
                    }
                }               
            };
          
            $scope.onSaveClick = () => {
                $scope.contract.SellerID = $scope.contract.Seller.ID;
                $scope.contract.BuyerID = $scope.contract.Buyer.ID;
                $http.post("/api/web/contracts/save", $scope.contract).success((data: any) => {
                    toastr.success("قرارداد با موفقیت ثبت شد.");
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