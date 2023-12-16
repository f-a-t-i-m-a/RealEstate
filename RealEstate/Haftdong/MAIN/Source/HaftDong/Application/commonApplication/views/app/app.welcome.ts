module JahanJooy.HaftDong {
    appModule.controller("WelcomeController", [
        '$scope', '$timeout', '$state', 'authService',
        ($scope, $timeout, $state: angular.ui.IStateService, authService) => {
            $timeout(() => {
                if (authService.isAnonymous()) {
                    authService.showLogin().then(() => {
                        location.reload();
                        $state.go("dashboard");
                    }
                )} else {
                    $state.go("dashboard");
                }
            }, 500);
        }
    ]);

} 