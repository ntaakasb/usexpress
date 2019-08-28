var Category = {
    ValidateInsertForm: function () {
        $("#frm_create_catelogy").validate({
            rules: {
                "CategoryName": "required",
                "Code": {
                    required: true,
                    IsNotExistCatelogyCode: true
                }
            },
            messages: {
                "CategoryName": "Catelogy name required",
                "Code": {

                    required: "Catelogy code required",
                    IsNotExistCatelogyCode: "Code had exist, please select another code!"
                }
            }
        });
    }
} 