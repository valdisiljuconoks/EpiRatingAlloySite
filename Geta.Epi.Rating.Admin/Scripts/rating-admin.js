$(function () {

    GetRatings();

    $("#btnFilter").on("click", function () {
        GetRatings();
    });


    function GetRatings(filterParams) {

        var url = "/api/rating/getratings";
        var urlEnableRating = "/api/rating/enablerating";

        var dateFrom = $("#txtFrom").val();
        var dateTo = $("#txtTo").val();

        if (dateFrom != null || dateTo != null) {
            url = url + "?" + $.param({ dateFrom: dateFrom, dateTo: dateTo });
        }

        $.ajax({
            type: "GET",
            url: url
        })
            .done(function (data) {

                $.each(data.ratingData, function (i, obj) {

                    $.extend(true, obj, {
                        checked: obj.ratingEnabled ? "checked" : "",
                        selectedyes: obj.ratingEnabled ? "selected" : "",
                        selectedno: !obj.ratingEnabled ? "selected" : ""
                    });
                });

                var template = $("#ratings-template").html();
                var info = Mustache.to_html(template, data);
                $("#tablePlaceholder").html(info);
                $("#ratingTable").DataTable({
                    dom: 'Bfrtip',
                    buttons: [
                            {
                                extend: 'excelHtml5',
                                text: 'Export',
                                filename: "PageRatingData"
                            }
                    ]
                });

                $("input[name=ratingSwitch]").each(function (i, obj) {

                    $(obj).on("click", function (e) {

                        var contentId = $(obj).attr("data-contentId");

                        if (confirm("By changing this value for this page it will be published. Do you want to proceed ?") == true) {

                            $.ajax({
                                type: "POST",
                                url: urlEnableRating,
                                data: { contentId: contentId, ratingEnabled: this.checked }
                            })
                            .done(function (data) {

                            })
                            .fail(function (error) {
                                console.log(error);
                            });
                        }
                        else
                        {
                            e.preventDefault();
                            return false;
                        }

                    });
                });
            })
            .fail(function (error) {
                console.log(error);
                //show error
            });
    }

});