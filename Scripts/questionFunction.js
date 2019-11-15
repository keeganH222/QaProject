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
    } else {
      let index = removeArr.indexOf($(this).data('name'));
      let Nameindex = NameArr.indexOf($(this).data('name'));

      if (index === -1) {
        removeArr.push($(this).data('name'));
      }
      if (Nameindex != -1) {

        if (Nameindex === NameArr.length - 1 && NameArr.length > 2) {
          alert("first test");
          NameArr[Nameindex] = NameArr.shift();
          Idarr[Nameindex] = Idarr.shift();
        } else if (NameArr.length === 2) {
          if (Nameindex === NameArr.length - 1) {
            alert("second test");
            NameArr.pop();
            Idarr.pop();
          } else {
            NameArr.shift();
            Idarr.shift();
          }

        } else {
          alert("third test");
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
      button.setAttribute('data-url', '@Url.Action("removeTag")');
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
  let jsonObj = JSON.stringify({
    TagName: TagName
  });
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