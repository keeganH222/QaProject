var Idarr = new Array();
var NameArr = new Array();
var removeArr = new Array();

var Idarr = new Array();
var NameArr = new Array();
var removeArr = new Array();
function Save(e) {
    var url = $('#btnSave').data('url');
    $('.tags').each(function (e) {
        if ($(this).prop('checked')) {
            var unique = true;
            Idarr.forEach(e => e === $(this).data('id') ? unique = false : 1 + 1);
            if (unique) {
                Idarr.push($(this).data('id'));
                NameArr.push($(this).data('name'));
            }
            let index = removeArr.indexOf($(this).data('name'));
            if (index !== -1) {
                removeArr[index] = removeArr.pop();
            }
        }
        else {
            let index = removeArr.indexOf($(this).data('name'));
            let Nameindex = NameArr.indexOf($(this).data('name'));

            if (index === -1) {
                removeArr.push($(this).data('name'));
            }
            if (Nameindex != -1) {

                if (Nameindex === NameArr.length - 1 && NameArr.length > 2) {
                    NameArr[Nameindex] = NameArr.shift();
                    Idarr[Nameindex] = Idarr.shift();
                }
                else if (NameArr.length === 2) {
                    if (Nameindex === NameArr.length - 1) {
                        NameArr.pop();
                        Idarr.pop();
                    }
                    else {
                        NameArr.shift();
                        Idarr.shift();
                    }

                }
                else {
                    NameArr = new Array();
                    Idarr = new Array();
                }

            }
        }
    });
    var jsonObj = JSON.stringify(Idarr);
    $.ajax({
        type: 'Post',
        url: url,
        contentType: 'application/json',
        data: jsonObj,
        traditional: true
    }).done((result) => {
        Display();
    });
};
function Display() {
    $('#DivClose').click();
    var divDisplay = document.querySelector('#DisplayTags');
    removeArr.forEach((name) => {

        var tags = document.getElementsByClassName(name);
        for (let i = 0; i < tags.length; i++) {

            if (tags[i] !== undefined) {
                divDisplay.removeChild(tags[i]);
            }
        }
    });
    NameArr.forEach((name) => {
        var HaveYouBeenSelected = $("." + name);
        if (!HaveYouBeenSelected.hasClass(name)) {
            var span = document.createElement("span");
            var text = document.createTextNode(name);
            span.append(text);
            $(span).addClass('tagName');
            $(span).addClass(name);
            var button = document.createElement('button');
            var x = document.createTextNode('x');
            button.append(x);
            button.setAttribute('data-name', name);
            button.setAttribute('data-url', 'removeTag');
            $(button).addClass('removeTag');
            span.append(button);
            $('#DisplayTags').append(span);
        }
    });
    $('.removeTag').click(removeTag);
};
function removeTag(e) {
    let TagName = $(this).data('name');
    let url = $(this).data('url');
    let jsonObj = JSON.stringify({ TagName: TagName });
    $.ajax({
        url: url,
        type: 'POST',
        contentType: "application/json",
        data: jsonObj,
        traditional: true
    }).done(() => {
        $('.' + TagName).remove();
    });
};

function changeVoteCount(voteType, itemId, itemType) {
    let elementName = "#vote_count_" + itemType + "_" + itemId;
    let voteElement = $(elementName);
    let voteCnter = parseInt(voteElement.text());
    if (voteType === "UpVote") {
        voteCnter += 1;
    }
    else {
        voteCnter -= 1;
    }
    voteElement.html(voteCnter);
    $("#UpVotequestion" + itemId).addClass("hidden");
    $("#DownVotequestion" + itemId).addClass("hidden");
}
function addComment(e) {
    let counter = $(this).data("count");
    let content = $("#Comment" + counter).val();
    let id = $("#Comment" + counter).data("itemid");
    let url = $("#Comment" + counter).data("url");
    let type = $("#Comment" + counter).data("itemtype")
    let jsonObj = JSON.stringify({ itemId: id, type: type, content: content });
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        data: jsonObj,
        traditional: true
    }).done((htmlResult) => {
        if (type === "question") {
            $(".question_comment").prepend(htmlResult);
        } else {
            $("#answer_comment_" + id).prepend(htmlResult);
        }
        $("#Comment" + counter).val("");
    });
};
function getTextAreaForComment() {
    let answerId = $(this).data("answerid");
    let url = $(this).data("url");
    $(this).addClass("hidden");
    $.get(url).done((result) => {
        $("#addCommentAnswer" + answerId).html(result);
    });
};
function AddTag(tagId, tagName) {
    let existingTag = $("#tagsToAdd" + tagId);
    let tr = undefined;
    if (existingTag.length === 0) {
        tr = $("<tr></tr>");
        let Nametd = $("<td></td>");
        let Checktd = $("<td></td>");
        let span = $("<span></span>").text(tagName);
        let checkBoxInput = $("<input class='tags' type='checkbox' checked/>").attr("Data-id", tagId).attr("Data-name", tagName);
        Nametd.append(span);
        Checktd.append(checkBoxInput);
        tr.append(Nametd).append(Checktd);
    } else {
        inputtag = existingTag.children(".tagInput").children(".tags").prop("checked", true);
        tr = existingTag;
        $("tbody").remove(existingTag)
    }
    $("tbody").prepend(tr);
};
function saveVote() {
    let url = $(this).data("url");
    let itemid = $(this).data("itemid");
    let userid = $(this).data("userid");
    let type = $(this).data("type");
    let jsonObj = { itemType: type, itemId: itemid, userId: userid, };
    $.ajax({
        type: "POST",
        url: url,
        contentType: 'application/json',
        data: JSON.stringify(jsonObj),
        traditional: true
    }).done((resultType) => {
        changeVoteCount(resultType, itemid, type);
    });
};