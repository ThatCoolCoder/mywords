export function enterNotifier(node) {

    function keyPress(event) {
        if (node && event.code == "Enter") {
            node.dispatchEvent(new CustomEvent("enter_pressed"));
        }
    }

    node.addEventListener("keypress", keyPress);

    return {
        destroy() {
            node.removeEventListener("keypress", keyPress);
        }
    }
}