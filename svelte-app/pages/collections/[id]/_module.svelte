<script>
    import { onMount, setContext } from "svelte";
    import { writable } from "svelte/store";
    import api from 'services/api';

    export let id;
    let set;

    onMount(async () => {
        set = writable(await api.get(`termsets/${id}`, 'Failed fetching collection info'));
        setContext('scopedTerm', {id, set});
    });
</script>

<slot />