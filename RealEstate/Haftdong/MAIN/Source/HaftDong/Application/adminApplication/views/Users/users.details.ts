module JahanJooy.HaftDong.User {
    appModule.controller("UserDetailsController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "$rootScope", "messageBoxService", "toastr", "$modal",
        "Upload", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, $rootScope, messageBoxService, toastr,
            $modal, Upload, scopes) => {

            $scope.GetTitle = () => {
                return 'جزئیات کاربر';
            }
            scopes.store('UserDetailsController', $scope);

            $scope.isContactCollapsed = true;
            $scope.isLoginCollapsed = true;

//            $scope.user = {
//                EmailAddress: "",
//                MobileNumber: ""
//                
//            };

            function initialize() {
                $http.get("/api/web/users/get/" + $stateParams.id, null).success((data: any) => {
                    $scope.user = data;
                    $scope.mobileNumberList = $scope.user.Contact.Phones;
                    $scope.emailAddressList = $scope.user.Contact.Emails;
                    $scope.addressList = $scope.user.Contact.Addresses;
                    $scope.isNotExpired = moment($scope.user.LicenseActivationTime).format("YYYY/MM/DD") >= moment().format("YYYY/MM/DD");
                    if ($scope.user.ProfilePicture != null && $scope.user.ProfilePicture.ID != null && $scope.user.ProfilePicture.DeletionTime == null) {
                        $http.get("/api/web/users/getSmallSize/" + $scope.user.ProfilePicture.ID, { responseType: "blob" })
                            .success((data: Blob) => {
                                $scope.user.ProfilePicture.blob = URL.createObjectURL(data);
                            });
                    }
                });

                $http.get("/api/web/supplies/getallsuppliesbycreatedbyid/" + $stateParams.id, null
                    ).success((data: any) => {
                        $scope.Supplies = data;
                    });

                $http.get("/api/web/requests/getallrequestsbycreatedbyid/" + $stateParams.id, null
                    ).success((data: any) => {
                        $scope.Requests = data;
                    });
                $http.get("/api/web/contracts/getallcontractsbycreatedbyid/" + $stateParams.id, null
                    ).success((data: any) => {
                        $scope.Contracts = data;
                    });
                $http.get("/api/web/customers/getallcustomersbycreatedbyid/" + $stateParams.id, null
                    ).success((data: any) => {
                        $scope.Customers = data;
                    });
            }

            $scope.upload = file => {
                if (file) {
                    Upload.upload({
                        url: '/api/web/users/addimage',
                        fields: {
                            userId: $stateParams.id
                        },
                        file: file
                    }).success(() => {
                        $scope.$emit('EntityUpdated');
                    });
                }
            };

            $scope.onShowImageClick = (image) => {
                if (!(image.OriginalFileExtension.toLowerCase() === ".jpg" ||
                    image.OriginalFileExtension.toLowerCase() === ".gif" ||
                    image.OriginalFileExtension.toLowerCase() === ".png" ||
                    image.OriginalFileExtension.toLowerCase() === ".bmp"))
                    return;
                $scope.selectedImage = image;
                $modal.open({
                    templateUrl: 'Application/adminApplication/views/Users/user.image.modal.html',
                    controller: 'UserImageModalController',
                    scope: $scope,
                    size: 'lg'
                });
            }

            $scope.onDeleteClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این کاربر را حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/users/delete/" + $scope.user.Id, null).success((data: any) => {
                        toastr.success("کاربر با موفقیت حذف شد.");
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onApproveClick = () => {
                var confirmResult = messageBoxService.confirm("آیا می خواهید عضویت این کاربر را تائید کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/users/approve", {
                        Ids: [$scope.user.Id]
                    }).success((data: any) => {
                        toastr.success("عضویت کاربر با موفقیت تائید شد.");
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onResetPassword = () => {
                $scope.title = 'ایجاد رمز جدید...';
                $scope.UserId = $scope.user.Id;
                var instance = $modal.open({
                    templateUrl: "/Application/adminApplication/views/Users/user.reset.password.modal.html",
                    controller: "UserResetPasswordModalController",
                    size: "md",
                    scope: $scope
                });
            };

            $scope.onEnableClick = () => {
                var confirmResult = $scope.user.IsEnabled ?
                    messageBoxService.confirm("آیا می خواهید حساب این کاربر را غیر فعال کنید؟") :
                    messageBoxService.confirm("آیا می خواهید حساب این کاربر را فعال کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/users/enable/" + $scope.user.Id, null).success((data: any) => {
                        $scope.user.IsEnabled ?
                            toastr.success("حساب کاربر با موفقیت غیر فعال شد.") :
                            toastr.success("حساب کاربر با موفقیت فعال شد.");
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.computePrice = (entity) => {
                if (entity.PriceSpecificationType === null) {
                    return 0;
                } else if (entity.PriceSpecificationType === 1) {
                    return entity.TotalPrice;
                } else if (entity.PriceSpecificationType === 2 && entity.Property.EstateArea != null) {
                    return entity.PricePerEstateArea * entity.Property.EstateArea;
                } else if (entity.PriceSpecificationType === 3 && entity.Property.UnitArea != null) {
                    return entity.PricePerUnitArea * entity.Property.UnitArea;
                }
                return 0;
            }

            initialize();

            var listener = $rootScope.$on('EntityUpdated', () => {
                initialize();
            });

            $scope.$on('$destroy', listener);

        }
    ]);

}