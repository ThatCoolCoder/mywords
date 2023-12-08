<script>
    import { onMount, getContext } from 'svelte';
    import { writable } from 'svelte/store';
    import api from 'services/api.js';
    import { navigate } from 'pages/utils';
    import TermSetMetadataEditor from 'shared/TermSetMetadataEditor.svelte';
    import ApiDependent from 'shared/ApiDependent.svelte';
    import LabelCard from 'shared/LabelCard.svelte';

    export let scoped;
    $: ({id, set} = scoped);
    let terms;
    let labels;

    const { open, close } = getContext('simple-modal');

    async function openEditSetInfo() {
        open(TermSetMetadataEditor, { termSetWritable : set });
    }

    function addLabel() {
        var l = {name: '', color: ''};
        labels.update(x => x.concat([l]));
    }

    onMount(async () => {
        terms = writable(await api.get(`termsets/${id}/terms`));
        labels = writable(await api.get(`termsets/${id}/labels`));
    });

</script>

<title>{$set?.name ?? 'Collection'} | MyWords</title>

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

<button class="btn btn-primary" on:click={() => navigate(`/collections/${id}/addterms`)}><i class="bi-plus-lg" />&ensp;Add terms</button>

<hr />

<h4>Labels</h4>
<ApiDependent ready={labels != null}>
    {#each $labels as label}
        <LabelCard label={writable(label)} />
    {:else}
        <span>This collection doesn't have any labels yet</span><br />
    {/each}
</ApiDependent>

<button class="btn" aria-label="add label" on:click={addLabel}><i class="bi-plus-lg"/></button>

<!-- <div>
    <button class="btn btn-primary" on:click={create}>Add</button>
</div> -->