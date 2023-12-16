module JahanJooy.HaftDong {
    export var appModule = angular.module("haftdong", [
        "ui.router", "ui.bootstrap",
        "toastr", "jjCommon", "ngFileUpload",
        "haftdongApi", "haftdongComponents", "haftdongGlobal",
        "nvd3"
    ]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("accessDenied", {
                    url: "/app/accessdenied",
                    templateUrl: "Application/commonApplication/views/app/app.accessdenied.html",
                    data: {
                        allowAnonymous: true,
                        title: "خطا در دسترسی"
                    }
                })
                .state("welcome", {
                    url: "/app/welcome",
                    templateUrl: "Application/commonApplication/views/app/app.welcome.html",
                    controller: "WelcomeController",
                    data: {
                        allowAnonymous: true,
                        showData: true,
                        title: "هفت دنگ"
                    }
                })
                .state("signup", {
                    url: "/app/signup",
                    templateUrl: "Application/commonApplication/views/app/app.signup.html",
                    controller: "SignupController",
                    data: {
                        allowAnonymous: true,
                        showData: true,
                        title: "ثبت نام"
                    }
                })
                .state("forgotPassword", {
                    url: "/app/forgotPassword",
                    templateUrl: "Application/commonApplication/views/app/app.forgotPassword.html",
                    controller: "ForgotPasswordController",
                    data: {
                        allowAnonymous: true,
                        showData: true,
                        title: "بازیابی کلمه عبور"
                    }
                })
                .state("resetPassword", {
                    url: "/app/resetPassword",
                    templateUrl: "Application/commonApplication/views/app/app.reset.password.html",
                    controller: "ResetPasswordController",
                    data: {
                        allowAnonymous: true,
                        showData: true,
                        title: "تنظیم مجدد کلمه عبور"
                    }
                })
            ;
        }
    ]);
}