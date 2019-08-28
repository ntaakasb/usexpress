var availableState = [
    "Alabama",
    "Alaska",
    "Arizona",
    "Arkansas",
    "California",
    "Colorado", "Connecticut", "Delaware", "Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New", "Hampshire", "New", "Jersey", "New", "Mexico", "New", "York", "North", "Carolina", "North", "Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode", "Island", "South", "Carolina", "South", "Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", "Washington", "West", "Virginia", "Wisconsin", "Wyoming", "Washington, D.C"
];

var Store = {
    /* Validate form*/
    ValidateStoreForm: function () {

        $("#frm_update_store").validate({
            rules: {
                "StoreAccount.FullName": "required",
                "StoreAccount.StoreName": "required",
                "StoreAccount.Phone": {
                    required: true,
                    //IsPhoneUS: true
                },
                "StoreAccount.Email": {
                    required: true,
                    email: true
                    //IsNotExistEmail: true 
                },
                "StoreAccount.Password": "required",
                "StoreAccount.CityId": "required",
                "StoreAccount.StateId": "required",
                "StoreAccount.Zip": {
                    required: true,
                    maxlength: 5
                },
                "RoleID": {
                    SelectRoleUser: true
                },
                "StoreAccount.WarehouseID": {
                    SelectWarehouse: true
                },
                "StoreAccount.Code": {
                    IsNotExistCode: true
                },
                "StoreAccount.WarehouseId": {
                    required: function (element) {
                        return isADmin;
                    }
                }

            },
            messages: {
                "StoreAccount.FullName": "Fullname required!",
                "StoreAccount.StoreName": "Store name required!",
                "StoreAccount.Email": {
                    required: "Email required!",
                    email: "Email is wrong format!"
                },
                "StoreAccount.Password": "Password required!",
                "StoreAccount.CityId": "City required!",
                "StoreAccount.StateID": "State required!",
                "StoreAccount.Phone": {
                    required: "Phone number required!"

                },
                "StoreAccount.Zip": {
                    required: "Zip code required",
                    maxlength: "Max length is 5"
                },
                "StoreAccount.Code": {
                    IsNotExistCode: "Code had already! please input another code"
                },
                "StoreAccount.WarehouseId": {
                    required: "Please select your warehouse"
                }
            }
        });

        $("#frm_create_store").validate({
            rules: {
                "StoreAccount.FullName": "required",
                "StoreAccount.StoreName": "required",
                "StoreAccount.Phone": {
                    required: true,
                    IsPhoneUS: true
                },
                "StoreAccount.Email": {
                    required: true,
                    email: true
                    //IsNotExistEmail: true 
                },
                "StoreAccount.Password": "required",
                "StoreAccount.CityId": "required",
                "StoreAccount.StateId": "required",
                "StoreAccount.Zip": {
                    required: true,
                    maxlength: 5
                },
                "RoleID": {
                    SelectRoleUser: true
                },
                "StoreAccount.WarehouseID": {
                    SelectWarehouse: true
                },
                "StoreAccount.Code": {
                    IsNotExistCode: true
                },
                "StoreAccount.WarehouseId": {
                    required: function (element) {
                        return isADmin;
                    }
                }

            },
            messages: {
                "StoreAccount.FullName": "Please enter your full name",
                "StoreAccount.StoreName": "Please enter your store name",
                "StoreAccount.Email": {
                    required: "Please enter a valid email address",
                    email: "Email is wrong format"
                },
                "StoreAccount.Password": "Please enter password",
                "StoreAccount.CityId": "Please select city!",
                "StoreAccount.StateID": "Please select an State",
                "StoreAccount.Phone": {
                    required: "Please enter your phone"

                },
                "StoreAccount.Zip": {
                    required: "Please enter zip code",
                    maxlength: "Max length is 5"
                },
                "StoreAccount.Code": {
                    IsNotExistCode: "Code had already! please input another code"
                },
                "StoreAccount.WarehouseId": {
                    required: "Please select your warehouse"
                }
            }
        });



    },
    ValidateFormSender: function () {
        $("#frm_create_sender").validate({
            rules: {
                "FullName": "required",
                "Add1": "required",
                "CityId": "required",
                "StateID": "required",
                "Phone": "IsPhoneUS",
                //"Email": {
                //    required: true,
                //    email: true
                //},
                "Zip": {
                    required: true,
                    maxlength: 5,
                    minlength: 5
                }
            },
            messages: {
                "FullName": "Please enter your full name",
                "Add1": "Please fill address",
                "CityId": "Please select city!",
                "StateID": "Please select an State",
                "Phone": {
                    IsPhoneUS: "Phone number is not format correct"
                },
                "Email": {
                    required: "Please enter your email",
                    email: "Email is format wrong"
                },
                "Zip": {
                    required: "Please enter your Zip code",
                    maxlength: "Zip code only 5 digit",
                    minlength: "Zip code must 5 digit"
                }

            }
        });
    },
    ValidateFormReciver: function () {
        $("#frm_create_reciver").validate({
            rules: {
                "FullName": "required",
                "Add1": "required",
                "CityId": "required",
                "Phone": "required"
            },
            messages: {
                "FullName": "Please enter your full name",
                "Add1": "Please fill address",
                "CityId": "Please select city!",
                "Phone": "Phone number is not format correct"

            }
        });
    },
    /*end validate*/
    ResetForm: function (formID) {
        $(formID)[0].reset();
    },
    UpdateStoreSuccess: function (avg) {
        alert(avg.Message);
        if (avg.Result > 0) {
            window.location.href = '/Store';
        }
    },
    CityChangeEvent: function (_this) {
        var value = _this.value;
        $.ajax({

            url: '/Common/LoadDistrictByCity',
            type: 'GET',
            data: {
                'CityID': value
            },
            dataType: 'json',
            success: function (data) {
                $('#DistrictID').html(data);
                Store.DistrictChangeEvent();
                try {
                    var str = "";
                    if ($("#Add1").val() != null && $("#Add1").val() != "") {
                        str += $("#Add1").val();
                    }

                    if ($("#WardId").val() != null && $("#WardId").val() != "") {
                        str += " , " + $("#WardId :selected").text();
                    }
                    if ($("#DistrictID").val() != null && $("#DistrictID").val() != "") {
                        str += " , " + $("#DistrictID :selected").text();
                    }
                    if ($("#CityId").val() != null && $("#CityId").val() != "") {
                        str += " , " + $("#CityId :selected").text();
                    }
                    $("#FullAddress").val(str);
                } catch (e) {

                }
            }
        });
    },
    DistrictChangeEvent: function (_this) {
        var value = _this.value;
        $.ajax({

            url: '/Common/LoadWardByDistrictID',
            type: 'GET',
            data: {
                'DistrictID': value
            },
            dataType: 'json',
            success: function (data) {
                $('#WardId').html(data);
                try {
                    var str = "";
                    if ($("#Add1").val() != null && $("#Add1").val() != "") {
                        str += $("#Add1").val();
                    }

                    if ($("#WardId").val() != null && $("#WardId").val() != "") {
                        str += " , " + $("#WardId :selected").text();
                    }
                    if ($("#DistrictID").val() != null && $("#DistrictID").val() != "") {
                        str += " , " + $("#DistrictID :selected").text();
                    }
                    if ($("#CityId").val() != null && $("#CityId").val() != "") {
                        str += " , " + $("#CityId :selected").text();
                    }
                    $("#FullAddress").val(str);
                } catch (e) {

                }
            }
        });
    },
    //LoadWarehouseByEvent: function () {
    //    var city = $('#CityId option:selected').val();
    //    var state = $('#StateID option:selected').val();
    //    $.ajax({

    //        url: '/Common/LoadWarehouse',
    //        type: 'GET',
    //        data: {
    //            'CityID': city,
    //            'StateID': state
    //        },
    //        dataType: 'json',
    //        success: function (data) {
    //            $('#WarehouseID').html(data);
    //            var warehouseID = $('#hWareHouseID').val();
    //            if (warehouseID != -1) {
    //                $('#WarehouseID').val(warehouseID);
    //            }
    //            else {
    //                $("#WarehouseID").val($("#target option:first").val());
    //            }
    //        }
    //    });
    //},
    //StateChangeEvent: function () {
    //    Store.LoadWarehouseByEvent();
    //},
    UpdateSenderSuccess: function (avg) {
        alert(avg.Message);
        if (avg.Result > 0) {
            window.location.href = '/store/SenderList';
        }
    },
    UpdateRecipientSuccess: function (avg) {
        alert(avg.Message);
        if (avg.Result > 0) {
            window.location.href = '/store/RecipientList';
        }
    }
}
