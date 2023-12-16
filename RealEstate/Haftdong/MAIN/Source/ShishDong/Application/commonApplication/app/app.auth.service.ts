module JahanJooy.ShishDong {
    appModule.service("authService", [
        "$rootScope", "$modal", "$window", "$q", "apiAccount",
        ($rootScope, $modal, $window: ng.IWindowService, $q: ng.IQService, apiAccount) => {

            var storageId: string = "JahanJooy.RealEstateAgency.CurrentUser";
            var currentUser;

            function storeCredentials(credentials, persistent) {
                currentUser = credentials;
                $rootScope.currentUserEmail = credentials.email;
                $window.sessionStorage.setItem(storageId, JSON.stringify(currentUser));

                if (persistent) {
                    $window.localStorage.setItem(storageId, JSON.stringify(currentUser));
                } else {
                    $window.localStorage.removeItem(storageId);
                }
            }

            function updateCredentials(credentials) {
                currentUser = credentials;
                $rootScope.currentUserEmail = credentials.email;
                $window.sessionStorage.setItem(storageId, JSON.stringify(currentUser));
            }

            function onLoginSuccessful(email, tokenData, permsData, persistent) {
                permsData = permsData || {};
                currentUser = {
                    email: email,
                    token: tokenData.access_token,
                    authenticationTime: new Date(),
                    user: permsData.User || {},
                    roles: permsData.Roles || [],
                    roleNames: permsData.RoleNames || [],
                    isAdministrator: false,
                    isVerified: false
                };

                if (Enumerable.from(<string[]> currentUser.roleNames).any(roleName => roleName.toUpperCase() === "ADMINISTRATOR")) {
                    currentUser.isAdministrator = true;
                    $rootScope.isAdministrator = currentUser.isAdministrator;
                }

                if (Enumerable.from(<string[]>currentUser.roleNames).any(roleName => roleName.toUpperCase() === "VERIFIEDUSER")) {
                    currentUser.isVerified = true;
                    $rootScope.isVerified = currentUser.isVerified;
                }

                storeCredentials(currentUser, persistent);
                return currentUser;
            }

            function updatePermissions(permsData, token) {
                permsData = permsData || {};
                currentUser.isAdministrator = false;
                currentUser.isVerified = false;
                currentUser.user = permsData.User || {};
                currentUser.roles = permsData.Roles || [];
                currentUser.roleNames = permsData.RoleNames || [];

                if (token !== "") {
                    currentUser.token = token;
                }

                if (Enumerable.from(<string[]>currentUser.roleNames).any(roleName => roleName.toUpperCase() === "ADMINISTRATOR")) {
                    currentUser.isAdministrator = true;
                    $rootScope.isAdministrator = currentUser.isAdministrator;
                }

                if (Enumerable.from(<string[]>currentUser.roleNames).any(roleName => roleName.toUpperCase() === "VERIFIEDUSER")) {
                    currentUser.isVerified = true;
                    $rootScope.isVerified = currentUser.isVerified;
                }

                updateCredentials(currentUser);
                return currentUser;
            }

            function clearStoredCredentials() {
                currentUser = undefined;
                $rootScope.currentUserEmail = undefined;
                $window.sessionStorage.removeItem(storageId);
                $window.localStorage.removeItem(storageId);
            }

            function restoreCredentials() {
                var storageUserString =
                    $window.sessionStorage.getItem(storageId) ||
                    $window.localStorage.getItem(storageId);

                if (storageUserString) {
                    try {
                        var storageUser = JSON.parse(storageUserString);
                        var expirationTime = new Date(storageUser.authenticationTime);
                        expirationTime.setDate(expirationTime.getDate() + 14);
                        var currentTime = new Date();
                        if (expirationTime < currentTime) {
                            $modal.open({
                                templateUrl: "/Application/commonApplication/views/app/app.auth.login.html",
                                controller: "LoginModalController",
                                size: "sm"
                            });
                        }

                        currentUser = storageUser;
                        $rootScope.currentUserEmail = storageUser.email;
                    } catch (e) {
                        console.log(e);
                        // Ignore any exception in deserialization
                    } 
                }
            }

            restoreCredentials();
            return {
                getCurrentUser() {
                    return currentUser;
                },
                isAnonymous() {
                    return typeof currentUser === "undefined" || !currentUser.token;
                },
                isAdministrator() {
                    return currentUser && currentUser.isAdministrator;
                },
                isVerified() {
                    return currentUser && currentUser.isVerified;
                },
                isInRole(roleToCheck: string) {
                    if (!currentUser || !(currentUser.roleNames))
                        return false;
                    return Enumerable.from(<string[]> currentUser.roleNames).any(roleName => roleName.toUpperCase() === roleToCheck.toUpperCase());
                },
                showLogin() {
                    var instance = $modal.open({
                        templateUrl: "/Application/commonApplication/views/app/app.auth.login.html",
                        controller: "LoginModalController",
                        size: "sm"
                    });

                    return instance.result;
                },
                logout() {
                    clearStoredCredentials();
                },
                attemptLogin(email, password, persistent) {
                    var deferred = $q.defer();

                    apiAccount.login(email, password)
                        .then(tokenData => {
                            onLoginSuccessful(email, tokenData, null, persistent);
                            apiAccount.getPermissions().success(permsData => {
                                onLoginSuccessful(email, tokenData, permsData, persistent);
                                deferred.resolve();
                            }).error(() => {
                                deferred.reject();
                            });
                        }, () => {
                            deferred.reject();
                        });

                    return deferred.promise;
                },
                permissionUpdate(token) {
                    var deferred = $q.defer();

                    apiAccount.getPermissions().success(permsData => {
                        updatePermissions(permsData, token);
                        deferred.resolve();
                    }).error(() => {
                        deferred.reject();
                        });

                    return deferred.promise;
                }
            };
        }
    ]);
}
