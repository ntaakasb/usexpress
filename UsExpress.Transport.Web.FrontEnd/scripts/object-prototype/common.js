var InitControl = {
    Pagination: function (_this, ajaxCall) {
        var divPaging = $(_this).attr("data-paging");
        var totalRows = $(_this).attr("data-totalrows");
        var intPageSize = $(_this).attr("data-pagesize") || pagesize;

        $(divPaging).pagination({
            items: totalRows,
            itemsOnPage: intPageSize,
            displayedPages: 3,
            prevText: "«",
            nextText: "»",
            cssStyle: "light-theme",
            onPageClick: function (pageNumber) {
                ajaxCall(pageNumber);
            }
        });
        if (parseInt(totalRows) <= parseInt(intPageSize)) {
            $(divPaging).hide();
        } else {
            $(divPaging).show();
        }
    }
}