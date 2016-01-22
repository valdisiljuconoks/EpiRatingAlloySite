$(function() {

    $("#thankyouMessage").hide();

    var contentId = $("#ratingSection").attr("data-contentId");
    var cookie = getCookie("IsRated_" + contentId);

    if (cookie != null) {
        $("#ratingSection").hide();
        return;
    }

    $("#commentArea").show();
    

    $("#btnYes").click(function() {
        sendRating("", true);
    });

    $("#btnSendRatingComment").click(function() {
        sendRating($("#commentText").val(), false);
    });

    function sendRating(comment, rating) {

        var url = "/api/rating/ratepage";

        $.ajax({
                type: "POST",
                url: url,
                data: { comment: comment, rating: rating, contentId: contentId }
            })
            .done(function() {
                $("#thankyouMessage").show();
                $("#ratingSection").hide();
            })
            .fail(function(error) {
                console.log(error);
                //show error
            });
    }

    function getCookie(name) {
        var re = new RegExp(name + "=([^;]+)");
        var value = re.exec(document.cookie);
        return (value != null) ? unescape(value[1]) : null;
    }
});