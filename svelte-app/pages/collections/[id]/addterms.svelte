<script>
    import { onMount, getContext } from 'svelte';
    import { writable } from 'svelte/store';
    import api from 'services/api.js';
    import ApiDependent from 'shared/misc/ApiDependent.svelte';

    export let scoped;
    $: ({set} = scoped);

    // const { open, close } = getContext('simple-modal');


</script>

<title>Add Terms | MyWords</title>

<ApiDependent ready={set != null}>
    <p>{$set.name}</p>
</ApiDependent>