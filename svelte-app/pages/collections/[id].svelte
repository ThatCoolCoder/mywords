<script>
    import { onMount } from 'svelte';
    import api from '../../services/api.js';

    export let id;
    let set;
    let terms;

    onMount(async () => {
        set = await api.get(`termsets/${id}`);
        terms = await api.get(`termsets/${id}/terms`);
    });

</script>

<title>{set?.name ?? 'Collection'} | MyWords</title>

<h2>{set?.name ?? ''}</h2>
<hr />

<h4>Terms</h4>

<!-- <div>
    <button class="btn btn-primary" on:click={create}>Add</button>
</div> -->