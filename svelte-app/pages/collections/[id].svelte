<script>
    import { onMount, getContext } from 'svelte';
    import api from '../../services/api.js';
    import TermSetMetadataEditor from '../../shared/TermSetMetadataEditor.svelte';
    import { writable } from 'svelte/store';
    import ApiDependent from '../../shared/ApiDependent.svelte';

    export let id;
    let set;
    let terms;
    let labels;

    const { open, close } = getContext('simple-modal');

    async function openEditSetInfo() {
        open(TermSetMetadataEditor, { termSetWritable : set });
    }

    onMount(async () => {
        set = writable(await api.get(`termsets/${id}`));
        terms = writable(await api.get(`termsets/${id}/terms`));
        labels = writable(await api.get(`termsets/${id}/labels`));
    });

</script>

<title>{set?.name ?? 'Collection'} | MyWords</title>

<div class="d-flex gap-4">
    <div>
        <ApiDependent ready={set != null}>
            <h2>{ $set.name }</h2>
            <p>{ $set.description }</p>
        </ApiDependent>
    </div>
    <button class="btn align-self-end" aria-label="Edit details" on:click={openEditSetInfo}><i class="bi-pencil" /></button>
    </div>
<hr />

<h4>Terms</h4>

<hr />

<h4>Labels</h4>

<!-- <div>
    <button class="btn btn-primary" on:click={create}>Add</button>
</div> -->