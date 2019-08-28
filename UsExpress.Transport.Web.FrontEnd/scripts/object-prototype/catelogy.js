var Catelogy = {
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
                "CategoryName": "Please enter catelogy name",
                "Code": {

                    required: "Please enter your code",
                    IsNotExistCatelogyCode: "Code had exist, please select another code"
                }
            }
        });
    }
} 