<script>
    import { getContext } from 'svelte';

    import { navigate } from 'pages/utils';
    import { TermListDisplayNames, TermLists } from 'data/termLists';

    import CollectionMetadataEditor from 'shared/CollectionMetadataEditor.svelte';
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import LabelList from 'shared/LabelList.svelte';
    import TermList from 'shared/TermList.svelte';
    import { WidthMode } from 'shared/TermCard.svelte';
    import BackButton from 'shared/misc/BackButton.svelte';

    export let collectionId;
    export let collection;
    export let terms;
    export let labels;
    
    const { open, close } = getContext('simple-modal');
    
    async function openEditCollectionInfo() {
        open(CollectionMetadataEditor, { collectionWritable : collection });
    }


</script>

<title>{$collection?.name ?? 'Collection'} | MyWords</title>

<BackButton text="Back to collections" href="/collections" />

<div class="d-flex gap-4">
    <div>
        <ApiDependent ready={$collection != null}>
            <h2>{ $collection.name }</h2>
            <p class="text-large-1">{ $collection.description }</p>
        </ApiDependent>
    </div>
    <button class="btn align-self-end" aria-label="Edit details" on:click={openEditCollectionInfo}><i class="bi-pencil" /></button>
</div>

<button class="btn btn-primary" on:click={() => navigate(`/collections/${collectionId}/practice`)}>Lazy button for practice</button>

<hr />

<h3>Labels</h3>
<ApiDependent ready={$labels != null}>
    <LabelList labelsWritable={labels} collectionId={collectionId} />
</ApiDependent>

<div class="mb-5"></div>

<div class="d-flex">
    <h3 class="mb-0 me-5">Terms</h3>

    <button class="btn btn-primary mb-0" on:click={() => navigate(`/collections/${collectionId}/addterms`)}><i class="bi-plus-lg" />&ensp;Add terms</button>
</div>
<hr />

<ApiDependent ready={$terms != null}>
    <div class="row mt-2">
        {#each Object.keys(TermLists) as listName}
            <div class="col-xs-1 col-lg-6 col-xl-4 col-xxl-3 mb-2">
                <h5>{TermListDisplayNames[TermLists[listName]]}</h5>
                <TermList termsStore={terms} termList={TermLists[listName]} widthMode={WidthMode.Quarter} dragAndDropEnabled={true}/>
            </div>
        {/each}
    </div>
</ApiDependent>