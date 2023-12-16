module JahanJooy.HaftDong.Property {
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    import SalePriceSpecificationType = RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
    import EnumExtentions = Common.EnumExtentions;
    appModule.controller("PropertyDetailsController",
    [
        "$scope", "$timeout", "$state", "$http", "$stateParams",
        "allPropertiesPerUsage", "allPriceSpecificationTypePerProperty", "$rootScope", "messageBoxService",
        "toastr", "$modal", "Upload", "scopes", "$window", "mapApiKey",
        ($scope,
            $timeout,
            $state: angular.ui.IStateService,
            $http,
            $stateParams,
            allPropertiesPerUsage,
            allPriceSpecificationTypePerProperty,
            $rootScope,
            messageBoxService,
            toastr,
            $modal,
            Upload,
            scopes,
            $window,
            mapApiKey) => {

            $scope.GetTitle = () => {
                return "جزئیات ملک";
            };
            scopes.store("PropertyDetailsController", $scope);

            $scope.isEstateCollapsed = true;
            $scope.isUnitCollapsed = true;
            $scope.isHouseCollapsed = true;
            $scope.isIndustryCollapsed = true;
            $scope.isExtraHouseCollapsed = true;
            $scope.isShopCollapsed = true;
            $scope.isSaleCollapsed = true;
            $scope.isRentCollapsed = true;
            $scope.isDailyRentCollapsed = true;
            $scope.showStaticMap = true;
            $scope
                .mapStr =
                "http://maps.googleapis.com/maps/api/staticmap?center=35.6892,51.3890&zoom=8&size=640x500&key=" +
                mapApiKey;
            $scope.newImage = {
                title: "",
                description: ""
            };
            $scope.isCollapsed = true;
            $scope.notDeletedImages = [];

            $scope.currentDateMinusTwoWeeks = moment(new Date()).add(-2, "w").format("YYYY/MM/DD");
            $scope.isTwoWeeksAgo = (entity) => {
                if (entity != null)
                    return moment(entity).format("YYYY/MM/DD") >= $scope.currentDateMinusTwoWeeks;
                else
                    return false;
            };

            function setNotDeletedImages() {
                $scope.notDeletedImages = Enumerable.from($scope.property.Photos)
                    .where(i => i.DeletionTime == null)
                    .toArray();
                $scope.notDeletedImages.forEach(img => {
                    $http.get("/api/web/properties/getThumbnail/" + img.ID, { responseType: "blob" })
                        .success((data: Blob) => {
                            img.blob = URL.createObjectURL(data);
                            if (img.ID !== $scope.property.CoverImageID)
                                img.cover = false;
                            else
                                img.cover = true;
                        });
                });
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

            function managePanels(property) {
                $scope.showEstatePanel = Enumerable.from($scope.allowances)
                    .any(a => a.propertyType === property.PropertyType &&
                        Enumerable.from(a.panels).any(at => at === $scope.allPanels.estatePanel));

                $scope.showHousePanel = Enumerable.from($scope.allowances)
                    .any(a => a.propertyType === property.PropertyType &&
                        Enumerable.from(a.panels).any(at => at === $scope.allPanels.housePanel));

                $scope.showUnitPanel = Enumerable.from($scope.allowances)
                    .any(a => a.propertyType === property.PropertyType &&
                        Enumerable.from(a.panels).any(at => at === $scope.allPanels.unitPanel));

                $scope.showShedPanel = Enumerable.from($scope.allowances)
                    .any(a => a.propertyType === property.PropertyType &&
                        Enumerable.from(a.panels).any(at => at === $scope.allPanels.shedPanel));

                $scope.showExtraHousePanel = Enumerable.from($scope.allowances)
                    .any(a => a.propertyType === property.PropertyType &&
                        Enumerable.from(a.panels).any(at => at === $scope.allPanels.extraHousePanel));

                $scope.showShopPanel = Enumerable.from($scope.allowances)
                    .any(a => a.propertyType === property.PropertyType &&
                        Enumerable.from(a.panels).any(at => at === $scope.allPanels.shopPanel));

                var allSalePriceSpecificationTypes = EnumExtentions.getValues(SalePriceSpecificationType)
                    .select(i => {
                        return { id: i, text: Common.localization.translateEnum("SalePriceSpecificationType", i) }
                    })
                    .toArray();
                $scope.priceSpecificationTypeOptions = Enumerable.from(allSalePriceSpecificationTypes)
                    .where(pr => Enumerable.from(allPriceSpecificationTypePerProperty)
                        .any(a => a.propertyType === property.PropertyType &&
                            Enumerable.from(a.priceTypes).any(pt => pt === pr.id)))
                    .toArray();

                if (property.IntentionOfOwner != null && property.IntentionOfOwner === 2) {
                    $scope.showRentPanel = false;
                    $scope.showSalePanel = true;
                    $scope.showDailyRentPanel = false;
                    $scope.property.Rent = "";
                } else if (property.IntentionOfOwner != null && property.IntentionOfOwner === 3) {
                    $scope.showSalePanel = false;
                    $scope.showRentPanel = true;
                    $scope.showDailyRentPanel = false;
                    $scope.property.Rent = 0;
                } else if (property.IntentionOfOwner != null && property.IntentionOfOwner === 1) {
                    $scope.showRentPanel = true;
                    $scope.showSalePanel = false;
                    $scope.showDailyRentPanel = false;
                } else if (property.IntentionOfOwner != null && property.IntentionOfOwner === 4) {
                    $scope.showRentPanel = false;
                    $scope.showDailyRentPanel = true;
                    $scope.showSalePanel = false;
                } else {
                    $scope.showSalePanel = false;
                    $scope.showRentPanel = false;
                    $scope.showDailyRentPanel = false;
                    $scope.property.Rent = "";
                }
            }

            function initialize() {
                $scope.PropertyID = $stateParams.id;
                $http.get("/api/web/properties/get/" + $stateParams.id, null)
                    .success((data: any) => {
                        $scope.property = data;
                        managePanels($scope.property);
                        setNotDeletedImages();

                        if ($scope.property.Supplies != null) {
                            $scope.property.Supplies.forEach(s => {
                                $http.get("/api/web/supplies/get/" + s.ID, null)
                                    .success((data: any) => {
                                        if (data != null) {
                                            var supplyDetail = data;

                                            if (supplyDetail.IntentionOfOwner === 5) {
                                                if (supplyDetail.SwapAdditionalComments != null &&
                                                    supplyDetail.SwapAdditionalComments != undefined &&
                                                    supplyDetail.SwapAdditionalComments !== "") {
                                                    s.SwapText = "معاوضه با: " + supplyDetail.SwapAdditionalComments;
                                                } else if (supplyDetail.Request != null && supplyDetail.Request != undefined) {
                                                    s.SwapText = "معاوضه با: ";
                                                    if (supplyDetail.Request.PropertyTypes != null &&
                                                        supplyDetail.Request.PropertyTypes != undefined &&
                                                        supplyDetail.Request.PropertyTypes.length !== 0) {
                                                        supplyDetail.Request.PropertyTypes.forEach(p => {
                                                            s.SwapText += Common.localization.translateEnum("PropertyType", p) + "، ";
                                                        });
                                                    }

                                                    s.SwapText += " در محدوده ی ";
                                                    if (supplyDetail.Request.SelectedVicinities != null &&
                                                        supplyDetail.Request.SelectedVicinities != undefined &&
                                                        supplyDetail.Request.SelectedVicinities.length !== 0) {
                                                        var count = 0;
                                                        supplyDetail.Request.SelectedVicinities.forEach(v => {
                                                            count++;
                                                            s.SwapText += v.Name;
                                                            if (count !== supplyDetail.Request.SelectedVicinities.length
                                                            ) {
                                                                s.SwapText += "، ";
                                                            }
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                    });
                            });
                        }

                        $http.get("/api/web/properties/getContactInfos/" + $scope.property.ID, null)
                            .success((data: any) => {
                                if (data != null) {
                                    $scope.property.Owner = data.Owner;
                                }
                            });

                        if ($scope.property != null && $scope.property.GeographicLocation != null) {
                            $scope.mapStr = "http://maps.googleapis.com/maps/api/staticmap?center=" +
                                $scope.property.GeographicLocation.Y +
                                "," +
                                $scope.property.GeographicLocation.X +
                                "&zoom=13&size=640x500&key=" +
                                mapApiKey +
                                "&markers=color:red%7C" +
                                $scope.property.GeographicLocation.Y +
                                "," +
                                $scope.property.GeographicLocation.X;
                        } else if ($scope.property != null &&
                            $scope.property.Vicinity != null &&
                            $scope.property.Vicinity.CenterPoint != null) {
                            $scope.mapStr = "http://maps.googleapis.com/maps/api/staticmap?center=" +
                                $scope.property.Vicinity.CenterPoint.Y +
                                "," +
                                $scope.property.Vicinity.CenterPoint.X +
                                "&zoom=13&size=640x500&key=" +
                                mapApiKey;
                            //it's better to show region instead of vicinity's center point
                        }
                    });
            }

            $scope.changeMapToDynamic = () => {
                $scope.showStaticMap = false;
            }

            $scope.onSourceClick = () => {
                var data = $scope.property.ExternalDetails;
                data = "<div>" + data + "</div>";
                $window.open("data:text/xml;charset=utf-8," + encodeURIComponent(data),
                    "_blank",
                    "width=800,height=600");
            };

            $scope.onDeleteClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این ملک را حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/properties/delete/" + $scope.property.ID, null)
                        .success(() => {
                            toastr.success("ملک با موفقیت حذف شد.");
                            $scope.$emit("EntityUpdated");
                        });
                });
            };

            $scope.onPublishClick = () => {
                var modalInstance = $modal.open({
                    templateUrl: "Application/userApplication/views/Properties/set.expirationtime.modal.html",
                    controller: "SetExpirationTimeModalController",
                    scope: $scope,
                    size: "lg"
                });
                modalInstance.result.then(() => {
                    $scope.$emit("EntityUpdated");
                });
            };

            $scope.onArchivedClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این ملک را آرشیو کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/properties/archived/" + $scope.property.ID, null)
                        .success(() => {
                            toastr.success("ملک آرشیو شد.");
                            $scope.$emit("EntityUpdated");
                        });
                });
            };

            $scope.onUnArchivedClick = () => {
                var confirmResult = messageBoxService
                    .confirm("آیا مطمئن هستید که می خواهید این ملک را از آرشیو حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/properties/unarchived/" + $scope.property.ID, null)
                        .success(() => {
                            toastr.success("ملک از آرشیو حذف شد.");
                            $scope.$emit("EntityUpdated");
                        });
                });
            };

            $scope.onDeleteSupplyClick = (id) => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این آگهی را حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/supplies/delete/" + id, null)
                        .success(() => {
                            toastr.success("آگهی با موفقیت حذف شد.");
                            $scope.$emit("EntityUpdated");
                        });
                });
            };

            $scope.onArchivedSupplyClick = (id) => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این آگهی را آرشیو کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/supplies/archived/" + id, null)
                        .success(() => {
                            toastr.success("آگهی آرشیو شد.");
                            $scope.$emit("EntityUpdated");
                        });
                });
            };

            $scope.onUnArchivedSupplyClick = (id) => {
                var confirmResult = messageBoxService
                    .confirm("آیا مطمئن هستید که می خواهید این آگهی را از آرشیو حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/supplies/unarchived/" + id, null)
                        .success(() => {
                            toastr.success("آگهی از آرشیو حذف شد.");
                            $scope.$emit("EntityUpdated");
                        });
                });
            };

            $scope.onPublishSupplyClick = (id) => {
                $scope.entityId = id;
                var modalInstance = $modal.open({
                    templateUrl: "Application/userApplication/views/Properties/set.expirationtime.modal.html",
                    controller: "SetExpirationTimeModalController",
                    scope: $scope,
                    size: "lg"
                });
                modalInstance.result.then(() => {
                    $scope.entityId = null;
                    $scope.$emit("EntityUpdated");
                });
            };

            $scope.onUnpublishSupplyClick = (id) => {
                var confirmResult = messageBoxService
                    .confirm("آیا مطمئن هستید که می خواهید این آگهی را از حالت عمومی خارج کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/supplies/unpublish/" + id, null)
                        .success(() => {
                            toastr.success("آگهی از حالت عمومی خارج شد.");
                            $scope.$emit("EntityUpdated");
                        });
                });
            };

            $scope.onNewSupplyClick = () => {
                $scope.supply = {};
                $scope.supply.Property = (JSON.parse(JSON.stringify($scope.property)));
                if ($scope.supply.Property != null)
                    $scope.supply.Property.Supplies = [];
                var modalInstance = $modal.open({
                    templateUrl: "Application/userApplication/views/Supplies/supplies.new.modal.html",
                    controller: "NewSupplyModalController",
                    scope: $scope,
                    size: "lg"
                });
                modalInstance.result.then(() => {
                    $scope.$emit("EntityUpdated");
                });
            };

            $scope.onEditSupplyClick = (supply) => {
                $http.get("/api/web/supplies/get/" + supply.ID, null)
                    .success((data: any) => {
                        $scope.supply = data;
                        $http.get("/api/web/properties/getContactInfos/" + $scope.property.ID, null)
                            .success((data: any) => {
                                if (data != null && data.ContactInfos != null) {
                                    var contactInfo = Enumerable.from(data.ContactInfos)
                                        .singleOrDefault(ci => ci.SupplyID === $scope.supply.ID);
                                    if (contactInfo != null) {
                                        $scope.supply.Contact = contactInfo;
                                        $scope.prepareContacts();
                                    }
                                }
                                var modalInstance = $modal.open({
                                    templateUrl: "Application/userApplication/views/Supplies/supplies.edit.modal.html",
                                    controller: "EditSupplyModalController",
                                    scope: $scope,
                                    size: "lg"
                                });
                                modalInstance.result.then(() => {
                                    $scope.$emit("EntityUpdated");
                                });
                            });
                    });
            };

            $scope.onSupplyDetailClick = (supply) => {
                $http.get("/api/web/supplies/get/" + supply.ID, null)
                    .success((data: any) => {
                        $scope.supply = data;

                        $http.get("/api/web/properties/getContactInfos/" + $scope.property.ID, null)
                            .success((data: any) => {
                                if (data != null && data.ContactInfos != null) {
                                    var contactInfo = Enumerable.from(data.ContactInfos)
                                        .singleOrDefault(ci => ci.SupplyID === $scope.supply.ID);
                                    if (contactInfo != null) {
                                        $scope.supply.Contact = contactInfo;
                                        $scope.prepareContacts();
                                    }
                                }

                                var modalInstance = $modal.open({
                                    templateUrl: "Application/userApplication/views/Supplies/supplies.detail.modal.html",
                                    controller: "SupplyDetailModalController",
                                    scope: $scope,
                                    size: "lg"
                                });
                                modalInstance.result.then(() => {
                                });
                            });
                    });
            };

            $scope.onContactInfoClick = (supply) => {
                $http.get("/api/web/supplies/get/" + supply.ID, null)
                    .success((data: any) => {
                        $scope.supply = data;

                        $http.get("/api/web/properties/getContactInfos/" + $scope.property.ID, null)
                            .success((data: any) => {
                                if (data != null && data.ContactInfos != null) {
                                    var contactInfo = Enumerable.from(data.ContactInfos)
                                        .singleOrDefault(ci => ci.SupplyID === $scope.supply.ID);
                                    if (contactInfo != null) {
                                        $scope.supply.Contact = contactInfo;
                                    }
                                }

                                var modalInstance = $modal.open({
                                    templateUrl: "Application/userApplication/views/Properties/contact.info.modal.html",
                                    controller: "ContactInfoModalController",
                                    scope: $scope,
                                    size: "lg"
                                });
                                modalInstance.result.then(() => {
                                });
                            });
                    });
            };

            $scope.toggleNewImage = () => {
                $scope.isCollapsed = !$scope.isCollapsed;
            };

            $scope.upload = files => {
                if (files) {
                    var file = files;
                    Upload.upload({
                            url: "/api/web/properties/addimage",
                            fields: {
                                description: $scope.newImage.description,
                                title: $scope.newImage.title,
                                propertyId: $stateParams.id
                            },
                            file: file
                        })
                        .success(() => {
                            $scope.newImage.title = "";
                            $scope.newImage.description = "";
                            $scope.isCollapsed = true;
                            $scope.$emit("EntityUpdated");
                            $scope.files.name = "";
                            files.name = "";
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
                    templateUrl: "Application/userApplication/views/Properties/property.image.modal.html",
                    controller: "PropertyImageModalController",
                    scope: $scope,
                    size: "lg"
                });
            };

            $scope.computePrice = (entity) => {
                if (entity.PriceSpecificationType === null) {
                    return 0;
                } else if (entity.PriceSpecificationType === 1) {
                    return entity.TotalPrice;
                } else if (entity.PriceSpecificationType === 2 && $scope.property.EstateArea != null) {
                    return entity.PricePerEstateArea * $scope.property.EstateArea;
                } else if (entity.PriceSpecificationType === 3 && $scope.property.UnitArea != null) {
                    return entity.PricePerUnitArea * $scope.property.UnitArea;
                }
                return 0;
            };

            $scope.prepareContacts = () => {
                if ($scope.supply.Contact.AgencyContact == null) {
                    $scope.supply.Contact.AgencyContact = {
                        Phones: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ],
                        Emails: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ],
                        Addresses: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
                            }
                        ]
                    };
                } else {
                    if ($scope.supply.Contact.AgencyContact.Phones == null ||
                        $scope.supply.Contact.AgencyContact.Phones.length === 0) {
                        $scope.supply.Contact.AgencyContact.Phones = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ];
                    }

                    if ($scope.supply.Contact.AgencyContact.Emails == null ||
                        $scope.supply.Contact.AgencyContact.Emails.length === 0) {
                        $scope.supply.Contact.AgencyContact.Emails = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ];
                    }

                    if ($scope.supply.Contact.AgencyContact.Addresses == null ||
                        $scope.supply.Contact.AgencyContact.Addresses.length === 0) {
                        $scope.supply.Contact.AgencyContact.Addresses = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
                            }
                        ];
                    }
                }


                if ($scope.supply.Contact.OwnerContact == null) {
                    $scope.supply.Contact.OwnerContact = {
                        Phones: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ],
                        Emails: [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ],
                        Addresses: []
                    };
                } else {
                    if ($scope.supply.Contact.OwnerContact.Phones == null ||
                        $scope.supply.Contact.OwnerContact.Phones.length === 0) {
                        $scope.supply.Contact.OwnerContact.Phones = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                            }
                        ];
                    }

                    if ($scope.supply.Contact.OwnerContact.Emails == null ||
                        $scope.supply.Contact.OwnerContact.Emails.length === 0) {
                        $scope.supply.Contact.OwnerContact.Emails = [
                            {
                                Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
                            }
                        ];
                    }
                }
            }

            $scope.onBackToListClick = () => {
                $state.go("files");
            }

            initialize();

            var listener = $rootScope.$on("EntityUpdated",
            () => {
                initialize();
            });

            $scope.$on("$destroy", listener);
        }
    ]);

}