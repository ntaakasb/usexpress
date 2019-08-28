$(function () {
    jQuery.validator.addMethod('IsPhoneUS', function (value, element) {
        return /\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})/.test(value); 
    }, 'Phone is wrong format');

    jQuery.validator.addMethod('IsNotExistEmail', function (value, element) {
        var IsNotExist = false;
        var id = $('#hId').length > 0 ? $('#hId').val() : -1;    
        //if update id > 0 and email == this email => validate return true
        $.ajax({
            url: '/StoreNoLogin/IsExistsUserName',
            global: false,
            type: 'POST',
            data: { UserName: value, id: id},
            async: false, //blocks window close
            success: function (result) {
                IsNotExist = result == true ? false : true; //if had exists validate is false
            }
        });
        return IsNotExist;
    }, 'Email had already. Please select another email');

    jQuery.validator.addMethod('SelectRoleUser', function (value, element) {
        var id = $('#hId').length > 0 ? $('#hId').val() : -1;    
        if (id > 0) {
            return value.length > 0;
        }
        else {
            return true;
        }
    }, 'Please select role user');

    jQuery.validator.addMethod('SelectWarehouse', function (value, element) {

        var id = $('#hId').length > 0 ? $('#hId').val() : -1;
        if (id > 0) {
            return value.length > 0;
        }
        else {
            return true;
        }
    }, 'Please select ware house');


    jQuery.validator.addMethod('IsNotExistCatelogyCode', function (value, element) {
        var IsNotExist = false;
        var id = $('#hCatelogyID').val() != undefined ? $('#hCatelogyID').val() : -1;
        $.ajax({
            url: '/Category/CheckExistCode',
            global: false,
            type: 'POST',
            data: { id: id, code: value },
            async: false, //blocks window close
            success: function (result) {
                IsNotExist = result == true ? false : true; //if had exists validate is false
            }
        });
        return IsNotExist;
    }, 'Code had already. Please select another code');

    jQuery.validator.addMethod('IsNotExistCode', function (value, element) {
        var IsNotExist = false;
        var id = element.getAttribute('data-id') == null ? -1 : element.getAttribute('data-id') ;
        $.ajax({
            url: '/Store/IsExistStoreCode',
            global: false,
            type: 'POST',
            data: { storeID: id, code: value },
            async: false, //blocks window close
            success: function (result) {
                IsNotExist = result == true ? false : true; //if had exists validate is false
            }
        });
        return IsNotExist;
    }, 'Code had already. Please select another code');

    jQuery.validator.addMethod('IsNotExistEmailWarehouse', function (value, element) {
        var IsNotExist = false;
        var id = $('#hId').length > 0 ? $('#hId').val() : -1;
        //if update id > 0 and email == this email => validate return true
        $.ajax({
            url: '/Warehouse/IsExistsEmailUpdate',
            global: false,
            type: 'POST',
            data: { Email: value, id: id },
            async: false, //blocks window close
            success: function (result) {
                IsNotExist = result == true ? false : true; //if had exists validate is false
            }
        });
        return IsNotExist;
    }, 'Email had already. Please select another email');
});