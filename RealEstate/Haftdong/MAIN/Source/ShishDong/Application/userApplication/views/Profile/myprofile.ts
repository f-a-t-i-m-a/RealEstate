module JahanJooy.ShishDong.Dashboard {
    import ContactMethodType = RealEstateAgency.Domain.Enums.User.ContactMethodType;
    ngModule.controller("ProfileController",
    [
        '$scope', '$rootScope', '$http', '$state', '$modal', "messageBoxService", "$stateParams",
        'toastr', 'authService', '$filter', '$sce', 'scopes',
        ($scope,
            $rootScope,
            $http: ng.IHttpService,
            $state,
            $modal,
            messageBoxService,
            $stateParams,
            toastr,
            authService,
            $filter,
            $sce,
            scopes) => {

            $scope.GetTitle = () => {
                return 'صفحه شخصی';
            }
            scopes.store('ProfileController', $scope);


            $scope.showEmailVerificationPanel = false;
            $scope.showAddEmailAddressPanel = false;
            $scope.restartEmailAddressVerificationDiv = false;
            $scope.showEmailPanel = false;
            $scope.expiredEmailContactMethodVerificationDiv = false;

            $scope.showMobileVerificationPanel = false;
            $scope.showAddMobilePanel = false;
            $scope.restartMobileVerificationDiv = false;
            $scope.showMobilePanel = false;
            $scope.expiredMobileContactMethodVerificationDiv = false;

            $scope.user = {
                EmailAddress: "",
                MobileNumber: ""
            };

            $scope.mobileNumberList = [];
            $scope.emailAddressList = [];

            $scope.isCollapsed = true;

            function showPanels(user) {
                if (user.Contact.Phones.length > 0) {
                    user.Contact.Phones.forEach(p => {
                        if (p.IsVerified) {
                            $scope.showMobilePanel = false;
                            $scope.showAddMobilePanel = false;
                        } else {
                            $scope.showMobilePanel = true;
                            $scope.showAddMobilePanel = false;

                            if (!p.IsVerified) {
                                $scope.showMobileVerificationPanel = true;
                            } else {
                                $scope.showMobileVerificationPanel = false;
                            }
                            }
                    });
                        } else {
                            $scope.showMobilePanel = true;
                                $scope.showAddMobilePanel = true;
                            }
                if (user.Contact.Emails.length > 0) {
                    user.Contact.Emails.forEach(e => {
                        if (e.IsVerified) {
                            $scope.showEmailPanel = false;
                            $scope.showAddEmailAddressPanel = false;
                        } else {
                            $scope.showEmailPanel = true;
                            $scope.showAddEmailAddressPanel = false;

                            if (!e.IsVerified) {
                                $scope.showEmailVerificationPanel = true;
                            } else {
                                $scope.showEmailVerificationPanel = false;
                            }
                        }
                    });
                } else {
                    $scope.showEmailPanel = true;
                    $scope.showAddEmailAddressPanel = true;
                }
            }

            function initialize() {
                $http.get("/api/haftdong/users/myprofile", null)
                    .success((data: any) => {
                    $scope.user = data;
                    showPanels($scope.user);
                        $scope.isNotExpired = moment($scope.user.LicenseActivationTime).format("YYYY/MM/DD") >=
                            moment().format("YYYY/MM/DD");
                    fillContactMethodsList(data);

                });
            }

            initialize();

            function fillContactMethodsList(data) {
                $scope.mobileNumberList = [];
                $scope.emailAddressList = [];
                if (data.Contact != null) {
                    if (data.Contact.Phones.length > 0) {
                        data.Contact.Phones.forEach(p => {
                            $scope.mobileNumberList.push({
                                ContactMethodText: p.NormalizedValue,
                                IsVerified: p.IsVerified
                            });
                            });
                        }
                    if (data.Contact.Emails.length > 0) {
                        data.Contact.Emails.forEach(e => {
                            $scope.emailAddressList.push({
                                ContactMethodText: e.NormalizedValue,
                                IsVerified: e.IsVerified
                            });
                    });
                }
            }
            }

            $scope.onAddNewEmail = () => {
                if ($scope.user.EmailAddress == null || $scope.user.EmailAddress === "") {
                    return;
                }

                $scope.newContactMethodInputForApplicationUser = {
                    UserId: $scope.user.Id,
                    Type: ContactMethodType.Email,
                    Value: $scope.user.EmailAddress
                }
                $http.post("/api/haftdong/users/addnewcontactmethod", $scope.newContactMethodInputForApplicationUser)
                    .success((data: any) => {
                    $scope.user = data;
                    showPanels($scope.user);
                    toastr.success("ایمیل با موفقیت ثبت شد.");
                    })
                    .error(() => {
                    toastr.error("ثبت ایمیل با مشکل مواجه شد.");
                });
            }

            $scope.onAddNewMobileNumber = () => {
                if ($scope.user.MobileNumber == null || $scope.user.MobileNumber === "") {
                    return;
                }

                $scope.newContactMethodInputForApplicationUser = {
                    UserId: $scope.user.Id,
                    Type: ContactMethodType.Phone,
                    Value: $scope.user.MobileNumber
                }
                $http.post("/api/haftdong/users/addnewcontactmethod", $scope.newContactMethodInputForApplicationUser)
                    .success((data: any) => {
                    $scope.user = data;
                    showPanels($scope.user);
                    toastr.success("موبایل با موفقیت ثبت شد.");
                    })
                    .error(() => {
                    toastr.error("ثبت موبایل با مشکل مواجه شد.");
                });
            }

            $scope.onCompleteRegistrationVerifyEmailAddress = () => {
                var contactMethod = Enumerable.from<any>($scope.user.Contact.Emails).first(e => !e.IsVerified);
                $scope.UserApplicationVerifyContactMethodInput = {
                    UserID: $scope.user.Id,
                    ContactMethodID: contactMethod.ID,
                    VerificationSecret: $scope.user.EmailVerificationSecret
                }
                $http.post("/api/haftdong/users/verifycontactmethod", $scope.UserApplicationVerifyContactMethodInput)
                    .success((data: any) => {
                    $scope.user = data.User;
                    showPanels($scope.user);
                    $scope.showAddEmailAddressPanel = false;
                    fillContactMethodsList(data);
                    toastr.success("ایمیل با موفقیت فعال سازی شده و توسط سایت قابل استفاده است. متشکریم.");
                    authService.permissionUpdate(data.Token);
                    location.reload();
                    })
                    .error((data: any) => {
                    if (data.Errors.length > 0) {
                            var result = Enumerable.from<any>(data.Errors)
                                .any(e => e.ErrorKey === "VerificationDeadlineExpired");
                        if (result) {
                            $scope.expiredEmailContactMethodVerificationDiv = true;
                            $scope.showAddEmailAddressPanel = false;
                            $scope.showEmailVerificationPanel = false;
                        }
                        result = Enumerable.from<any>(data.Errors).any(e => e.ErrorKey === "InvalidSecret");
                        if (result) {
                            $scope.expiredEmailContactMethodVerificationDiv = false;
                            $scope.showAddEmailAddressPanel = false;
                            $scope.showEmailVerificationPanel = true;
                        }
                    }
                    $scope.user.EmailVerificationSecret = "";
                });
            }

            $scope.onCompleteRegistrationVerifyMobileNumber = () => {
                var contactMethod = Enumerable.from<any>($scope.user.Contact.Phones)
                    .first(p => !p.IsVerified);
                $scope.UserApplicationVerifyContactMethodInput = {
                    UserID: $scope.user.Id,
                    ContactMethodID: contactMethod.ID,
                    VerificationSecret: $scope.user.MobileVerificationSecret
                }
                $http.post("/api/haftdong/users/verifycontactmethod", $scope.UserApplicationVerifyContactMethodInput)
                    .success((data: any) => {
                    $scope.user = data.User;
                    showPanels($scope.user);
                    $scope.showAddmobilePanel = false;
                    fillContactMethodsList(data);
                    toastr.success("موبایل با موفقیت فعال سازی شده و توسط سایت قابل استفاده است. متشکریم.");
                    authService.permissionUpdate(data.Token).then(() => {
                        location.reload();
                    });
                    })
                    .error((data: any) => {
                    if (data.Errors.length > 0) {
                            var result = Enumerable.from<any>(data.Errors)
                                .any(e => e.ErrorKey === "VerificationDeadlineExpired");
                        if (result) {
                            $scope.expiredMobileContactMethodVerificationDiv = true;
                            $scope.showAddMobilePanel = false;
                            $scope.showMobileVerificationPanel = false;
                        }
                    }
                    $scope.user.MobileVerificationSecret = "";
                });
            }

            $scope.onRestartEmailAddressVerification = () => {
                var contactMethod = Enumerable.from<any>($scope.user.Contact.Emails)
                    .first(e => !e.IsVerified);
                $scope.NewSecretForUserApplicationInput = {
                    UserID: $scope.user.Id,
                    ContactMethodID: contactMethod.ID,
                    ContactMethodType: ContactMethodType.Email
                }
                $http.post("/api/haftdong/users/getanothersecret", $scope.NewSecretForUserApplicationInput)
                    .success(() => {
                    $scope.showEmailVerificationPanel = true;
                    $scope.restartEmailAddressVerificationDiv = false;
                    $scope.expiredEmailContactMethodVerificationDiv = false;
                    toastr.success("رمز فعال سازی برای شما دوباره ارسال شد");
                    })
                    .error((data: any) => {
                    if (data.Errors.length > 0) {
                            var result = Enumerable.from<any>(data.Errors)
                                .any(e => e.ErrorKey === "IsTooFrequentContactMethodVerification");
                        if (result) {
                            $scope.restartEmailAddressVerificationDiv = false;
                        }
                    }
                });
            }

            $scope.onRestartMobileVerification = () => {
                var contactMethod = Enumerable.from<any>($scope.user.Contact.Phones)
                    .first(p => !p.IsVerified);
                $scope.NewSecretForUserApplicationInput = {
                    UserID: $scope.user.Id,
                    ContactMethodID: contactMethod.ID,
                    ContactMethodType: ContactMethodType.Phone
                }
                $http.post("/api/haftdong/users/getanothersecret", $scope.NewSecretForUserApplicationInput)
                    .success(() => {
                    $scope.showMobileVerificationPanel = true;
                    $scope.restartMobileVerificationDiv = false;
                    $scope.expiredMobileContactMethodVerificationDiv = false;
                    toastr.success("رمز فعال سازی برای شما دوباره ارسال شد");
                    })
                    .error((data: any) => {
                    if (data.Errors.length > 0) {
                            var result = Enumerable.from<any>(data.Errors)
                                .any(e => e.ErrorKey === "IsTooFrequentContactMethodVerification");
                        if (result) {
                            $scope.restartMobileVerificationDiv = false;
                        }
                    }
                });
            }
        }
    ]);

}