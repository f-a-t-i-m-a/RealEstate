module JahanJooy.HaftDong.Api {
    ngModule.service("apiAccount", [
        "$http", "$q",
        ($http, $q) => {

            return {
                login(email, password) {
                    var data = "grant_type=password" +
                        "&username=" + encodeURIComponent(email) +
                        "&password=" + encodeURIComponent(password);

                    var deferred = $q.defer();

                    $http.post(
                        "/token",
                        data,
                        {
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            }
                        }
                    ).success(response => {
                        deferred.resolve(response);
                    }).error((err, status) => {
                        deferred.reject(err);
                    });

                    return deferred.promise;
                },
                getPermissions() {
                    return $http.get("/api/web/account/permissions");
                },
                changePassword(oldPassword, newPassword, confirmPassword) {
                    var data = {
                        OldPassword: oldPassword,
                        NewPassword: newPassword,
                        ConfirmPassword: confirmPassword
                    };

                    return $http.post(
                        "/api/web/account/changepassword",
                        data);
                }
            };
        }
    ]);

}
