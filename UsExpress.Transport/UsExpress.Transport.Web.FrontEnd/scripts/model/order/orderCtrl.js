(function () {
    'use strict';
    angular
        .module('OrderApp')
        .controller('OrderController', OrderController);

    OrderController.$inject = ['$http', '$scope', '$filter', '$timeout', '$q', '$compile', 'moment'];

    function OrderController($http, $scope, $filter, $timeout, $q, $compile, moment) {
        var vm = this;
        vm.lstPackage = [];
        vm.citySender = citySender;
        vm.cityRecipient = cityRecipient;
        vm.districtRecipient = [];
        vm.wardRecipient = [];
        vm.categoryStore = category;
        vm.orderDetail = {};
        vm.orderId = orderId;
        vm.packageShow = new packageModel();
        vm.packageIndexSelected = -1;
        vm.Title = orderId == 0 ? "Create an Order" : "Order Detail";
        vm.maxItemPackage = 5;

        vm.senderInfo = vm.orderDetail.SenderInfo;
        vm.recipientInfo = vm.orderDetail.RecipientInfo;

        vm.itemPackageInfo = new itemPackageModel();
        vm.showItemPackageInfo = false;
        vm.showbtnSubmit = true;
        vm.resultInfo = [];
        vm.resultCategory = [];
        vm.lockInput = false;
        var availableState = [
            "Alabama",
            "Alaska",
            "Arizona",
            "Arkansas",
            "California",
            "Colorado", "Connecticut", "Delaware", "Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New", "Hampshire", "New", "Jersey", "New", "Mexico", "New", "York", "North", "Carolina", "North", "Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode", "Island", "South", "Carolina", "South", "Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", "Washington", "West", "Virginia", "Wisconsin", "Wyoming", "Washington, D.C"
        ];


        // dialog
        var wsCSNoteDialog;

        vm.prefix_id_item_package = "category_";

        //function
        /// package
        vm.addPackage = addPackage;
        vm.removePackage = removePackage;
        vm.updatePackage = updatePackage;
        vm.oneMorePackage = oneMorePackage;
        vm.setPackageSelected = setPackageSelected;
        vm.createItemDefault = createItemDefault;

        /// itempackage
        vm.getNameCategory = getNameCategory;
        vm.oneMoreItemPackage = oneMoreItemPackage;
        vm.addItemPackage = addItemPackage;
        vm.removeItemPackage = removeItemPackage;
        vm.clearItemPackage = clearItemPackage;

        vm.submitOrder = submitOrder;
        vm.getDetailOrder = getDetailOrder;
        vm.showDialogDetail = showDialogDetail;
        vm.printLabel = printLabel;
        vm.printInvoice = printInvoice;

        vm.calc_TotalItem = calc_TotalItem;
        vm.calc_TotalValue = calc_TotalValue;
        vm.calc_TotalFee = calc_TotalFee;
        vm.calc_TotalCharge = calc_TotalCharge;
        vm.saveSender = saveSender;
        vm.saveRecipient = saveRecipient;



        $scope.autocompeleteDescription = function (_this) {
            $(".autocompleteinput").autocomplete({
                source: lsProductDescription,
                select: function (event, ui) {
                    loadDetailProduct(_this, ui.item.value, 'des', 0);

                },
            });
        }

        $scope.autocompeleteBarcode = function (_this) {
            $(".autocompletebarcode").autocomplete({
                source: lsBarcode,
                select: function (event, ui) {
                    loadDetailProduct(_this, ui.item.value, 'bar', 0);

                },
            });
        }
        $scope.loadDetailProduct = function (_this, type) {
            var value = '';
            if (type == 'des') {
                value = _this.item.Description;
            }
            if (type == 'bar') {
                value = _this.item.BarCode;
            }
           
            loadDetailProduct(_this, value, type, 1);
        }

        function loadDetailProduct(_this, value, type, isLoad) {
            var suffix_id = _this.$index;
            if (productList != null && productList.length > 0) {
                if (isLoad == 0) {
                    $.each(productList, function (index, item) {
                        if (type == 'des') {
                            if (item.Description == value) {


                                vm.packageShow.Items[_this.$index].BarCode = item.BarCode;
                                vm.packageShow.Items[_this.$index].Code = item.ScheduleBCode;
                                vm.packageShow.Items[_this.$index].CategoryId = item.CategoryID;
                                vm.packageShow.Items[_this.$index].hasFocus = true;
                                $("#category_" + _this.$index).select2("val", item.CategoryID);

                                $scope.$apply(vm.packageShow);
                                //return true;
                            }

                        }
                        if (type == 'bar') {
                            if (item.BarCode == value) {
                                vm.packageShow.Items[_this.$index].Description = item.Description;
                                vm.packageShow.Items[_this.$index].Code = item.ScheduleBCode;
                                vm.packageShow.Items[_this.$index].CategoryId = item.CategoryID;
                                $("#category_" + _this.$index).select2("val", item.CategoryID);
                                vm.packageShow.Items[_this.$index].hasFocus = true;
                                $scope.$apply(vm.packageShow);
                                //return true;
                            }

                        }

                    })
                }

            }
            if (!checkBarCode(value) && $('#bar_code_' + _this.$index).val() != '' && type == 'bar') {
                vm.packageShow.Items[_this.$index].IsNew = true;
            }
            else if (!checkDescription(value) && $('#desctiption_code_' + _this.$index).val() != '' && type == 'des') {
                vm.packageShow.Items[_this.$index].IsNew = true;
            }
            else {
                vm.packageShow.Items[_this.$index].IsNew = false;
            }
        }

        function checkBarCode(barcode) {
            var exist = false;
            if (productList != null && productList.length > 0) {
                $.each(productList, function (index, item) {
                    if (item.BarCode == barcode) {
                        exist = true;
                    }
                })
            }
            return exist;
        }

        function checkDescription(des) {
            var exist = false;
            if (productList != null && productList.length > 0) {
                $.each(productList, function (index, item) {
                    if (item.Description == des) {
                        exist = true;
                    }
                })
            }
            return exist;
        }

        $scope.addNewProduct = function (_index) {

            productItem.Description = vm.packageShow.Items[_index].Description;
            productItem.BarCode = vm.packageShow.Items[_index].BarCode;
            productItem.ScheduleBCode = vm.packageShow.Items[_index].Code;
            productItem.CategoryID = vm.packageShow.Items[_index].CategoryId;

            if (productItem.Description == '') {
                notification(notifiType.warning, "Vui lòng nhập description", "Required");
                $('#desctiption_code_' + _index).focus();
                return false;
            }
            if (productItem.BarCode == '') {
                notification(notifiType.warning, "Vui lòng nhập barcode", "Required");
                $('#bar_code_' + _index).focus();
                return false;
            }
            if (productItem.CategoryID == '') {
                notification(notifiType.warning, "Vui lòng chọn category", "Required");
                return false;
            }

            $.ajax({
                url: '/Product/InsertProduct',
                type: 'POST',
                data: {
                    'model': productItem
                },
                dataType: 'json',
                success: function (data) {
                   
                    if (data.Result > 0) {
                        notification(notifiType.success, data.Message, "Success");
                        vm.packageShow.Items[_index].IsNew = false;
                        $('#btnAddProduct_' + _index).addClass('ng-hide');
                    }
                    else {
                        notification(notifiType.error, data.Message, "Error");
                    }
                }
            });
        }




        function createItemDefault() {
            for (var i = 0; i < vm.maxItemPackage; i++) {
                vm.packageShow.Items.push(new itemPackageModel());
            }
            addEventAutoCompleteCategory();
        }
        function addPackage() {

            if (!checkValidateItem()) {
                showMessageItemWrong();
                return;
            }
            if (vm.calc_TotalItem() < 1) {
                notification(notifiType.warning, "Bạn chưa nhập các mặt hàng vào package", "Required");
                return;
            }
            vm.packageShow.Ordinal = vm.lstPackage.length + 1;
            vm.lstPackage.push(vm.packageShow);
            vm.packageShow = new packageModel();
            vm.createItemDefault();
        }
        function removePackage() {
            if (vm.packageIndexSelected != -1) {
                vm.lstPackage.splice(vm.packageIndexSelected, 1);
                vm.packageShow = new packageModel();
                vm.packageIndexSelected = -1;
            }
            if (vm.lstPackage.length < 1) {
                vm.createItemDefault();
            }
            else {
                addEventAutoCompleteCategory();
            }
        }


        function updatePackage() {
            if (!checkValidateItem()) {
                showMessageItemWrong();
                return;
            }
            vm.lstPackage.splice(vm.packageShow, 1);
            vm.lstPackage.splice(vm.packageIndexSelected, 0, vm.packageShow);
            submitOrder();
        }
        function oneMorePackage() {
            if (!checkValidateItem()) {
                showMessageItemWrong();
                return;
            }
            vm.packageShow = new packageModel();
            vm.createItemDefault();
            vm.packageIndexSelected = -1;
        }
        function setPackageSelected(index) {
            vm.packageIndexSelected = index;
            vm.packageShow = vm.lstPackage[index];
            addEventAutoCompleteCategory();
        }

        function getNameCategory(categoryId) {
            for (var i = 0; i < vm.categoryStore.length; i++) {
                if (vm.categoryStore[i].Id == categoryId) {
                    return vm.categoryStore[i].Name;
                }
            }
        }
        function oneMoreItemPackage() {
            vm.createItemDefault();
            //vm.itemPackageInfo = new itemPackageModel();
            //vm.showItemPackageInfo = true;
            //$("#sl-category").trigger("change");
            //vm.showbtnSubmit = false;
        }
        function addItemPackage() {
            vm.showItemPackageInfo = false;
            vm.showbtnSubmit = true;
            vm.itemPackageInfo.CategoryId = parseInt($("#sl-category").val());
            for (var i = 0; i < vm.categoryStore.length; i++) {
                if (vm.categoryStore[i].Id == vm.itemPackageInfo.CategoryId) {
                    vm.itemPackageInfo.Code = vm.categoryStore[i].Code;
                    break;
                }
            }
            vm.packageShow.Items.push(vm.itemPackageInfo);
        }
        function removeItemPackage(index) {
            vm.packageShow.Items.splice(index, 1);
        }
        function clearItemPackage() {
            vm.showItemPackageInfo = false;
            vm.showbtnSubmit = true;
        }


        function calc_TotalItem() {
            var count = 0;
            for (var i = 0; i < vm.packageShow.Items.length; i++) {
                if (vm.packageShow.Items[i].CategoryId > 0 && vm.packageShow.Items[i].Quantity > 0) {
                    count += vm.packageShow.Items[i].Quantity;
                }
            }
            return count;
        }
        function calc_TotalValue() {
            var total = 0
            for (var i = 0; i < vm.packageShow.Items.length; i++) {
                if (vm.packageShow.Items[i].CategoryId > 0 && vm.packageShow.Items[i].Quantity > 0 && vm.packageShow.Items[i].Value > 0) {
                    total += (parseFloat(vm.packageShow.Items[i].Value) * parseInt(vm.packageShow.Items[i].Quantity));
                }
            }
            return parseFloat(total).toFixed(2);
        }
        function calc_TotalFee() {
            var total = 0
            total = Math.ceil(vm.packageShow.Weight) * parseFloat(vm.packageShow.Fee);
            return total;
        }
        function calc_TotalCharge() {
            var total = 0
            total = parseFloat(vm.packageShow.Weight) * parseFloat(vm.packageShow.Fee);
            return total;
        }

        function submitOrder() {
            var isError = 0;
            $('input').removeClass('err-boder');
            $('select').removeClass('err-boder');
            showLoading();

            var sender_phone = $('#ip-sender-phone').val();
            var sender_fullName = $('input[name="senderInfo.FullName"]').val();
            var senderInfo_AddressLine1 = $('input[name="senderInfo.AddressLine1"]').val();
            var senderInfo_AddressLine2 = $('input[name="senderInfo.AddressLine2"]').val();
            var senderInfo_CityName = $('input[name="senderInfo.CityName"]').val();
            var senderInfo_StateName = $('input[name="senderInfo.StateName"]').val();
            var senderInfo_Zip = $('input[name="senderInfo.Zip"]').val();
            var senderInfo_Email = $('input[name="senderInfo.Email"]').val();

            var recipientInfo_phone = $('#ip-recipient-phone').val();
            var recipientInfo_FullName = $('input[name="recipientInfo.FullName"]').val();
            var recipientInfo_AddressLine1 = $('input[name="recipientInfo.AddressLine1"]').val();
            var recipientInfo_AddressLine2 = $('input[name="recipientInfo.AddressLine2"]').val();
            var recipientInfor_City = $('#s2id_sl-city-recipient option:selected').text().replace(/\s/g, '')
            var recipientInfor_District = $('#sl-district-recipient option:selected').text().replace(/\s/g, '');
            var recipientInfor_Ward = $('#sl-ward-recipient option:selected').text().replace(/\s/g, '');

            if (sender_phone == '') {
                $('#ip-sender-phone').addClass('err-boder');
                isError = 1;
            }

            if (sender_fullName == '') {
                $('input[name="senderInfo.FullName"]').addClass('err-boder');
                isError = 1;
            }
            if (senderInfo_AddressLine1 == '') {
                $('input[name="senderInfo.AddressLine1"]').addClass('err-boder');
                isError = 1;
            }
            if (senderInfo_CityName == '') {
                $('input[name="senderInfo.CityName"]').addClass('err-boder');
                isError = 1;
            }
            if (senderInfo_StateName == '') {
                $('input[name="senderInfo.StateName"]').addClass('err-boder');
                isError = 1;
            }
            if (senderInfo_Zip == '') {
                $('input[name="senderInfo.Zip"]').addClass('err-boder');
                isError = 1;
            }

            if (isError > 0) {
                notification(notifiType.warning, "Bạn chưa nhập đẩy đủ thông tin sender", "Required");
            }

            if (recipientInfo_phone == '') {
                $('#ip-recipient-phone').addClass('err-boder');
                isError = 1;
            }
            if (recipientInfo_FullName == '') {
                $('input[name="recipientInfo.FullName"]').addClass('err-boder');
                isError = 1;
            }
            if (recipientInfo_AddressLine1 == '') {
                $('input[name="recipientInfo.AddressLine1"]').addClass('err-boder');
                isError = 1;
            }


            if (recipientInfor_District == 'Quận/Huyện') {
                $('#sl-district-recipient').addClass('err-boder');
                isError = 1;
            }
            if (recipientInfor_Ward == 'Phường/xã') {
                $('#sl-ward-recipient').addClass('err-boder');
                isError = 1;

            }
            if (isError > 0) {
                notification(notifiType.warning, "Bạn chưa nhập đầy đủ thông tin recipient", "Required");
                hideLoading();
                return;
            }


            if (vm.lstPackage.length < 1) {
                notification(notifiType.warning, "Bạn chưa nhập thông tin package", "Required");
                hideLoading();
                return;
            }
            for (var i = 0; i < vm.lstPackage.length; i++) {
                vm.packageShow = vm.lstPackage[i];
                // remove item không hợp lệ trước khi post
                var packageCheck = angular.copy(vm.packageShow);
                for (var j = 0; j < packageCheck.Items.length; j++) {
                    if (vm.packageShow.Items[j].CategoryId < 0) {
                        vm.packageShow.Items.splice(j, 1);
                    }
                }
                if (!checkValidateItem() || vm.calc_TotalItem < 1) {
                    notification(notifiType.error, "Gói hàng thứ " + (i + 1) + " dữ liệu không hợp lệ. Bạn hãy kiểm tra lại!", "Error");
                    hideLoading();
                    return;
                }

            }

            vm.senderInfo.CityId = $("#sl-city-sender").val();
            vm.senderInfo.StateId = $("#sl-state-sender").val();

            vm.recipientInfo.CityId = $("#sl-city-recipient").val();
            vm.recipientInfo.DistrictId = $("#sl-district-recipient").val();
            vm.recipientInfo.WardId = $("#sl-ward-recipient").val();

            vm.orderDetail.Destination = $('#sl-city-recipient option:selected').data("airport");
            vm.orderDetail.SenderInfo = vm.senderInfo;
            vm.orderDetail.RecipientInfo = vm.recipientInfo;
            vm.orderDetail.Packages = vm.lstPackage;

            vm.orderDetail.TotalItems = vm.calc_TotalItem;
            vm.orderDetail.TotalDeclareValues = vm.calc_TotalValue;
            vm.orderDetail.ShippingFree = vm.calc_TotalFee;

            if (vm.orderDetail.Id < 1) {
                vm.orderDetail.CreateTime = parseInt(moment().unix().valueOf());//parseInt(vm.orderDetail.CreateTime / 1000);
            }
            var data = {
                model: vm.orderDetail
            };

            $.ajax({
                url: "/Order/SubmitOrder",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: data,
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    if (result.Message) {
                        alert(result.Message);

                    }
                    else if (result.Result > 0) {
                        if (vm.orderId == 0) {
                            notification(notifiType.success, "Tạo order thành công!", "Success");
                            vm.orderId = result.Result;
                            vm.Title = "Thank you";
                            //$scope.$apply(vm.orderId);
                            //$scope.$apply(vm.Title);
                            vm.getDetailOrder();
                            disableAllInput();
                        }
                        else {
                            notification(notifiType.success, "Cập nhật thành công!", "Success");
                        }
                    }
                    else {
                        notification(notifiType.error, "Cập nhật không thành công!", "Error");
                    }
                    hideLoading();

                },
                error: function (err) {
                    hideLoading();
                    alert(err.statusText);
                }
            });
        }

        function printLabel() {
            if (vm.packageShow.Id > 0) {
                var link = '/Order/PrintLabel/' + vm.packageShow.Id;
                window.open(link, "_blank", "left=100,top=50,width=1000,height=600,toolbar=0,scrollbars=1,status=0")
            }
            else {
                notification(notifiType.warning, "Bạn chưa chọn gói hàng để in", "Warning");
            }

        }
        function printInvoice() {
            if (vm.packageShow.Id > 0) {
                var link = '/Order/PrintInVoice/' + vm.packageShow.Id;
                window.open(link, "_blank", "left=100,top=50,width=1000,height=600,toolbar=0,scrollbars=1,status=0")
            }
            else {
                notification(notifiType.warning, "Bạn chưa chọn gói hàng để in", "Warning");
            }
        }

        function showMessageItemWrong() {
            notification(notifiType.warning, "Bạn chưa nhập xong dữ liệu!", "Required");
        }

        function setItemHasFocus(index) {
            vm.packageShow.Items[index].hasFocus = true;
        }
        function checkValidateItem() {
            var result = true;
            for (var i = 0; i < vm.packageShow.Items.length; i++) {
                var item = vm.packageShow.Items[i];
                if (item.hasFocus || item.CategoryId > 0) {
                    if (item.Description != "" && item.Quantity > 0 && item.Unit != "" && item.Value > 0 && item.CategoryId > 0) {
                        item.hasFocus = false;
                    }
                    else {
                        return false;
                    }
                }
            }
            return result;
        }

        function saveSender() {
            if ($scope.create_order1.$valid) {
                vm.senderInfo.TypeUser = 1;
                vm.senderInfo.CityId = $("input[name='senderInfo.CityName']").val();
                vm.senderInfo.StateId = $("#ip-StateId").val();

                var f = true;

                if ($("#create_order1 button").text() == "UPDATE THIS SENDER") {
                    f = confirm("Do you accept update your inputed information for this sender ?");
                }

                if (f) {
                    var data = {
                        model: vm.senderInfo
                    };
                    console.log(vm.senderInfo);
                    updateShippingInfo(data);
                }
            }
        }

        function saveRecipient() {
            if ($scope.create_order2.$valid) {
                vm.recipientInfo.TypeUser = 2;

                vm.recipientInfo.CityId = $("#sl-city-recipient").val();
                vm.recipientInfo.DistrictId = $("#sl-district-recipient").val();
                vm.recipientInfo.WardId = $("#sl-ward-recipient").val();
                var data = {
                    model: vm.recipientInfo
                };
                updateShippingInfo(data);
            }
        }
        function updateShippingInfo(data) {
            $.ajax({
                url: "/Order/UpdateShippingInfo",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: data,
                success: function (result) {
                    if (result.Message) {
                        alert(result.Message);

                    }
                    else {
                        switch (result.Result) {
                            case -2:
                                notification(notifiType.error, "Update Info dont success!", "Error");
                                break;
                            case -1:
                                notification(notifiType.warning, "Phone number has exists!", "Warning");
                                break;
                            case 0:
                                notification(notifiType.error, "Update Info dont success in database", "Error");
                                break;
                            default:
                                notification(notifiType.success, "Update info success", "Success");
                                switch (data.model.TypeUser) {
                                    case 1:
                                        vm.senderInfo.Id = result.Result;
                                        break;
                                    case 2:
                                        vm.recipientInfo.Id = result.Result;
                                        break;
                                }
                                break;
                        }
                    }

                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        }

        function disableAllInput() {
            $(".cus_disabled").prop("disabled", true);
        }

        /// model
        function packageModel() {
            return {
                Weight: '',
                Fee: '',
                Heigh: '',
                Width: '',
                Depth: '',
                Items: [],
                Expedite: false,
                Ordinal: 0,
                TotalCharge: 0,
                TotalFee: 0,
                Id: 0,
                WarehouseId: 0,
                Destination: 0
            };
        }

        function itemPackageModel() {
            return {
                Description: '',
                CategoryId: '',
                Code: '',
                Quantity: '',
                Value: '',
                Unit: 'PCS',
                Id: 0,
                NameCategory: '',
                hasFocus: false,
                BarCode: '',
                IsNew: false
            };
        }

        // utils format
        function getFormatDateDDMMYYYYY(date) {
            return date.toLocaleString('en-us', { year: 'numeric', month: '2-digit', day: '2-digit' })
                .replace(/(\d+)\/(\d+)\/(\d+)/, '$2/$1/$3');
        }
        init();
        var shiftDialog;
        function init() {
            $(document).ready(function () {
                $("#sl-city-recipient").select2(
                    //{
                    //data: { results: vm.cityRecipient, text: 'Name' },
                    //}
                );
                $("#sl-city-recipient").on("change", function (b) {
                    var cityId = $("#sl-city-recipient").val();
                    vm.districtRecipient = [];
                    if (parseInt(cityId) > 0) {
                        $.ajax({
                            url: "/Common/GetDistrictByCityId",
                            type: 'POST',
                            params: { contentType: "application/json;" },
                            dataType: 'json',
                            data: { cityId: cityId },
                            success: function (result) {
                                if (result.Message) {
                                    alert(result.Message);

                                }
                                else {
                                    vm.districtRecipient = result.Result;
                                    $scope.$apply(vm.districtRecipient);
                                    if (vm.recipientInfo.DistrictId) {
                                        $("#sl-district-recipient").val(vm.recipientInfo.DistrictId);
                                        $("#sl-district-recipient").trigger("change");
                                    }
                                    else {
                                        $("#sl-district-recipient").val(0);
                                    }
                                }

                            },
                            error: function (err) {
                                alert(err.statusText);
                            }
                        });
                    }
                });
                $("#sl-category").on("change", function (b) {

                    $("#td-code-selected").html($("#sl-category option:selected").data("code"));
                });

                $("#sl-district-recipient").on("change", function (b) {
                    var districtId = $("#sl-district-recipient").val();
                    vm.wardRecipient = [];
                    if (parseInt(districtId) > 0) {
                        $.ajax({
                            url: "/Common/GetWardByDistrictId",
                            type: 'POST',
                            params: { contentType: "application/json;" },
                            dataType: 'json',
                            data: { districtId: districtId },
                            success: function (result) {
                                if (result.Message) {
                                    alert(result.Message);

                                }
                                else {
                                    vm.wardRecipient = result.Result;
                                    $scope.$apply(vm.wardRecipient);
                                    if (vm.recipientInfo.WardId) {
                                        $("#sl-ward-recipient").val(vm.recipientInfo.WardId);
                                    }
                                    else {
                                        $("#sl-ward-recipient").val(0);
                                    }
                                }

                            },
                            error: function (err) {
                                alert(err.statusText);
                            }
                        });
                    }
                });
                $("#sl-category").on("change", function (b) {

                    $("#td-code-selected").html($("#sl-category option:selected").data("code"));
                });
            });

            vm.getDetailOrder();

            autocomplete(document.getElementById("ip-sender-phone"), 1);
            autocomplete(document.getElementById("ip-recipient-phone"), 2);

            vm.createItemDefault();
            $("#ip-StateId").autocomplete({
                source: availableState
            });

            //setTimeout(function () {
            //    shiftDialog = $("#model-dialog").dialog({
            //        autoOpen: false,
            //        height: 650,
            //        width: 700,
            //        modal: false,
            //        buttons: {
            //            Apply: self.onsubmit,
            //            Cancel: function () {
            //                shiftDialog.dialog("close");
            //            }
            //        },
            //        close: function () {
            //            self.initParentShifts();
            //        }
            //    });
            //}, 500);


        }

        function getDetailOrder(callback) {
            vm.lstPackage = [];
            $.ajax({
                url: "/Order/GetDetailOrderById",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: { id: vm.orderId },
                success: function (result) {
                    if (result.Message) {
                        alert(result.Message);
                    }
                    else {
                        debugger
                        vm.orderDetail = result.Result;
                        if (vm.orderDetail.Id < 1) {
                            genCodeOrder(vm.orderDetail.Code);
                            vm.orderDetail.CreateTime = parseInt(moment().utc().valueOf());
                        }
                        vm.orderDetail.CreateDateLocalString = moment(vm.orderDetail.CreateTime).format("DD/MM/YYYY HH:mm:ss");

                        $scope.$apply(vm.orderDetail);
                        vm.senderInfo = vm.orderDetail.SenderInfo;
                        vm.recipientInfo = vm.orderDetail.RecipientInfo;
                        $scope.$apply(vm.senderInfo);
                        $scope.$apply(vm.recipientInfo);
                        if (vm.orderDetail.Id > 0) {
                            $("#sl-city-sender").val(vm.senderInfo.CityId + '');
                            $("#sl-state-sender").val(vm.senderInfo.StateId + '');

                            $("#sl-city-recipient").val(vm.recipientInfo.CityId);
                            $("#sl-city-recipient").trigger('change');

                            for (var i = 0; i < vm.orderDetail.Packages.length; i++) {
                                vm.lstPackage.push(vm.orderDetail.Packages[i]);
                            }

                            $scope.$apply(vm.lstPackage);
                            if (callback) {
                                callback();
                            }
                            vm.setPackageSelected(0);
                        }
                    }

                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        }

        function showDialogDetail(orderId, dialogId) {
            vm.orderId = orderId;
            vm.getDetailOrder(function () {
                //$("#" + dialogId).dialog({
                //    dialogClass: "no-close",
                //    buttons: [
                //      {
                //          text: "OK",
                //          click: function () {
                //              $(this).dialog("close");
                //          }
                //      }
                //    ]
                //});
                //shiftDialog.dialog("open");
            });

        }

        ///
        function addEventAutoCompleteCategory() {
            setTimeout(function () {
                var autoCates = document.getElementsByClassName("auto-category");
                for (var i = 0; i < autoCates.length; i++) {
                    //autocompleteCategory(document.getElementById(prefix + i));
                    select2Category(document.getElementById(vm.prefix_id_item_package + i));
                }
                for (var i = 0; i < vm.packageShow.Items.length; i++) {
                    if (vm.packageShow.Items[i].CategoryId > 0) {
                        $("#" + vm.prefix_id_item_package + i).select2("val", (vm.packageShow.Items[i].CategoryId));
                    }
                }
            }, 100)


        }
        function genCodeOrder(storeCode) {
            var d = new Date,
                dformat = moment().format("YYMMDDHHmmss");//[(d.getFullYear() % 1000).padLeft(), (d.getMonth() + 1).padLeft(), d.getDate().padLeft(), d.getHours().padLeft(), d.getMinutes().padLeft(), d.getSeconds().padLeft()].join('');
            vm.orderDetail.Code = storeCode + dformat;
        }
        function autocomplete(inp, typeUser) {
            if (inp == null) {
                return;
            }
            /*the autocomplete function takes two arguments,
            the text field element and an array of possible autocompleted values:*/
            var currentFocus;
            /*execute a function when someone writes in the text field:*/
            inp.addEventListener("input", function (e) {
                var a, b, i, val = this.value;
                /*close any already open lists of autocompleted values*/
                closeAllLists();
                if (!val) { return false; }
                currentFocus = -1;
                /*create a DIV element that will contain the items (values):*/
                a = document.createElement("DIV");
                a.setAttribute("id", this.id + "autocomplete-list");
                a.setAttribute("class", "autocomplete-items");
                /*append the DIV element as a child of the autocomplete container:*/
                this.parentNode.appendChild(a);
                /*for each item in the array...*/
                $.ajax({
                    url: "/Common/SearchPhone",
                    type: 'POST',
                    params: { contentType: "application/json;" },
                    dataType: 'json',
                    data: { phone: val, typeUser: typeUser, storeId: vm.orderDetail.StoreId },
                    success: function (result) {
                        // Default set button
                        $("#create_order1 button").text("SAVE THIS SENDER");
                        $("#create_order2 button").text("SAVE THIS RECIPIENT");
                        console.log(result);
                        vm.resultInfo = [];
                        for (i = 0; i < result.Result.length; i++) {
                            /*check if the item starts with the same letters as the text field value:*/
                            if (result.Result[i].Phone.substr(0, val.length).toUpperCase() == val.toUpperCase()) {
                                vm.resultInfo.push(result.Result[i]);
                                /*create a DIV element for each matching element:*/
                                b = document.createElement("DIV");
                                /*make the matching letters bold:*/
                                b.innerHTML = "<strong>" + result.Result[i].Phone.substr(0, val.length) + "</strong>";
                                b.innerHTML += result.Result[i].Phone.substr(val.length);
                                b.innerHTML += '|' + result.Result[i].FullName;
                                /*insert a input field that will hold the current array item's value:*/
                                b.innerHTML += "<input type='hidden' value='" + result.Result[i].Id + "'>";
                                /*execute a function when someone clicks on the item value (DIV element):*/
                                b.addEventListener("click", function (e) {
                                    /*insert the value for the autocomplete text field:*/
                                    //inp.value = this.getElementsByTagName("input")[0].value;
                                    var id = this.getElementsByTagName("input")[0].value;
                                    for (var j = 0; j < vm.resultInfo.length; j++) {
                                        if (vm.resultInfo[j].Id == parseInt(id)) {
                                            inp.value = vm.resultInfo[j].Phone;
                                            if (typeUser == 1) {
                                                $("#create_order1 button").text("UPDATE THIS SENDER");
                                            }
                                            else {
                                                $("#create_order2 button").text("UPDATE THIS RECIPIENT");
                                            }
                                            switch (typeUser) {
                                                case 1:
                                                    //debugger;
                                                    vm.senderInfo = vm.resultInfo[j];
                                                    $scope.$apply(vm.senderInfo);
                                                    break;
                                                case 2:
                                                    vm.recipientInfo = vm.resultInfo[j];
                                                    $scope.$apply(vm.recipientInfo);
                                                    if (vm.recipientInfo.CityId > 0) {
                                                        $("#sl-city-recipient").val(vm.recipientInfo.CityId + '');
                                                        $("#sl-city-recipient").trigger('change');
                                                    }
                                                    break;
                                            }
                                            break;
                                        }
                                    }
                                    /*close the list of autocompleted values,
                                    (or any other open lists of autocompleted values:*/
                                    closeAllLists();
                                });
                                a.appendChild(b);
                            }
                        }

                    },
                    error: function (err) {
                        alert(err.statusText);
                    }
                });

            });
            /*execute a function presses a key on the keyboard:*/
            inp.addEventListener("keydown", function (e) {
                var x = document.getElementById(this.id + "autocomplete-list");
                if (x) x = x.getElementsByTagName("div");
                if (e.keyCode == 40) {
                    /*If the arrow DOWN key is pressed,
                    increase the currentFocus variable:*/
                    currentFocus++;
                    /*and and make the current item more visible:*/
                    addActive(x);
                } else if (e.keyCode == 38) { //up
                    /*If the arrow UP key is pressed,
                    decrease the currentFocus variable:*/
                    currentFocus--;
                    /*and and make the current item more visible:*/
                    addActive(x);
                } else if (e.keyCode == 13) {
                    /*If the ENTER key is pressed, prevent the form from being submitted,*/
                    e.preventDefault();
                    if (currentFocus > -1) {
                        /*and simulate a click on the "active" item:*/
                        if (x) x[currentFocus].click();
                    }
                }
            });
            function addActive(x) {
                /*a function to classify an item as "active":*/
                if (!x) return false;
                /*start by removing the "active" class on all items:*/
                removeActive(x);
                if (currentFocus >= x.length) currentFocus = 0;
                if (currentFocus < 0) currentFocus = (x.length - 1);
                /*add class "autocomplete-active":*/
                x[currentFocus].classList.add("autocomplete-active");
            }
            function removeActive(x) {
                /*a function to remove the "active" class from all autocomplete items:*/
                for (var i = 0; i < x.length; i++) {
                    x[i].classList.remove("autocomplete-active");
                }
            }
            function closeAllLists(elmnt) {
                /*close all autocomplete lists in the document,
                except the one passed as an argument:*/
                var x = document.getElementsByClassName("autocomplete-items");
                for (var i = 0; i < x.length; i++) {
                    if (elmnt != x[i] && elmnt != inp) {
                        x[i].parentNode.removeChild(x[i]);
                    }
                }
            }
            /*execute a function when someone clicks in the document:*/
            document.addEventListener("click", function (e) {
                closeAllLists(e.target);
            });
        }

        function select2Category(inp) {
            if (inp == null || $(inp).hasClass("select2-offscreen")) {
                return;
            }
            $(inp).select2({
                placeholder: "Select category",
                Width: '100%',
                allowClear: true,
                ajax: {
                    url: "/Common/SearchCategory",
                    type: 'POST',
                    params: { contentType: "application/json;" },
                    dataType: 'json',
                    quietMillis: 250,
                    data: function (term, page) { // page is the one-based page number tracked by Select2
                        return JSON.stringify({
                            keyword: term, //search term
                            top: 10 // page number
                        });
                    },
                    results: function (data, page) {
                        //var more = (page * 30) < data.total_count; // whether or not there are more results available

                        //// notice we return the value of more so Select2 knows if more results can be loaded
                        //return { results: data.items, more: more };
                        return {
                            results:
                                data.Result.map(function (item) {
                                    return $.extend(item, {
                                        id: item.Id,
                                    });
                                }
                                )
                        }
                    }
                },
                initSelection: function (element, callback) {
                    // the input tag has a value attribute preloaded that points to a preselected repository's id
                    // this function resolves that id attribute to an object that select2 can render
                    // using its formatResult renderer - that way the repository name is shown preselected
                    var id = $(element).val();

                    if (id !== "") {
                        var data = {
                            id: id
                        };
                        $.ajax({
                            url: "/Common/GetDetailCategoryById",
                            data: JSON.stringify(data),
                            type: 'POST',
                            contentType: 'application/json',
                            success: function (rs) {
                                //callback(result.Result[0]);
                                var results;
                                results = [];
                                if (rs.Result != null) {
                                    results.push($.extend(rs.Result, {
                                        id: rs.Result.Id,
                                    })
                                    );
                                    callback(results[0]);
                                }
                                else {
                                    callback(results);
                                }
                            }
                        });
                    }
                },
                formatResult: setTemplateSelection, // omitted for brevity, see the source of this page
                formatSelection: setTemplateSelection, // omitted for brevity, see the source of this page
                dropdownCssClass: "width-100", // apply css that makes the dropdown taller
                escapeMarkup: function (m) { return m; }
            });
            $(inp).on("select2-opening", function (e) {
                var suffix_id = parseInt(e.target.id.replace(vm.prefix_id_item_package, ""));
                if (!checkValidateItem()) {
                    for (var u = 0; u < vm.packageShow.Items.length; u++) {
                        if (vm.packageShow.Items[u].hasFocus == true && u != suffix_id) {
                            showMessageItemWrong();
                            $(this).val("");
                            return false;
                        }
                    }
                }
            }).on("select2-selecting", function (e) {
                console.log("selecting val=" + e.val + " choice=" + JSON.stringify(e.object));
                var suffix_id = parseInt(e.target.id.replace(vm.prefix_id_item_package, ""));
                vm.packageShow.Items[suffix_id].Code = e.object.AttributeCode;
                vm.packageShow.Items[suffix_id].CategoryId = e.object.Id;
                vm.packageShow.Items[suffix_id].hasFocus = true;
                $scope.$apply(vm.packageShow);
            }).on("select2-removed", function (e) {
                console.log("removed val=" + e.val + " choice=" + JSON.stringify(e.choice));
                var suffix_id = parseInt(e.target.id.replace(vm.prefix_id_item_package, ""));
                vm.packageShow.Items[suffix_id].Code = "";
                vm.packageShow.Items[suffix_id].CategoryId = 0;
                vm.packageShow.Items[suffix_id].hasFocus = false;
                $scope.$apply(vm.packageShow);
            }).on("change", function (e) {
                if (vm.packageIndexSelected != -1) {
                    var suffix_id = parseInt(e.target.id.replace(vm.prefix_id_item_package, ""));
                    if (e.added) {
                        vm.packageShow.Items[suffix_id].hasFocus = false;
                    }
                    else {
                        vm.packageShow.Items[suffix_id].hasFocus = true;
                    }
                }
                //console.log("change " + JSON.stringify({ val: e.val }));
            });


        }
        function format(item) { return item.Name; }
        function setTemplateSelection(item) {
            if (item.loading) {
                return item.Name;
            }
            return item.Name || item.Name;
        }
        function autocompleteCategory(inp) {
            if (inp == null) {
                return;
            }
            /*the autocomplete function takes two arguments,
            the text field element and an array of possible autocompleted values:*/
            var currentFocus;
            /*execute a function when someone writes in the text field:*/
            var prefix_id = "category_";
            var prefix_code = "category_code_";
            inp.addEventListener("input", function (e) {
                var a, b, i, val = this.value;
                var idCurrent = this.id;
                var suffix_id = parseInt(idCurrent.replace(prefix_id, ""));
                var codeCurrent = prefix_code + suffix_id;
                if (!checkValidateItem()) {
                    for (var u = 0; u < vm.packageShow.Items.length; u++) {
                        if (vm.packageShow.Items[u].hasFocus == true && u != suffix_id) {
                            showMessageItemWrong();
                            $(this).val("");
                            return;
                        }
                    }
                }

                /*close any already open lists of autocompleted values*/
                closeAllLists();
                if (!val) { return false; }
                currentFocus = -1;
                /*create a DIV element that will contain the items (values):*/
                a = document.createElement("DIV");
                a.setAttribute("id", this.id + "autocomplete-list");
                a.setAttribute("class", "autocomplete-items");
                /*append the DIV element as a child of the autocomplete container:*/
                this.parentNode.appendChild(a);
                /*for each item in the array...*/
                $.ajax({
                    url: "/Common/SearchCategory",
                    type: 'POST',
                    params: { contentType: "application/json;" },
                    dataType: 'json',
                    data: { keyword: val, top: 10 },
                    success: function (result) {
                        vm.resultCategory = [];
                        for (i = 0; i < result.Result.length; i++) {
                            /*check if the item starts with the same letters as the text field value:*/
                            if (result.Result[i].Name.substr(0, val.length).toUpperCase() == val.toUpperCase()) {
                                vm.resultCategory.push(result.Result[i]);
                                /*create a DIV element for each matching element:*/
                                b = document.createElement("DIV");
                                /*make the matching letters bold:*/
                                b.innerHTML = "<strong>" + result.Result[i].Name.substr(0, val.length) + "</strong>";
                                b.innerHTML += result.Result[i].Name.substr(val.length);
                                //b.innerHTML += '|' + result.Result[i].Name;
                                /*insert a input field that will hold the current array item's value:*/
                                b.innerHTML += "<input type='hidden' value='" + result.Result[i].Id + "' data-code='" + result.Result[i].AttributeCode + "'>";
                                /*execute a function when someone clicks on the item value (DIV element):*/
                                b.addEventListener("click", function (e) {
                                    /*insert the value for the autocomplete text field:*/
                                    //inp.value = this.getElementsByTagName("input")[0].value;
                                    var id = this.getElementsByTagName("input")[0].value;

                                    for (var j = 0; j < vm.resultCategory.length; j++) {
                                        if (vm.resultCategory[j].Id == parseInt(id)) {
                                            inp.value = vm.resultCategory[j].Name;
                                            //$("#" + codeCurrent).html(vm.resultCategory[j].AttributeCode);
                                            vm.packageShow.Items[suffix_id].Code = vm.resultCategory[j].AttributeCode;
                                            vm.packageShow.Items[suffix_id].CategoryId = vm.resultCategory[j].Id;
                                            setItemHasFocus(suffix_id);
                                            $scope.$apply(vm.packageShow);
                                            break;
                                        }
                                    }
                                    /*close the list of autocompleted values,
                                    (or any other open lists of autocompleted values:*/
                                    closeAllLists();
                                });
                                a.appendChild(b);
                            }
                        }

                    },
                    error: function (err) {
                        alert(err.statusText);
                    }
                });

            });
            /*execute a function presses a key on the keyboard:*/
            inp.addEventListener("keydown", function (e) {
                var x = document.getElementById(this.id + "autocomplete-list");
                if (x) x = x.getElementsByTagName("div");
                if (e.keyCode == 40) {
                    /*If the arrow DOWN key is pressed,
                    increase the currentFocus variable:*/
                    currentFocus++;
                    /*and and make the current item more visible:*/
                    addActive(x);
                } else if (e.keyCode == 38) { //up
                    /*If the arrow UP key is pressed,
                    decrease the currentFocus variable:*/
                    currentFocus--;
                    /*and and make the current item more visible:*/
                    addActive(x);
                } else if (e.keyCode == 13) {
                    /*If the ENTER key is pressed, prevent the form from being submitted,*/
                    e.preventDefault();
                    if (currentFocus > -1) {
                        /*and simulate a click on the "active" item:*/
                        if (x) x[currentFocus].click();
                    }
                }
            });
            function addActive(x) {
                /*a function to classify an item as "active":*/
                if (!x) return false;
                /*start by removing the "active" class on all items:*/
                removeActive(x);
                if (currentFocus >= x.length) currentFocus = 0;
                if (currentFocus < 0) currentFocus = (x.length - 1);
                /*add class "autocomplete-active":*/
                x[currentFocus].classList.add("autocomplete-active");
            }
            function removeActive(x) {
                /*a function to remove the "active" class from all autocomplete items:*/
                for (var i = 0; i < x.length; i++) {
                    x[i].classList.remove("autocomplete-active");
                }
            }
            function closeAllLists(elmnt) {
                /*close all autocomplete lists in the document,
                except the one passed as an argument:*/
                var x = document.getElementsByClassName("autocomplete-items");
                for (var i = 0; i < x.length; i++) {
                    if (elmnt != x[i] && elmnt != inp) {
                        x[i].parentNode.removeChild(x[i]);
                    }
                }
            }
            /*execute a function when someone clicks in the document:*/
            document.addEventListener("click", function (e) {
                closeAllLists(e.target);
            });


        }




    }
})();


Number.prototype.padLeft = function (base, chr) {
    var len = (String(base || 10).length - String(this).length) + 1;
    return len > 0 ? new Array(len).join(chr || '0') + this : this;
}
