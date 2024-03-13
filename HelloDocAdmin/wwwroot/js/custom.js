
document.getElementById('thm').style.display = 'none';
document.getElementById('thm2').style.display = 'none';
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
if (currentTheme == 'dark') {
    document.getElementById('thm').style.display = 'block';
    document.getElementById('thm2').style.display = 'none';
}
else {
    document.getElementById('thm2').style.display = 'block';
    document.getElementById('thm').style.display = 'none';
}
function mode() {

   var body = document.body;
  // var currentTheme = body.getAttribute('data-bs-theme');
    var currentTheme = localStorage.getItem("Theme");
    
  // Toggle between 'light' and 'dark'
  var newTheme = currentTheme === 'light' ? 'dark' : 'light';
    if (newTheme == 'dark') {
        document.getElementById('thm').style.display = 'block';
        document.getElementById('thm2').style.display = 'none';
    }
    else {
        document.getElementById('thm2').style.display = 'block';
        document.getElementById('thm').style.display = 'none';
    }
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
$(document).ready(function () {
    $('.btn-upl1').on('click', function () {
        $('.file1').trigger('click');
    });

    $('.file1').on('change', function () {
        var fileName = $(this)[0].files[0].name;
        $('#file-name1').val(fileName);
    });
})
$(document).ready(function () {
    $('.btn-upl2').on('click', function () {
        $('.file2').trigger('click');
    });

    $('.file2').on('change', function () {
        var fileName = $(this)[0].files[0].name;
        $('#file-name2').val(fileName);
    });
})