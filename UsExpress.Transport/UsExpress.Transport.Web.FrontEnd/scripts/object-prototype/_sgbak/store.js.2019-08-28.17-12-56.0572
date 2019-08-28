var Store = {
    /* Validate form*/
    ValidateStoreForm: function () {

        $("#frm_update_store").validate({
            rules: {
                "StoreAccount.FullName": "required",
                "StoreAccount.StoreName": "required",
                "StoreAccount.Phone": {
                    required: true,
                    IsPhoneUS: true
                },
                "StoreAccount.Email": {
                    required: true,
                    email: true,
                    IsNotExistEmail: true 
                },
                "StoreAccount.Password": "required",
                "StoreAccount.CityId": "required",
                "StoreAccount.StateID": "required",
                "StoreAccount.Zip": {
                    required: true,
                    maxlength: 5
                },
                "RoleID": {
                    SelectRoleUser: true
                },
                "StoreAccount.WarehouseID": {
                    SelectWarehouse: true
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
                }
            }
        });

        $("#frm_create_store").validate({
            rules: {
                "FullName": "required",
                "StoreName": "required",
                "Zip": {
                    required: true,
                    maxlength: 5
                },
                "Phone": {
                    required: true,
                    IsPhoneUS: true
                },
                "Email": {
                    required: true,
                    email: true,
                    IsNotExistEmail: true 
                },
                "Password": "required",
                "CityId": "required",
                "StateID": "required"
            },
            messages: {
                "FullName": "Please enter your full name",
                "StoreName": "Please enter your store name",
                "Phone": "Please enter your phone number",
                //"Email": "Please enter a valid email address",
                "Password": "Please enter password",
                "CityId": "Please select city!",
                "StateID": "Please select an State",
                "Phone": "Phone number is format wrong"

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
                "Phone": "required"
            },
            messages: {
                "FullName": "Please enter your full name",
                "Add1": "Please fill address",
                "CityId": "Please select city!",
                "StateID": "Please select an State",
                "Phone": "Phone number is format ifrrect"

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
                "Phone": "Phone number is format ifrrect"

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
                Store.LoadWarehouseByEvent();
            }
        });
    },
    LoadWarehouseByEvent: function () {
        var city = $('#CityId option:selected').val();
        var state = $('#StateID option:selected').val();
        $.ajax({

            url: '/Common/LoadWarehouse',
            type: 'GET',
            data: {
                'CityID': city,
                'StateID': state
            },
            dataType: 'json',
            success: function (data) {
                $('#WarehouseID').html(data);
                var warehouseID = $('#hWareHouseID').val();
                if (warehouseID != -1) {
                    $('#WarehouseID').val(warehouseID);
                }
                else {
                    $("#WarehouseID").val($("#target option:first").val());
                }
            }
        });
    },
    StateChangeEvent: function () {
        Store.LoadWarehouseByEvent();
    },
    UpdateSenderSuccess: function (avg) {
        alert(avg.Message);
        if (avg.Result > 0) {
            window.location.href = '/store/DeliveryManager';
        }
    }
}
