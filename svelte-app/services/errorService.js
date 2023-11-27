import { writable } from 'svelte/store';

var _handler = (message, details) => alert(message + '\n' + details);

export const handler = writable(_handler);
handler.subscribe(val => {
    _handler = val;
});

export function display(message, details) {
    _handler(message, details);
}
