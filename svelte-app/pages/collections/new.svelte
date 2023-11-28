<script>
    import { onMount } from 'svelte';
    import api from '../../services/api.js';

    async function create() {
        await api.post('termsets/', {name: 'My new set', description: 'A very amazing set'}, 'Failed creating new set');
    }

</script>

<title>Create Collection | MyWords</title>

<h2>Your collections</h2>
<hr />

<div>
    <button class="btn btn-primary" on:click={create}>Add</button>
</div>