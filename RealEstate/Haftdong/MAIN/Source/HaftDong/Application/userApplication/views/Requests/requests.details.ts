module JahanJooy.HaftDong.Request {
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    appModule.controller("RequestDetailsController",
    [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "$rootScope", "messageBoxService", "toastr", "scopes",
        "$modal",
        ($scope,
            $timeout,
            $state: angular.ui.IStateService,
            $http,
            $stateParams,
            $rootScope,
            messageBoxService,
            toastr,
            scopes,
            $modal) => {

            $scope.GetTitle = () => {
                return 'جزئیات درخواست';
            }
            scopes.store('RequestDetailsController', $scope);

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
                    $scope.showEstatePanel = Enumerable.from($scope.allowances)
                        .any(a => a.propertyType === request.PropertyTypes[0] &&
                            Enumerable.from(a.panels).any(at => at === $scope.allPanels.estatePanel));

                    $scope.showHousePanel = Enumerable.from($scope.allowances)
                        .any(a => a.propertyType === request.PropertyTypes[0] &&
                            Enumerable.from(a.panels).any(at => at === $scope.allPanels.housePanel));

                    $scope.showUnitPanel = Enumerable.from($scope.allowances)
                        .any(a => a.propertyType === request.PropertyTypes[0] &&
                            Enumerable.from(a.panels).any(at => at === $scope.allPanels.unitPanel));

                    $scope.showExtraHousePanel = Enumerable.from($scope.allowances)
                        .any(a => a.propertyType === request.PropertyTypes[0] &&
                            Enumerable.from(a.panels).any(at => at === $scope.allPanels.extraHousePanel));

                    $scope.showShopPanel = Enumerable.from($scope.allowances)
                        .any(a => a.propertyType === request.PropertyTypes[0] &&
                            Enumerable.from(a.panels).any(at => at === $scope.allPanels.shopPanel));

                    var allUsageTypes = Common.EnumExtentions
                        .getValues(RealEstateAgency.Domain.Enums.Property.UsageType)
                        .select(i => { return { id: i, text: Common.localization.translateEnum("UsageType", i) } })
                        .toArray();
                    $scope.UsageTypeOptions = Enumerable.from(allUsageTypes)
                        .where(u => Enumerable.from($scope.allUsages)
                            .any(a => a.propertyType === request.PropertyTypes[0] &&
                                Enumerable.from(a.usages).any(uu => uu === u.id)))
                        .toArray();
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
                $http.get("/api/web/requests/get/" + $stateParams.id, null)
                    .success((data: any) => {
                        $scope.request = data;
                        managePanels($scope.request);
                        $http.get("/api/web/requests/getContactInfos/" + $scope.request.ID, null)
                            .success((data: any) => {
                                if (data != null) {
                                    $scope.request.Owner = data.Owner;
                                    $scope.request.Contact = {};
                                    $scope.request.Contact.AgencyContact = data.AgencyContact;
                                    $scope.request.Contact.OwnerContact = data.OwnerContact;
                                    $scope.request.Contact.OwnerCanBeContacted = data.OwnerCanBeContacted;
                                }
                            });
                    });
            }

            $scope.onArchivedClick = () => {
                var confirmResult = messageBoxService
                    .confirm("آیا مطمئن هستید که می خواهید این درخواست را آرشیو کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/requests/archived/" + $scope.request.ID, null)
                        .success(() => {
                            toastr.success("درخواست آرشیو شد.");
                            $scope.$emit('EntityUpdated');
                        });
                });
            }

            $scope.onUnArchivedClick = () => {
                var confirmResult = messageBoxService
                    .confirm("آیا مطمئن هستید که می خواهید این درخواست را از آرشیو حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/requests/unarchived/" + $scope.request.ID, null)
                        .success(() => {
                            toastr.success("درخواست از آرشیو حذف شد.");
                            $scope.$emit('EntityUpdated');
                        });
                });
            }

            $scope.onDeleteClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این درخواست را حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/requests/delete/" + $scope.request.ID, null)
                        .success(() => {
                            toastr.success("درخواست با موفقیت حذف شد.");
                            $scope.$emit('EntityUpdated');
                        });
                });
            }

            $scope.onPublishClick = () => {
                var modalInstance = $modal.open({
                    templateUrl: "Application/userApplication/views/Requests/set.expirationtime.modal.html",
                    controller: "SetRequestExpirationTimeModalController",
                    scope: $scope,
                    size: "lg"
                });
                modalInstance.result.then(() => {
                    $scope.$emit("EntityUpdated");
                });
            };

            $scope.onUnpublishClick = () => {
                var confirmResult = messageBoxService
                    .confirm("آیا مطمئن هستید که می خواهید این درخواست را از حالت عمومی خارج کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/requests/unpublish/" + $scope.request.ID, null)
                        .success(() => {
                            toastr.success("درخواست از حالت عمومی خارج شد.");
                            $scope.$emit("EntityUpdated");
                        });
                });
            };

            $scope.onBackToListClick = () => {
                $state.go("requests");
            }

            initialize();

            var listener = $rootScope.$on('EntityUpdated',
            () => {
                initialize();
            });

            $scope.$on('$destroy', listener);

        }
    ]);

}