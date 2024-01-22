export function hoverNotifier(node) {

    function mouseEnter() {
        if (node) {
            node.dispatchEvent(new CustomEvent("hover_changed", {detail: true}));
        }
    }

    function mouseLeave() {
        if (node) {
            node.dispatchEvent(new CustomEvent("hover_changed", {detail: false}));
        }
    }

    node.addEventListener("mouseenter", mouseEnter);
    node.addEventListener("mouseleave", mouseLeave);

    return {
        destroy() {
            node.removeEventListener("mouseenter", mouseEnter);
            node.removeEventListener("mouseleave", mouseLeave);
        }
    }
}