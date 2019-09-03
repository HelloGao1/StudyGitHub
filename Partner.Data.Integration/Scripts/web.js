var loadingCount = 0;
function showLoading() {
    loadingCount++;
    var loading = $("#loading");
    if (loadingCount > 0 && loading.length === 0) {
        $("body").append('<div id=\"loading\"><span></span></div>');
    }
}
function hideLoading() {
    loadingCount--;
    if (loadingCount <= 0) {
        $("#loading").remove();
    }
}
function showDialog(id) {
    var item = $(id);
    if (item.css('visibility') === 'hidden') {
        item.css('visibility', 'visible');
    }
    if (item.css('display') === 'none') {
        item.css('display', 'block');
    }
}
function closeDialog() {
    $('.win-dialog').each(function (i, n) {
        var item = $(n);
        if (item.css('visibility') === 'visible') {
            item.css('visibility', 'hidden');
        }
        if (item.css('display') === 'block') {
            item.css('display', 'none');
        }
    });
}