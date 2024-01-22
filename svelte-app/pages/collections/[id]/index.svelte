<script>
    import { onMount, getContext } from 'svelte';
    import { writable } from 'svelte/store';

    import { navigate } from 'pages/utils';
    import { TermListDisplayNames, TermLists } from 'data/termLists';

    import TermSetMetadataEditor from 'shared/TermSetMetadataEditor.svelte';
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import LabelList from 'shared/LabelList.svelte';
    import TermList from 'shared/TermList.svelte';

    // $: ({id, set} = scoped);
    // let id, set;
    // let id = $params.id;
    // export let props;
    // export let setCtx;
    export let setId;
    export let set;
    export let terms;
    export let labels;
    
    const { open, close } = getContext('simple-modal');
    
    async function openEditSetInfo() {
        console.log(set);
        open(TermSetMetadataEditor, { termSetWritable : set });
    }


</script>

<title>{$set?.name ?? 'Collection'} | MyWords</title>

<div class="d-flex gap-4">
    <div>
        <ApiDependent ready={$set != null}>
            <h2>{ $set.name }</h2>
            <p>{ $set.description }</p>
        </ApiDependent>
    </div>
    <button class="btn align-self-end" aria-label="Edit details" on:click={openEditSetInfo}><i class="bi-pencil" /></button>
    </div>

<hr />

<h4>Labels</h4>
<ApiDependent ready={$labels != null}>
    <LabelList labelsWritable={labels} termSetId={setId} />
</ApiDependent>

<hr />

<h4>Terms</h4>
<div class="flex-1">
    <button class="btn btn-primary" on:click={() => navigate(`/collections/${setId}/addterms`)}><i class="bi-plus-lg" />&ensp;Add terms</button>
</div>



<ApiDependent ready={$terms != null}>
    <div class="row mt-3">
        {#each Object.keys(TermLists) as listName}
            <div class="col-xs-1 col-lg-6 col-xl-4 col-xxl-3">
                <h5>{TermListDisplayNames[TermLists[listName]]}</h5>
                <TermList termsWritable={terms} termSetId={setId} termList={TermLists[listName]} />
            </div>
        {/each}
    </div>
</ApiDependent>