
function modal(id, position, transform) {
    var box = document.getElementById(id);
    var modal = document.getElementsByClassName('modal');
    modal[position].style.transform = transform;
    box.style.transform = transform;
}

