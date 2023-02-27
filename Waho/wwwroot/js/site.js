// success, info, warning, error
function createNotify(status, title, text, autoClose) {
    new Notify({
        status: status,
        title: title,
        text: text,
        effect: 'slide',
        speed: 300,
        customClass: '',
        customIcon: '',
        showIcon: true,
        showCloseButton: true,
        autoclose: autoClose,
        autotimeout: 5000,
        gap: 10,
        distance: 20,
        type: 1,
        position: 'right bottom',
        customWrapper: ''
    })
}
