<script>
    import { onMount, setContext } from "svelte";
    import { writable, get } from "svelte/store";
    import api from 'services/api';

    export let id;
    let set;
    let terms;
    let labels;
    $: props = {id: id, set: set, terms: terms, labels: labels};

    onMount(async () => {
        set = (await api.get(`termsets/${id}`, 'Failed fetching collection info'));
        terms = (await api.get(`termsets/${id}/terms`, 'Failed loading terms'));
        labels = (await api.get(`termsets/${id}/labels`, 'Failed loading labels'));
    });
</script>

<slot props={{setCtx: props}} />

{JSON.stringify(props)}