// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var quill = new Quill('#quillEditor', {
    theme: 'snow'
});

document.querySelector("form").addEventListener("submit", function () {
    //Stuff the data currently inside of the Quill editor inside the hidden input the name="Content"
    document.getElementById("Content").value = quill.container.firstChild.innerHTML;
})
