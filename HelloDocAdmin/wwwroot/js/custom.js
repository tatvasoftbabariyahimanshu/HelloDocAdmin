

function Toggle() {
  let temp = document.getElementById("pas1");
   
  if (temp.type === "password") {
      temp.type = "text";
  }
  else {
      temp.type = "password";
  }
}
function Toggle_new1() {
    let temp = document.getElementById("pas2");

    if (temp.type === "password") {
        temp.type = "text";
    }
    else {
        temp.type = "password";
    }
}
function Toggle_new2() {
    let temp = document.getElementById("pas3");

    if (temp.type === "password") {
        temp.type = "text";
    }
    else {
        temp.type = "password";
    }
}
var body = document.body;
var currentTheme=localStorage.getItem("Theme");
body.setAttribute('data-bs-theme', currentTheme);
function mode() {

   var body = document.body;
  // var currentTheme = body.getAttribute('data-bs-theme');
  var currentTheme=localStorage.getItem("Theme");

  // Toggle between 'light' and 'dark'
  var newTheme = currentTheme === 'light' ? 'dark' : 'light';

  // Update the data-bs-theme attribute
  body.setAttribute('data-bs-theme', newTheme);
  localStorage.setItem("Theme",newTheme);
  
  console.log(body.getAttribute('data-bs-theme'));
}
$(document).ready(function () {
  $('.btn-upl').on('click', function () {
    $('.file').trigger('click');
  });
  
  $('.file').on('change', function () {
    var fileName = $(this)[0].files[0].name;
    $('#file-name').val(fileName);
  });
})