function tableDynamic (id,rowString,indexInit) {
    var i = parseInt(indexInit);
    $("#add_row" + id).click(function () {
        $('#'+id).append('<tr id="r' +id+ (i) + '"></tr>');
        $('#r'+id + i).html(String.format(rowString, i + 1, i));
        //$('#r' + i).html(String.format("<td>{0}</td><td><input name='name{1}' type='text' placeholder='url' class='form-control input-md'  /> </td>",i+1,i));
        i++;
    });
    $("#delete_row" + id).click(function () {
        if (i >0) {
            $("#r"+id + (i-1)).remove();
            i--;
        }
    }); 
};
String.format = function () {
    var s = arguments[0];
    if (s == null) return "";
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = getStringFormatPlaceHolderRegEx(i);
        s = s.replace(reg, (arguments[i + 1] == null ? "" : arguments[i + 1]));
    }
    return cleanStringFormatResult(s);
}
String.prototype.format = function () {
    var txt = this.toString();
    for (var i = 0; i < arguments.length; i++) {
        var exp = getStringFormatPlaceHolderRegEx(i);
        txt = txt.replace(exp, (arguments[i] == null ? "" : arguments[i]));
    }
    return cleanStringFormatResult(txt);
}
function getStringFormatPlaceHolderRegEx(placeHolderIndex) {
    return new RegExp('({)?\\{' + placeHolderIndex + '\\}(?!})', 'gm')
}
function cleanStringFormatResult(txt) {
    if (txt == null) return "";
    return txt.replace(getStringFormatPlaceHolderRegEx("\\d+"), "");
}