// All functions here return true so you can set like (e) => changed = notBlank(e)
// yes its confusing no i dont care

export function notBlank(event) {
    let input = event.target;
    if (input.value.trim() == '') {
        input.setCustomValidity("Must not be blank")
    }
    else {
        input.setCustomValidity('');
    }
    return true;
}