module JahanJooy.HaftDong {
    var app = angular.module('haftdong');
    app.requires.push("haftdongUsers", "haftdongLog", "haftdongAdminDashboard",
        "haftdongUserActivities", "haftdongConfigureDataItem", "haftdongVicinity");
}