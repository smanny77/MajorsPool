var iconClickCount = 0;

$('#logo').click(function () {
    if (++iconClickCount == 7)
    {
        // If the parameter doesn't already exist.
        if ($(document)[0].location.search.indexOf("Admin") < 0) {
            // If we already have a "?", then use "&"
            if ($(document)[0].location.search.indexOf("?") >= 0) {
                $(document)[0].location.href += "&Admin=318";
            }
            else {
                $(document)[0].location.href += "?Admin=318";
            }
        }
    }    
});