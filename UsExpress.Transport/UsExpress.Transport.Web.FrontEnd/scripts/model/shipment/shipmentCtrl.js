(function () {
    'use strict';
    angular
        .module('OrderApp')
        .controller('ShipmentController', ShipmentController);
    ShipmentController.$inject = ['$http', '$scope', '$timeout', 'moment', 'pagerService', '$uibModal', '$log', '$document'];

    function ShipmentController($http, $scope, $timeout, moment, pagerService, $uibModal, $log, $document) {
        var vm = this;
        vm.destinations = [{ Id: 1, Name: 'HAN' }, { Id: 2, Name: 'SGN' }];
        vm.poundToKg = 0.45359237;
        vm.lstWarehouse = lstWarehouse;
        vm.animationsEnabled = true;
        vm.formatCurrent = "MMM DD YYYY";
        vm.showAdvanceSearch = true;
        vm.lstPackageAvaiable = [];
        vm.lstShipment = [];
        vm.pager = {};
        vm.showDetailOrder = showDetailOrder;
        vm.shipmentSelectedIndex = -1;
        vm.shipmentSelected = {};
        vm.filter = {
            Keyword: '',
            TypeSearch: 0,
            FromDate: '',
            PageIndex: 0,
            PageSize: 1000,
            ToDate: '',
            TotalPages: 1,
            total: 0,
            LstStatus: '1,2',
            LstDestination: '1,2',
            WarehouseId: -1
        };
        vm.optionsSearch = [{ Id: 0, Name: 'Id' }, { Id: 1, Name: 'sender' }, { Id: 2, Name: "warehouse's name" }];
        vm.isShowBtnRemovePackage = false;
        vm.isShowBtnAddPackage = false;
        vm.isShowShipmentDetail = false;
        vm.isShowBtnAddShipment = true;
        vm.isShowBtnForShipment = false;

        vm.getPackageAvaiable = getPackageAvaiable;
        vm.setDateText = setDateText;
        vm.setShowAdvanceSearch = setShowAdvanceSearch;
        vm.setPage = setPage;
        vm.searchShipment = searchShipment;
        vm.setShipmentSelected = setShipmentSelected;
        vm.removePackageInShipment = removePackageInShipment;
        vm.showBtnRemovePackage = showBtnRemovePackage;
        vm.saveShipment = saveShipment;
        vm.addPackageInShipment = addPackageInShipment;
        vm.showBtnAddPackage = showBtnAddPackage;
        vm.addNewShipment = addNewShipment;
        vm.cancelUpdateShipment = cancelUpdateShipment;
        vm.computedWeightShipmentSelected = computedWeightShipmentSelected;
        vm.computedWeightShipmentSelectedPound = computedWeightShipmentSelectedPound;
        vm.computedWeightPackageAvaiable = computedWeightPackageAvaiable;
        vm.computedWeightPackageAvaiablePound = computedWeightPackageAvaiablePound;
        vm.removeShipment = removeShipment;
        vm.exportShipmentUSA = exportShipmentUSA;
        vm.exportShipmentVN = exportShipmentVN;
        vm.setShowBtnForShipment = setShowBtnForShipment;
        vm.exportShipment = exportShipment;
        vm.exportMultiShipment = exportMultiShipment;

        vm.FromDate = new Date();
        vm.ToDate = new Date();
        vm.searchAdvance = false;


        function exportMultiShipment(type) {
            var lstId = [];
            for (var i = 0; i < vm.lstShipment.length; i++) {
                if (vm.lstShipment[i].checked) {
                    lstId.push(vm.lstShipment[i].Id);
                }
            }
            if (lstId.length > 0) {
                window.open('/Shipment/ExportShipmentInvoice?typeId=' + type + '&ids=' + lstId.join());
            }
        }
        function setShowBtnForShipment() {
            for (var i = 0; i < vm.lstShipment.length; i++) {
                if (vm.lstShipment[i].checked) {
                    vm.isShowBtnForShipment = true;
                    return;
                }
            }
            vm.isShowBtnForShipment = false;
        }
        function exportShipmentVN() {
            exportShipment(2);
        }
        function exportShipmentUSA() {
            exportShipment(1);
        }
        function exportShipment(type) {
            window.open('/Shipment/ExportShipmentInvoice?typeId=' + type + '&ids=' + vm.shipmentSelected.Id);
        }
        function removeShipment() {
            var lstId = [];
            for (var i = 0; i < vm.lstShipment.length; i++) {
                if (vm.lstShipment[i].checked) {
                    lstId.push(vm.lstShipment[i].Id);
                }
            }
            if (lstId.length > 0) {
                if (confirm("Bạn chắc chứ?")) {
                    $.ajax({
                        url: "/Shipment/RemoveLstShipment",
                        type: 'POST',
                        params: { contentType: "application/json;" },
                        dataType: 'json',
                        data: { lstId: lstId },
                        success: function (result) {
                            if (result.Message) {
                                notification(notifiType.error, result.Message, "Error");
                            }
                            else {
                                notification(notifiType.success, lb_UpdateSucceed, "Success");
                                init();
                            }

                        },
                        error: function (err) {
                            notification(notifiType.error, err.statusText, "Error");
                        }
                    });
                }
            }
            else {
                notification(notifiType.warning, lb_ShipmentRequired, "Warning");
            }
        }
        function computedWeightPackageAvaiable() {
            var total = 0;
            if (vm.shipmentSelectedIndex != -1 && vm.lstPackageAvaiable.length > 0) {
                for (var i = 0; i < vm.lstPackageAvaiable.length; i++) {
                    total += vm.lstPackageAvaiable[i].Weight;
                }
            }
            return total;
        }
        function computedWeightPackageAvaiablePound() {
            return parseFloat(vm.computedWeightPackageAvaiable() * vm.poundToKg).toFixed(2);
        }
        function computedWeightShipmentSelected() {
            var total = 0;
            if (vm.shipmentSelectedIndex != -1 && vm.shipmentSelected.LstPackage.length > 0) {
                for (var i = 0; i < vm.shipmentSelected.LstPackage.length; i++) {
                    total += vm.shipmentSelected.LstPackage[i].Weight;
                }
            }
            return total;
        }
        function computedWeightShipmentSelectedPound() {
            return parseFloat(vm.computedWeightShipmentSelected() * vm.poundToKg).toFixed(2);
        }
        vm.open = function (size, parentSelector) {
            var parentElem = parentSelector ?
                angular.element($document[0].querySelector('.modal-demo ' + parentSelector)) : undefined;
            var modalInstance = $uibModal.open({
                animation: vm.animationsEnabled,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'myModalContent.html',
                controller: 'ModalInstanceCtrl',
                controllerAs: 'manCtrl',
                size: size,
                appendTo: parentElem,
                resolve: {
                    items: function () {
                        return vm.destinations;
                    }
                }
            });

            modalInstance.result.then(function (selectedItem) {
                vm.selected = selectedItem;
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        function cancelUpdateShipment() {
            vm.isShowShipmentDetail = false;
            vm.isShowBtnAddShipment = true;
            vm.shipmentSelectedIndex = -1;
        }
        function addNewShipment(size, parentSelector) {
            $("#btnAddShipment").prop("disabled", true);
            var parentElem = parentSelector ?
                angular.element($document[0].querySelector('.modal-demo ' + parentSelector)) : undefined;
            var modalInstance = $uibModal.open({
                animation: vm.animationsEnabled,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'myModalContent.html',
                controller: 'ModalInstanceCtrl',
                controllerAs: 'manCtrl',
                size: size,
                appendTo: parentElem,
                resolve: {
                    items: function () {
                        return { destinations: vm.destinations, warehouse: vm.lstWarehouse };
                    }
                }
            });

            modalInstance.result.then(function (selectedItem) {
                $.ajax({
                    url: "/Shipment/AddNewShipment",
                    type: 'POST',
                    params: { contentType: "application/json;" },
                    dataType: 'json',
                    data: { destinationId: selectedItem.destinationId, warehouseId: selectedItem.warehouseId },
                    success: function (result) {
                        $("#btnAddShipment").prop("disabled", false);
                        if (result.Message) {
                            notification(notifiType.error, result.Message, "Error");
                        }
                        else {

                            vm.filter.WarehouseId = selectedItem.warehouseId;
                            vm.getPackageAvaiable();
                            console.log(vm.filter);
                            result.Result.checked = false;
                            vm.shipmentSelected = result.Result;
                            if (vm.shipmentSelected)
                                vm.shipmentSelected.CreateTime = moment().unix();
                            vm.shipmentSelectedIndex = vm.lstShipment.length > 0 ? vm.lstShipment.length - 1 : 0;
                            vm.isShowShipmentDetail = true;
                            vm.isShowBtnAddShipment = false;
                            $scope.$apply(vm.isShowShipmentDetail);
                            $scope.$apply(vm.isShowBtnAddShipment);
                        }

                    },
                    error: function (err) {
                        $("#btnAddShipment").prop("disabled", false);
                        alert(err.statusText);
                    }
                });
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });

        }
        function showBtnAddPackage() {
            if (vm.shipmentSelectedIndex != -1) {
                for (var i = 0; i < vm.lstPackageAvaiable.length; i++) {
                    if (vm.lstPackageAvaiable[i].checked) {
                        vm.isShowBtnAddPackage = true;
                        return;
                    }
                }
            }
            vm.isShowBtnAddPackage = false;
        }
        function addPackageInShipment() {
            if (vm.shipmentSelectedIndex != -1) {
                var lstIndex = [];
                var lsttemp = angular.copy(vm.lstPackageAvaiable);
                if (!vm.shipmentSelected.LstPackage) {
                    vm.shipmentSelected.LstPackage = [];
                }
                for (var i = 0; i < lsttemp.length; i++) {
                    if (lsttemp[i].checked) {
                        if (lsttemp[i].WarehouseId != vm.shipmentSelected.WarehouseId) {
                            notification(notifiType.warning, "Package " + lsttemp[i].Code + ' does not belong to the current selected warehouse', "Warning");
                        }
                        else {
                            for (var j = 0; j < vm.lstPackageAvaiable.length; j++) {
                                if (vm.lstPackageAvaiable[j].Id == lsttemp[i].Id) {
                                    var item = lsttemp[i];
                                    item.checked = false;
                                    vm.lstPackageAvaiable.splice(j, 1);
                                    vm.shipmentSelected.LstPackage.push(item);
                                    break;
                                }
                            }
                        }

                    }
                }
                vm.showBtnAddPackage();
                vm.computedWeightShipmentSelected();
                vm.computedWeightShipmentSelectedPound();
                vm.computedWeightPackageAvaiable();
                vm.computedWeightPackageAvaiablePound();
            }
            else {
                notification(notifiType.warning, "Please select shipment", "Warning");
            }
        }
        function saveShipment() {

            $.ajax({
                url: "/Shipment/SaveShipment",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: vm.shipmentSelected,
                success: function (result) {
                    if (result.Message) {
                        notification(notifiType.error, result.Message, "Error");
                    }
                    else {
                        notification(notifiType.success, lb_UpdateSucceed, "Success");
                        init();
                    }
                },
                error: function (err) {
                    notification(notifiType.error, err.statusText, "Error");
                }
            });
        }

        function showBtnRemovePackage() {
            if (vm.shipmentSelectedIndex != -1) {
                for (var i = 0; i < vm.shipmentSelected.LstPackage.length; i++) {
                    if (vm.shipmentSelected.LstPackage[i].checked) {
                        vm.isShowBtnRemovePackage = true;
                        return;
                    }
                }
            }
            vm.isShowBtnRemovePackage = false;
        }
        function removePackageInShipment() {
            if (vm.shipmentSelectedIndex != -1) {
                var lstIndex = [];
                var lsttemp = angular.copy(vm.shipmentSelected.LstPackage);
                for (var i = 0; i < lsttemp.length; i++) {
                    if (lsttemp[i].checked) {
                        for (var j = 0; j < vm.shipmentSelected.LstPackage.length; j++) {
                            if (vm.shipmentSelected.LstPackage[j].Id == lsttemp[i].Id) {
                                var item = lsttemp[i];
                                item.checked = false;
                                vm.lstPackageAvaiable.push(item);
                                vm.shipmentSelected.LstPackage.splice(j, 1);
                                break;
                            }
                        }
                    }
                }
                vm.showBtnRemovePackage();
                vm.computedWeightShipmentSelected();
                vm.computedWeightPackageAvaiable();
            }
        }

        function setShipmentSelected(index) {
            vm.shipmentSelectedIndex = index;
            vm.shipmentSelected = vm.lstShipment[index];
            vm.isShowShipmentDetail = true;
            vm.isShowBtnAddShipment = true;
            vm.filter.WarehouseId = vm.shipmentSelected.WarehouseId;
            //$scope.$apply(vm.filter.WarehouseId);

            vm.getPackageAvaiable();
        }
        vm.FromDatePost = moment().format("MM/DD/YYYY");
        vm.ToDatePost = moment().format("MM/DD/YYYY");
        vm.ChangeFromDate = function (item) {
            vm.FromDatePost = moment(item).format("MM/DD/YYYY");
        }
        vm.ChangeToDate = function (item) {
            vm.ToDatePost = moment(item).format("MM/DD/YYYY");
        }
        vm.setDate = function (date) {
            vm.FromDatePost = date;
            vm.ToDatePost = date;
            vm.FromDate = new Date(date);
            vm.ToDate = new Date(date);
            searchShipment()
        }
        function searchShipment() {
            vm.lstShipment = [];
            vm.filter.FromTime = (moment(vm.FromDatePost + " 00:00", "MM/DD/YYYY HH:mm").valueOf() || 0) / 1000;
            vm.filter.ToTime = (moment(vm.ToDatePost + " 23:59", "MM/DD/YYYY HH:mm").valueOf() || 0) / 1000;
            $.ajax({
                url: "/Shipment/SearchShipment",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: vm.filter,
                success: function (result) {
                    if (result.Message) {
                        notification(notifiType.error, result.Message, "Error");
                    }
                    else {
                        vm.lstShipment = result.Result;
                        for (var i = 0; i < vm.lstShipment.length; i++) {
                            vm.lstShipment[i].checked = false;
                            if (vm.lstShipment[i].LstPackage) {
                                for (var j = 0; j < vm.lstShipment[i].LstPackage.length; j++) {
                                    vm.lstShipment[i].LstPackage[j].CreateDate = moment.unix(vm.lstShipment[i].LstPackage[j].CreateTime).format(vm.formatCurrent);
                                    vm.lstShipment[i].LstPackage[j].checked = false;
                                }
                            }


                        }
                        $scope.$apply(vm.lstShipment);
                    }

                },
                error: function (err) {
                    notification(notifiType.error, err.statusText, "Error");
                }
            });

            if (vm.shipmentSelectedIndex != -1) {
                vm.getPackageAvaiable();
            }
        }

        function getPackageAvaiable() {
            vm.lstPackageAvaiable = [];

            if (vm.showAdvanceSearch) {
                var fromDate = $("#ip-fromdate").val()
                vm.filter.FromDate = moment($("#ip-fromdate").val(), "YYYY-MM-DD").format(vm.formatCurrent);
                vm.filter.ToDate = moment($("#ip-todate").val(), "YYYY-MM-DD").format(vm.formatCurrent);
            }

            $.ajax({
                url: "/Shipment/SearchPackage",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: vm.filter,
                success: function (result) {
                    if (result.Message) {
                        notification(notifiType.error, result.Message, "Error");
                    }
                    else {
                        vm.lstPackageAvaiable = result.Result;
                        for (var i = 0; i < vm.lstPackageAvaiable.length; i++) {
                            vm.lstPackageAvaiable[i].CreateDate = moment.unix(vm.lstPackageAvaiable[i].CreateTime).format(vm.formatCurrent);
                            vm.lstPackageAvaiable[i].checked = false;
                        }
                        vm.filter.total = result.Optional;
                        vm.pager = pagerService.GetPager(vm.filter.total, vm.filter.PageIndex + 1, vm.filter.PageSize);
                        vm.filter.totalPages = vm.pager.totalPages;
                        $scope.$apply(vm.lstPackageAvaiable);
                    }

                },
                error: function (err) {
                    notification(notifiType.error, err.statusText, "Error");
                }
            });
        }

        function showDetailOrder(orderId) {
            location.href = "/Order/DetailOrder/" + orderId;
        }

        function setShowAdvanceSearch(flag) {
            vm.showAdvanceSearch = flag;
        }

        function init() {
            vm.filter.ToDate = moment().format("DD/MM/YYYY HH:mm:ss");
            vm.searchShipment();
            vm.isShowShipmentDetail = false;
            vm.isShowBtnAddShipment = true;
            //$scope.$apply(vm.isShowShipmentDetail);
            //$scope.$apply(vm.isShowBtnAddShipment);
        }

        function setDateText(date) {
            vm.filter.FromDate = date;
            vm.filter.ToDate = date;
            vm.filter.PageIndex = 0;
            vm.getPackageAvaiable();
        }

        function setPage(page) {
            if (page >= 1 && page <= vm.filter.totalPages + 1) {
                vm.filter.PageIndex = page - 1;
                vm.getPackageAvaiable();
            }
        }

        init();
    }
})();


angular.module('OrderApp').controller('ModalInstanceCtrl', function ($uibModalInstance, $scope, items) {
    var $ctrl = this;
    $ctrl.items = items.destinations;
    $ctrl.warehouse = items.warehouse;
    $ctrl.selected = {
        destinationId: $ctrl.items[0].Id,
        warehouseId: $ctrl.warehouse[0].AttributeId
    };

    $ctrl.ok = function () {
        $uibModalInstance.close($ctrl.selected);
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});

// Please note that the close and dismiss bindings are from $uibModalInstance.

angular.module('OrderApp').component('modalComponent', {
    templateUrl: 'myModalContent.html',
    bindings: {
        resolve: '<',
        close: '&',
        dismiss: '&'
    },
    controller: function () {
        var $ctrl = this;

        $ctrl.$onInit = function () {
            $ctrl.items = $ctrl.resolve.items;
            $ctrl.selected = {
                item: $ctrl.items[0]
            };
        };

        $ctrl.ok = function () {
            $ctrl.close({ $value: $ctrl.selected.item });
        };

        $ctrl.cancel = function () {
            $ctrl.dismiss({ $value: 'cancel' });
        };
    }
});