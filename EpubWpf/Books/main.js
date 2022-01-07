var t = '';
function gtext(e) {
    t = (document.all) ? document.selection.createRange().text : document.getSelection();
    if (t.length>0) {
        window.external.GetSelected(t);
        $("p:contains(t)").html(function (_, html) {
            return html.replace(/(t)/g, '<span style="background-color:red">$1</span>');
        });
    }
}
document.onmouseup = gtext;
if (!document.all) document.captureEvents(Event.MOUSEUP);