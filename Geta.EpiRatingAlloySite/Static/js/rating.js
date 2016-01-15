$(function() {

    $("#thankyouMessage").hide();
    $("#commentArea").show();
    var contentId = $("#ratingSection").attr("data-contentId");

    $("#btnYes").click(function() {
        sendRating("", true);
    });

    $("#btnSendRatingComment").click(function() {
        sendRating($("#commentText").val(), false);

    });

    function sendRating(comment, rating) {

        var url = "api/rating/ratepage";

        $.ajax({
                type: "POST",
                url: url,
                data: { comment: comment, rating: rating, contentId: contentId }
            })
            .done(function() {
                $("#thankyouMessage").show();
                $("#commentArea").hide();
            })
            .fail(function(error) {
                console.log(error);
            });
    }

});