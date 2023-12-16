module JahanJooy.HaftDong {
    var app = angular.module('haftdong');
    app.requires.push("haftdongCustomers", "haftdongOperations", "haftdongRequests",
        "haftdongDashboard", "haftdongContracts", "haftdongFiles");
}