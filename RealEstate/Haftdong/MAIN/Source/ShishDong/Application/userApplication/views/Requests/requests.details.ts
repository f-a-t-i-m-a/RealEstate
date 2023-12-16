module JahanJooy.ShishDong.Request {
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    appModule.controller("RequestDetailsController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "$rootScope", "messageBoxService", "toastr", "scopes", "authService", "$modal",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, $rootScope, messageBoxService, toastr, scopes, authService, $modal) => {

            $scope.GetTitle = () => {
                return 'جزئیات درخواست';
            }
            scopes.store('RequestDetailsController', $scope);

            $scope.request = {};

            $scope.currentDateMinusTwoWeeks = moment(new Date()).add(-2, "w").format("YYYY/MM/DD");
            $scope.isTwoWeeksAgo = (entity) => {
                if (entity != null)
                    return moment(entity).format("YYYY/MM/DD") >= $scope.currentDateMinusTwoWeeks;
                else
                    return false;
            }

            $scope.allPanels = {
                estatePanel: "showEstatePanel",
                housePanel: "showHousePanel",
                unitPanel: "showUnitPanel",
                industryPanel: "showIndustryPanel",
                extraHousePanel: "showHousePanel",
                shopPanel: "showShopPanel"
            };

            $scope.allowances = [
                {
                    propertyType: PropertyType.Land,
                    panels: [
                        $scope.allPanels.estatePanel
                    ]
                },
                {
                    propertyType: PropertyType.AgriculturalLand,
                    panels: [
                        $scope.allPanels.estatePanel
                    ]
                },
                {
                    propertyType: PropertyType.Garden,
                    panels: [
                        $scope.allPanels.estatePanel
                    ]
                },
                {
                    propertyType: PropertyType.House,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.OldHouse,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Shed,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.industryPanel
                    ]
                },
                {
                    propertyType: PropertyType.Tenement,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.housePanel
                    ]
                },
                {
                    propertyType: PropertyType.Villa,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Apartment,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Complex,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Tower,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.GardenTower,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Office,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.OfficialResidency,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Commercial,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.CommercialResidency,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Suite,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Penthouse,
                    panels: [
                        $scope.allPanels.housePanel,
                        $scope.allPanels.unitPanel,
                        $scope.allPanels.extraHousePanel
                    ]
                },
                {
                    propertyType: PropertyType.Shop,
                    panels: [
                        $scope.allPanels.shopPanel
                    ]
                },
                {
                    propertyType: PropertyType.Factory,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.industryPanel
                    ]
                },
                {
                    propertyType: PropertyType.WorkShop,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.industryPanel
                    ]
                },
                {
                    propertyType: PropertyType.RepairShop,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.industryPanel
                    ]
                },
                {
                    propertyType: PropertyType.StoreHouse,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.industryPanel
                    ]
                },
                {
                    propertyType: PropertyType.Parking,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.industryPanel
                    ]
                },
                {
                    propertyType: PropertyType.Gym,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.industryPanel
                    ]
                },
                {
                    propertyType: PropertyType.CityService,
                    panels: [
                        $scope.allPanels.estatePanel,
                        $scope.allPanels.industryPanel
                    ]
                }
            ];

            function managePanels(request) {
                if (request.PropertyTypes.length === 1) {
                    $scope.showEstatePanel = Enumerable.from<any>($scope.allowances).any(a => a.propertyType === request.PropertyTypes[0]
                        && Enumerable.from<any>(a.panels).any(at => at === $scope.allPanels.estatePanel));

                    $scope.showHousePanel = Enumerable.from<any>($scope.allowances).any(a => a.propertyType === request.PropertyTypes[0]
                        && Enumerable.from<any>(a.panels).any(at => at === $scope.allPanels.housePanel));

                    $scope.showUnitPanel = Enumerable.from<any>($scope.allowances).any(a => a.propertyType === request.PropertyTypes[0]
                        && Enumerable.from<any>(a.panels).any(at => at === $scope.allPanels.unitPanel));

                    $scope.showExtraHousePanel = Enumerable.from<any>($scope.allowances).any(a => a.propertyType === request.PropertyTypes[0]
                        && Enumerable.from<any>(a.panels).any(at => at === $scope.allPanels.extraHousePanel));

                    $scope.showShopPanel = Enumerable.from<any>($scope.allowances).any(a => a.propertyType === request.PropertyTypes[0]
                        && Enumerable.from<any>(a.panels).any(at => at === $scope.allPanels.shopPanel));

                    var allUsageTypes = Common.EnumExtentions.getValues(RealEstateAgency.Domain.Enums.Property.UsageType).select(i => { return { id: i, text: Common.localization.translateEnum("UsageType", i) } }).toArray();
                    $scope.UsageTypeOptions = Enumerable.from<any>(allUsageTypes).where(u => Enumerable.from<any>($scope.allUsages).any(a => a.propertyType === request.PropertyTypes[0]
                        && Enumerable.from<any>(a.usages).any(uu => uu === u.id))).toArray();
                } else {
                    $scope.UsageTypeOptions = [];
                    $scope.showEstatePanel = false;
                    $scope.showUnitPanel = false;
                    $scope.showHousePanel = false;
                    $scope.showExtraHousePanel = false;
                    $scope.showShopPanel = false;
                }
            }

            function initialize() {
                $http.get("/api/haftdong/requests/get/" + $stateParams.id, null).success((data: any) => {
                    $scope.request = data;
                    managePanels($scope.request);
                });
            }

            $scope.onRequestContactInfoClick = () => {
                if (!authService.isAnonymous() && authService.isVerified()) {
                    $http.get("/api/haftdong/requests/getContactInfos/" + $scope.request.ID, null)
                        .success((data: any) => {
                            if (data != null) {
                                $scope.request.AgencyContact = data.AgencyContact;
                                $scope.request.OwnerContact = data.OwnerContact;
                                $scope.request.OwnerCanBeContacted = data.OwnerCanBeContacted;
                            }

                            var modalInstance = $modal.open({
                                templateUrl: "Application/userApplication/views/Requests/request.contactinfo.modal.html",
                                controller: "RequestContactInfoModalController",
                                scope: $scope,
                                size: "lg"
                            });
                            modalInstance.result.then(() => {
                            });
                        });
                }
            }

            $scope.onDeleteClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این درخواست را حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/haftdong/requests/delete/" + $scope.request.ID, null).success(() => {
                        toastr.success("درخواست با موفقیت حذف شد.");
                        $scope.$emit('EntityUpdated');
                        $state.go("^");
                    });
                });
            }

            $scope.onEditClick = () => {
                if (!authService.isAnonymous()) {
                    $http.get("/api/haftdong/requests/getContactInfos/" + $scope.request.ID, null)
                        .success((data: any) => {
                            if (data != null) {
                                $scope.request.Contact = {};
                                $scope.request.Contact.AgencyContact = data.AgencyContact;
                                $scope.request.Contact.OwnerContact = data.OwnerContact;
                                $scope.request.Contact.OwnerCanBeContacted = data.OwnerCanBeContacted;
                                $state.go("welcome.requests.details.edit");
                            }
                        });
                }
            }

            $scope.onBackToListClick = () => {
                $state.go("welcome.requests");
            }

            initialize();

            var listener = $rootScope.$on('EntityUpdated', () => {
                initialize();
            });

            $scope.$on('$destroy', listener);

        }
    ]);

}