<script>
    import api from 'services/api.js';

    async function create() {
        await api.post('collections/', {name: 'My new collection', description: 'A very amazing collection'}, 'Failed creating new collection');
    }

</script>

<title>Create Collection | MyWords</title>

<h2>Your collections</h2>
<hr />

<div>
    <button class="btn btn-primary" on:click={create}>Add</button>
</div>