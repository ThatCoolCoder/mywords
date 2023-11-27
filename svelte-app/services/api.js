import { display } from "./errorService";

async function get() {
    display('We were unable to get datas', 'Some very technical reasons yes');
}

export default {
    get
}