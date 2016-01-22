$(document).ready(function () {

    GetRatings();

    $("ratingTable").DataTable();


    function GetRatings() {

        var url = "/api/rating/getratings";

        $.ajax({
            type: "GET",
            url: url
        })
            .done(function (data) {

                var template = $("#ratings-template").html();
                var info = Mustache.to_html(template, data);
                $("#tablePlaceholder").html(info);

            })
            .fail(function (error) {
                console.log(error);
                //show error
            });
    }





});