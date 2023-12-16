module JahanJooy.HaftDong.Request {
    appModule.controller("SetRequestExpirationTimeModalController",
    [
        '$scope', '$http', '$state', '$modalInstance', 'toastr',
        ($scope, $http: ng.IHttpService, $state, $modalInstance, toastr) => {

            $scope.cancel = () => {
                $modalInstance.dismiss('cancel');
            };

            $scope.input = {
                Contact: {}
            }

            $scope.onPublishClick = () => {
                if ($scope.input.Contact !== null) {
                    $scope.input.OwnerCanBeContacted = $scope.input.Contact.OwnerCanBeContacted;
                    if ($scope.input.Contact.OwnerCanBeContacted === false) {
                        $scope.input.AgencyContact = $scope.input.Contact.AgencyContact;
                    } else if ($scope.input.Contact.OwnerCanBeContacted === false) {
                        $scope.input.OwnerContact = $scope.input.Contact.OwnerContact;
                    }
                }

                $http.post("/api/web/requests/publish/",
                    {
                        ID: $scope.request.ID,
                        ExpirationTime: $scope.input.ExpirationTime,
                        OwnerCanBeContacted: $scope.input.OwnerCanBeContacted,
                        AgencyContact: $scope.input.AgencyContact,
                        OwnerContact: $scope.input.OwnerContact
                    })
                    .success(() => {
                        toastr.success("درخواست با موفقیت منتشر شد.");
                        $scope.$emit("EntityUpdated");
                        $modalInstance.close();
                    });
            }
        }
    ]);

}