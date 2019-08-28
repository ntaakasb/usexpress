(function () {
    'use strict';
    angular
        .module('OrderApp')
        .controller('ManageController', ManageController);

    ManageController.$inject = ['$http', '$scope', '$filter', '$timeout', '$q', '$compile', 'moment', 'pagerService'];

    function ManageController($http, $scope, $filter, $timeout, $q, $compile, moment, pagerService) {
        var vm = this;
        vm.formatCurrent = "MMM DD YYYY";
        vm.formatFromServer = "DD/MM/YYYY HH:mm:ss"
        vm.lstDate = [];
        $scope.fromDate = new Date();
        $scope.toDate = (new Date()).addDays(-6);
        vm.showAdvanceSearch = false;
        vm.lstResult = [];
        vm.pager = {};
        vm.showDetailOrder = showDetailOrder;
        vm.filter = {
            Keyword: '',
            TypeSearch: 0,
            FromDate: '',
            PageIndex: 0,
            PageSize: 15,
            ToDate: '',
            TotalPages: 1,
            total: 0,
            IsActive: true
            //StoreId: -1
        };
        vm.options = [{ Id: 0, Name: 'Id' }, { Id: 1, Name: 'sender' }];
        vm.search = search;
        vm.setDateText = setDateText;
        vm.setShowAdvanceSearch = setShowAdvanceSearch;
        vm.setPage = setPage;
        vm.changeStatus = changeStatus;
        vm.storeList = [];
        vm.SearchOrder = function () {
            SearchOrder('');
        }
        console.log(vm)
        vm.setActive = function (item) {

        }

        function getStoreList() {
            $.ajax({
                type: "POST",
                url: "/Order/LoadListStore",
                dataType: "json",
                success: function (rs) {
                    vm.storeList = rs;
                },
                error: function () {

                }
            });
        }
        function changeStatus(item) {
            if (item.StatusIdChange==2) {
                $.ajax({
                    url: "/Order/CheckMultiRecipient",
                    type: 'POST',
                    params: { contentType: "application/json;" },
                    dataType: 'json',
                    async: false,
                    data: { id: item.Id },
                    success: function (result) {
                        var data = result;
                        var lstPack = "";
                        if (data.length > 2) {
                            var str = data[0].RecipientName + " has " + data.length + " packages. Do you want to combine all packages ID ";
                            for (var i = 0; i < data.length; i++) {
                                lstPack += data[i].Code + ","
                            }
                            lstPack = lstPack.substring(0, lstPack.length - 1);
                            str += lstPack + " into one Kerry's bill?";
                            if (confirm(str)) {
                                $.ajax({
                                    url: "/Order/UpdateListStatusPackage",
                                    type: 'POST',
                                    params: { contentType: "application/json;" },
                                    dataType: 'json',
                                    data: { lstId: lstPack, id: item.Id, statusUpdate: item.StatusIdChange },
                                    success: function (result) {
                                        if (result.Message) {
                                            notification(notifiType.success, result.Message, lb_succeed);
                                        }
                                        vm.search();

                                    },
                                    error: function (err) {
                                        notification(notifiType.error, err.statusText, lb_error);
                                    }
                                });
                            }
                            else {
                                sendStatus(item.Id, item.StatusIdChange);
                            }
                        }
                        else {
                            if (confirm("Bạn chắc chứ?")) {
                                sendStatus(item.Id, item.StatusIdChange);
                            }
                        }

                    },
                    error: function (err) {

                    }
                });
            }
            else {
                sendStatus(item.Id, item.StatusIdChange);
            }
            
            //if (confirm("Bạn chắc chứ?")) {
            //    $.ajax({
            //        url: "/Order/UpdateStatusPackage",
            //        type: 'POST',
            //        params: { contentType: "application/json;" },
            //        dataType: 'json',
            //        data: { id: item.Id, statusUpdate: item.StatusIdChange },
            //        success: function (result) {
            //            if (result.Message) {
            //                notification(notifiType.success, result.Message, "Thành công");
            //            }
            //            vm.search();

            //        },
            //        error: function (err) {
            //            notification(notifiType.error, err.statusText, "Error");
            //        }
            //    });
            //}
        }
        function sendStatus(Id, StatusIdChange) {
            $.ajax({
                url: "/Order/UpdateStatusPackage",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: { id: Id, statusUpdate: StatusIdChange },
                success: function (result) {
                    if (result.Message) {
                        notification(notifiType.success, result.Message, lb_succeed);
                    }
                    vm.search();

                },
                error: function (err) {
                    notification(notifiType.error, err.statusText, lb_error);
                }
            });
        }
        function search() {
            vm.lstResult = [];

            if (vm.showAdvanceSearch) {
                var fromDate = $("#ip-fromdate").val()
                vm.filter.FromDate = moment($("#ip-fromdate").val(), "YYYY-MM-DD").format(vm.formatCurrent);
                vm.filter.ToDate = moment($("#ip-todate").val(), "YYYY-MM-DD").format(vm.formatCurrent);
            }

            $.ajax({
                url: "/Order/SearchOrder",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: vm.filter,
                success: function (result) {
                    if (result.Message) {
                        alert(result.Message);

                    }
                    else {
                        vm.lstResult = result.Result;
                        var offset = moment().utcOffset();
                        for (var i = 0; i < vm.lstResult.length; i++) {
                            vm.lstResult[i].CreateDate = moment(vm.lstResult[i].CreateDateLocalString, vm.formatFromServer).format(vm.formatCurrent);
                            vm.lstResult[i].PickupDate = vm.lstResult[i].PickupTime > 0 ? moment.unix(vm.lstResult[i].PickupTime).format(vm.formatCurrent) : "";
                            vm.lstResult[i].ShippingDate = vm.lstResult[i].ShippingTime > 0 ? moment.unix(vm.lstResult[i].ShippingTime).format(vm.formatCurrent) : "";
                            vm.lstResult[i].ClearCustomDate = vm.lstResult[i].ClearCustomTime > 0 ? moment.unix(vm.lstResult[i].ClearCustomTime).format(vm.formatCurrent) : "";
                            vm.lstResult[i].DeliverDate = vm.lstResult[i].DeliverTime > 0 ? moment.unix(vm.lstResult[i].DeliverTime).format(vm.formatCurrent) : "";
                            vm.lstResult[i].StatusIdChange = -1;
                        }
                        vm.filter.total = result.Optional;
                        vm.pager = pagerService.GetPager(vm.filter.total, vm.filter.PageIndex + 1, vm.filter.PageSize);
                        vm.filter.totalPages = vm.pager.totalPages;
                        console.log(vm.pager);
                        console.log(vm.pager.currentPage); 
                        $scope.$apply(vm.lstResult);
                    }

                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        }

        vm.setActive = function (item) {
            $.ajax({
                type: "POST",
                url: "/Order/SetActiveOrder",
                data: JSON.stringify({ idOrder: item.OrderId }),
                contentType: "application/json; charset:utf-8",
                dataType: "json",
                success: function (rs) {
                    if (rs === false) {
                        notification(notifiType.success, lb_UpdateSuccessful, lb_succeed);
                        vm.search();
                    } else {
                        notification(notifiType.error, lb_UpdateFailed, lb_error);
                    }
                },
                error: function (rs) {

                }
            })
        }

        function showDetailOrder(orderId) {
            location.href = "/Order/DetailOrder/" + orderId;
        }
        function setShowAdvanceSearch(flag) {
            vm.showAdvanceSearch = flag;
        }
        function init() {
            for (var i = 14; i >= 0; i--) {
                vm.lstDate.push(getFormatDateDDMMYYYYY((new Date()).addDays(-i)));
            }
            getStoreList();
            //vm.search();
        }
        function setDateText(date) {
            vm.filter.FromDate = date;
            vm.filter.ToDate = date;
            vm.filter.PageIndex = 0;
            vm.search();
        }
        function setPage(page) {
            if (page >= 1 && page <= vm.filter.totalPages + 1) {
                vm.filter.PageIndex = page - 1;
                vm.search();
                //vm.loadData(function (rs) {
                //    if (rs.Result != null) {
                //        $.each(rs.Result, function (idx, val) {
                //            //$.extend(val, {
                //            //    TimeRangeText: shiftService.getTimespanText(val.TimeStart) + ' - ' + shiftService.getTimespanText(val.TimeEnd)
                //            //});
                //            val.CreatedOn = shiftService.getFormatDateDDMMYYYYY(shiftService.dateTimeJson(val.CreatedOn));
                //            vm.lstEquipment.push(val);
                //        });

                //    }
                //    vm.filter.total = rs.Optional;
                //    vm.pager = pagerService.GetPager(vm.filter.total, vm.filter.PageIndex + 1, vm.filter.PageSize);
                //    vm.filter.totalPages = vm.pager.totalPages;
                //});
            }
        }

        // utils format
        function getFormatDateDDMMYYYYY(date) {
            return date.toLocaleString('en-us', { year: 'numeric', month: '2-digit', day: '2-digit' })
                .replace(/(\d+)\/(\d+)\/(\d+)/, '$2/$1/$3');
        }
        init();
    }
})();
Date.prototype.addDays = function (days) {
    this.setDate(this.getDate() + parseInt(days));
    return this;
};