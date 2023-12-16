module JahanJooy.HaftDong.Property {
    appModule.controller("SetExpirationTimeModalController",
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
                    } else if ($scope.input.Contact.OwnerCanBeContacted === true) {
                        $scope.input.OwnerContact = $scope.input.Contact.OwnerContact;
                    }
                }

                if ($scope.entityId != null && $scope.entityId != undefined) {
                    $http.post("/api/web/supplies/publish/",
                        {
                            ID: $scope.entityId,
                            ExpirationTime: $scope.input.ExpirationTime,
                            OwnerCanBeContacted: $scope.input.OwnerCanBeContacted,
                            AgencyContact: $scope.input.AgencyContact,
                            OwnerContact: $scope.input.OwnerContact
                        })
                        .success(() => {
                            toastr.success("آگهی با موفقیت منتشر شد.");
                            $scope.$emit("EntityUpdated");
                            $modalInstance.close();
                        });
                } else {
                    $http.post("/api/web/properties/publish/",
                        {
                            ID: $scope.property.ID,
                            ExpirationTime: $scope.input.ExpirationTime,
                            OwnerCanBeContacted: $scope.input.OwnerCanBeContacted,
                            AgencyContact: $scope.input.AgencyContact,
                            OwnerContact: $scope.input.OwnerContact
                        })
                        .success(() => {
                            toastr.success("ملک با موفقیت منتشر شد.");
                            $scope.$emit("EntityUpdated");
                            $modalInstance.close();
                        });
                }
            }
        }
    ]);

}