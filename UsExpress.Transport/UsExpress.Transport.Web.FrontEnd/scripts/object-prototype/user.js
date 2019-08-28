var User = {
    Innit: function () {
        User.ValidateForm();
    },
    ValidateForm: function () {
        $("#form-user").validate({
            rules: {
                "FullName": "required",
                "Add1": "required",
                "Phone": {
                    required: true
                },
                "Email": {
                    required: true,
                    email: true,
                    IsNotExistEmail: true
                },
                "Password": "required",
                "StateID": "required",
                "WarehouseID": {
                    required: function (element) {
                        return $("select[name=slRole] option:selected").val() == 6;
                    }
                },
                "StoreID": {
                    required: function (element) {
                        return $("select[name=slRole] option:selected").val() == 3;
                    }
                }

                
            },
            messages: {
                "FullName": "Fullname required!",
                "Add1": "Addredd required!",
                //"Email": "Please enter a valid email address",
                "CityId": "City required!",
                "StateID": "State required!",
                "Phone": {
                    required: "Phone number required"
                },
                "WarehouseID": {
                    required: "Warehouse required"
                },
                "StoreID": {
                    required: "Store required"
                }

            }
        });
    }
}

function ChangeRole(_this) {
    if (_this.value == 6) {
        $('#WarehouseID').parent().show();
    }
    else if (_this.value == 3) {
        $('#StoreID').parent().show();
    }
    else {
        $('#WarehouseID').parent().hide();
        $('#StoreID').parent().hide();
    }
}