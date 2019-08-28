var availableState = [
    "Alabama",
    "Alaska",
    "Arizona",
    "Arkansas",
    "California",
    "Colorado", "Connecticut", "Delaware", "Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New", "Hampshire", "New", "Jersey", "New", "Mexico", "New", "York", "North", "Carolina", "North", "Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode", "Island", "South", "Carolina", "South", "Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", "Washington", "West", "Virginia", "Wisconsin", "Wyoming", "Washington, D.C"
];

var WareHouse = {
    Innit: function () {
        WareHouse.ValidateForm();
    },
    ValidateForm: function () {
        $("#frm_create_warehouse").validate({
            rules: {
                "FullName": "required",
                "Warehouse": "required",
                "Phone": {
                    required: true,
                    IsPhoneUS: true
                },
                "Email": {
                    required: true,
                    email: true,
                    IsNotExistEmailWarehouse: true
                },
                "Add1": "required",
                "CityId": "required",
                "StateID": "required",
                "Zip": {
                    required: true,
                    maxlength: 5,
                    minlength: 5
                }

            },
            messages: {
                "FullName": "Please enter your full name",
                "Warehouse": "Please enter your ware house",
                "Email": {
                    required: "Please enter a valid email address",
                    email: "Email is wrong format"
                },
                "Add1": "Please enter Address",
                "CityId": "Please select city!",
                "StateID": "Please select an State",
                "Phone": {
                    required: "Please enter your phone"

                },
                "Zip": {
                    required: "Please enter zip code",
                    maxlength: "Zip code only 5 digit",
                    minlength: "Zip code must 5 digit"
                }
            }
        });
    }
}