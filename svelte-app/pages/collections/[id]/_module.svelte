<script>
    import { onMount, setContext } from "svelte";
    import { writable } from "svelte/store";
    
    import api from 'services/api';

    export let id;
    let collectionId;
    let collection;
    let terms;
    let labels = writable([]);
    setContext('labels', labels);
    let props = writable({});

    onMount(async () => {
        collectionId = id;
        collection = writable(await api.get(`collections/${id}`, 'Failed fetching collection info'));
        terms = writable(await api.get(`collections/${id}/terms`, 'Failed loading terms'));
        labels.set(await api.get(`collections/${id}/labels`, 'Failed loading labels'));
        props.set({collectionId, collection, terms, labels});
    });
</script>

{#key $props}
    <slot props={$props}/>
{/key}