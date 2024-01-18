<script>
    import { onMount, setContext } from "svelte";
    import { writable, get } from "svelte/store";
    
    import api from 'services/api';

    export let id;
    let setId;
    let set;
    let terms;
    let labels;
    let props = writable({});

    onMount(async () => {
        setId = id;
        set = writable(await api.get(`termsets/${id}`, 'Failed fetching collection info'));
        terms = writable(await api.get(`termsets/${id}/terms`, 'Failed loading terms'));
        labels = writable(await api.get(`termsets/${id}/labels`, 'Failed loading labels'));
        props.set({setId, set, terms, labels});
    });
</script>

{#key $props}
    <slot props={$props}/>
{/key}