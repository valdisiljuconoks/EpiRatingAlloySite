$(function () {

    $('#loading-image').hide();

    if ($("#ratings-template").length > 0) {
        GetRatings();
    }

    $("#btnFilter").on("click", function() {
        GetRatings();
    });

    if ($("#comments-template").length > 0) {

        var contentId = $("#comments-template").attr("data-contentId");
        GetPageComments(contentId);
    }


    function GetRatings(filterParams) {

        var url = "/api/rating/getratings";

        var dateFrom = $("#txtFrom").val();
        var dateTo = $("#txtTo").val();

        if (dateFrom != null || dateTo != null) {
            url = url + "?" + $.param({ dateFrom: dateFrom, dateTo: dateTo });
        }

        $('#loading-image').show();
        $.ajax({
                type: "GET",
                url: url
            })
            .done(function(data) {

                $.each(data.ratingData, function(i, obj) {

                    $.extend(true, obj, {
                        checked: obj.ratingEnabled ? "checked" : ""
                    });
                });

                var template = $("#ratings-template").html();
                var info = Mustache.to_html(template, data);
                $("#tablePlaceholder").html(info);
                $("#ratingTable").DataTable({
                    dom: '<B><f>rtip',
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            text: 'Export',
                            filename: "PageRatingData",
                            className: "data-table-button"
                        }
                    ]
                });
            })
            .fail(function(error) {
                console.log(error);
                //show error
            })
            .always(function() {
                $('#loading-image').hide();
            });
    }

    function GetPageComments(contentId) {

        $('#loading-image').show();
        $.ajax({
            type: "GET",
            url: "/api/rating/getpagecomments" + "?contentId=" + contentId
    })
            .done(function(data) {

                var template = $("#comments-template").html();
                var info = Mustache.to_html(template, data);
                $("#tablePlaceholder").html(info);
                if (data.ratingData.length) {
                    $("#pageTitle").text(data.ratingData[0].pageName);
                }
            })
            .fail(function(error) {
                console.log(error);
                //show error
            })
            .always(function () {
                $('#loading-image').hide();
            });;
    }
});

function enableRating(element, e) {

    var urlEnableRating = "/api/rating/enablerating";
    var contentId = $(element).attr("data-contentId");

    if (confirm("By changing this value for this page it will be published. Do you want to proceed ?") === true) {

        $.ajax({
            type: "POST",
            url: urlEnableRating,
            data: { contentId: contentId, ratingEnabled: element.checked }
        })
            .done(function (data) {

            })
            .fail(function (error) {
                console.log(error);
            });
    } else {
        e.preventDefault();
        return false;
    }
    return true;
}