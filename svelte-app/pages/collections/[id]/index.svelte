<script>
    import { onMount, getContext } from 'svelte';
    import { writable } from 'svelte/store';
    import { params } from '@roxi/routify';

    import api from 'services/api.js';
    import { navigate } from 'pages/utils';

    import TermSetMetadataEditor from 'shared/TermSetMetadataEditor.svelte';
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import LabelCard from 'shared/LabelCard.svelte';
    import LabelList from 'shared/LabelList.svelte';

    // $: ({id, set} = scoped);
    // let id, set;
    // let id = $params.id;
    // export let props;
    export let setCtx;
    
    const { open, close } = getContext('simple-modal');
    
    async function openEditSetInfo() {
        console.log(setCtx);
        open(TermSetMetadataEditor, { termSetWritable : setCtx.set });
    }


</script>

<title>{setCtx?.set?.name ?? 'Collection'} | MyWords</title>

<div class="d-flex gap-4">
    <div>
        <ApiDependent ready={setCtx?.set != null}>
            <h2>{ setCtx.set.name }</h2>
            <p>{ setCtx.set.description }</p>
        </ApiDependent>
    </div>
    <button class="btn align-self-end" aria-label="Edit details" on:click={openEditSetInfo}><i class="bi-pencil" /></button>
    </div>
<hr />

<h4>Terms</h4>

<button class="btn btn-primary" on:click={() => navigate(`/collections/${setCtx.id}/addterms`)}><i class="bi-plus-lg" />&ensp;Add terms</button>

<hr />

<h4>Labels</h4>
<ApiDependent ready={setCtx?.labels != null}>
    <LabelList labels={setCtx.labels} termSetId={setCtx.id} />
</ApiDependent>


<!-- <div>
    <button class="btn btn-primary" on:click={create}>Add</button>
</div> -->