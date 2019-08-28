(function () {
    'use strict';
    angular
        .module('OrderApp')
        .controller('OrderController', OrderController);

    OrderController.$inject = ['$http', '$scope', '$filter', '$timeout', '$q', '$compile'];

    function OrderController($http, $scope, $filter, $timeout, $q, $compile) {
        var vm = this;
        vm.lstPackage = [];
        vm.citySender = citySender;
        vm.cityRecipient = cityRecipient;
        vm.districtRecipient = [];
        vm.categoryStore = category;
        vm.orderDetail = {};
        vm.orderId = orderId;
        vm.packageShow = new packageModel();
        vm.packageIndexSelected = -1;
        vm.Title = orderId == 0 ? "Create an Order" : "Order Detail";
        vm.senderInfo = vm.orderDetail.SenderInfo;
        vm.recipientInfo = vm.orderDetail.RecipientInfo;

        vm.itemPackageInfo = new itemPackageModel();
        vm.showItemPackageInfo = false;
        vm.showbtnSubmit = true;
        vm.resultInfo = [];

        // dialog
        var wsCSNoteDialog;

        //function
        /// package
        vm.addPackage = addPackage;
        vm.removePackage = removePackage;
        vm.updatePackage = updatePackage;
        vm.oneMorePackage = oneMorePackage;
        vm.setPackageSelected = setPackageSelected;


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


        function addPackage() {
            vm.packageShow.Ordinal = vm.lstPackage.length + 1;
            vm.lstPackage.push(vm.packageShow);
            vm.packageShow = new packageModel();
        }
        function removePackage() {
            if (vm.packageIndexSelected != -1) {
                vm.lstPackage.splice(vm.packageShow, 1);
                vm.packageShow = new packageModel();
                vm.packageIndexSelected = -1;
            }
        }
        function updatePackage() {
            vm.lstPackage.splice(vm.packageShow, 1);
            vm.lstPackage.splice(vm.packageIndexSelected, 0, vm.packageShow);
        }
        function oneMorePackage() {
            vm.packageShow = new packageModel();
            vm.packageIndexSelected = -1;
        }
        function setPackageSelected(index) {
            vm.packageIndexSelected = index;
            vm.packageShow = vm.lstPackage[index];
        }

        function getNameCategory(categoryId) {
            for (var i = 0; i < vm.categoryStore.length; i++) {
                if (vm.categoryStore[i].Id == categoryId) {
                    return vm.categoryStore[i].Name;
                }
            }
        }
        function oneMoreItemPackage() {
            vm.itemPackageInfo = new itemPackageModel();
            vm.showItemPackageInfo = true;
            $("#sl-category").trigger("change");
            vm.showbtnSubmit = false;
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
        function removeItemPackage(item) {
            vm.packageShow.Items.splice(item, 1);
        }
        function clearItemPackage() {
            vm.showItemPackageInfo = false;
            vm.showbtnSubmit = true;
        }


        function calc_TotalItem() {
            return vm.packageShow.Items.length;
        }
        function calc_TotalValue() {
            var total = 0
            for (var i = 0 ; i < vm.packageShow.Items.length ; i++) {
                total += parseFloat(vm.packageShow.Items[i].Value);
            }
            return total;
        }
        function calc_TotalFee() {
            var total = 0
            total = parseFloat(vm.packageShow.Weight) * parseFloat(vm.packageShow.Fee);
            return total;
        }
        function calc_TotalCharge() {
            var total = 0
            total = parseFloat(vm.packageShow.Weight) * parseFloat(vm.packageShow.Fee);
            return total;
        }

        function submitOrder() {

            vm.senderInfo.CityId = $("#sl-city-sender").val();
            vm.senderInfo.StateId = $("#sl-state-sender").val();

            vm.recipientInfo.CityId = $("#sl-city-recipient").val();
            vm.recipientInfo.DistrictId = $("#sl-district-recipient").val();
            vm.orderDetail.Destination = $('#sl-city-recipient option:selected').data("airport");
            vm.orderDetail.SenderInfo = vm.senderInfo;
            vm.orderDetail.RecipientInfo = vm.recipientInfo;
            vm.orderDetail.Packages = vm.lstPackage;
            var data = {
                model: vm.orderDetail
            };
            $.ajax({
                url: "/Order/SubmitOrder",
                type: 'POST',
                params: { contentType: "application/json;" },
                dataType: 'json',
                data: data,
                success: function (result) {
                    if (result.Message) {
                        alert(result.Message);

                    }
                    else if (result.Result > 0) {
                        if (vm.orderId == 0) {
                            location.href = "/Order/DetailOrder/" + result.Result;
                        }
                        else {
                            alert("Cập nhật thành công!");
                        }
                    }
                    else {
                        alert("Cập nhật không thành công!");
                    }

                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        }

        function printLabel() {
            alert("print thành công!");
        }
        function printInvoice() {
            alert("print thành công!")
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
                Unit: '',
                Id: 0
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
                        vm.orderDetail = result.Result;
                        vm.orderDetail.CreateDate = getFormatDateDDMMYYYYY(new Date(parseInt(vm.orderDetail.CreateDate.replace('/Date(', '').replace(')/', ''))));
                        $scope.$apply(vm.orderDetail);
                        if (vm.orderDetail.Id > 0) {
                            vm.senderInfo = vm.orderDetail.SenderInfo;
                            vm.recipientInfo = vm.orderDetail.RecipientInfo;
                            $scope.$apply(vm.senderInfo);
                            $scope.$apply(vm.recipientInfo);

                            $("#sl-city-sender").val(vm.senderInfo.CityId + '');
                            $("#sl-state-sender").val(vm.senderInfo.StateId + '');

                            $("#sl-city-recipient").val(vm.recipientInfo.CityId);
                            $("#sl-city-recipient").trigger('change');
                            for (var i = 0 ; i < vm.orderDetail.Packages.length; i++) {
                                vm.lstPackage.push(vm.orderDetail.Packages[i]);
                            }

                            $scope.$apply(vm.lstPackage);
                            if (callback) {
                                callback();
                            }
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

        function autocomplete(inp, typeUser) {
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
                    data: { phone: val, typeUser: typeUser },
                    success: function (result) {
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
                                            switch (typeUser) {
                                                case 1:
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
    }
})();


