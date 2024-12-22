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

<BackButton text="Back to all collections" href="/collections" />

<div class="card p-3">
    <div class="d-flex gap-4">
        <div style="max-width: 70vw">
            <ApiDependent ready={$collection != null}>
                <h2>{ $collection.name }</h2>
                <p class="text-large-1">{ $collection.description }</p>
            </ApiDependent>
        </div>
        <button class="btn align-self-end" aria-label="Edit details" on:click={openEditCollectionInfo}><i class="bi-pencil" /></button>
    </div>
    <button class="btn btn-primary practice-button text-large-1" on:click={() => navigate(`/collections/${collectionId}/practice`)}>Practice this collection
        <i class="bi bi-chevron-double-right"></i>
    </button>
</div>

<div>
</div>


<div class="card p-3 mt-3">
    <div class="row">
        <div class="col-sm-12 col-md">
            <h3>Labels</h3>
            <ApiDependent ready={$labels != null}>
                <LabelList labelsWritable={labels} collectionId={collectionId} />
            </ApiDependent>
        </div>
        <div class="col-sm-12 col-md">
            <h3>
                Stats
            </h3>
        </div>
    </div>
</div>

<hr />

<div class="card p-3">
    <div class="d-flex mb-3">
        <h3 class="mb-0 me-5">Terms</h3>
    
        <button class="btn btn-primary mb-0" on:click={() => navigate(`/collections/${collectionId}/addterms`)}><i class="bi-plus-lg" />&ensp;Add terms</button>
    </div>
    
    <ApiDependent ready={$terms != null}>
        <div class="row mt-2">
            {#each Object.keys(TermLists) as listName}
                <div class="col-xs-1 col-lg-6 col-xl-4 col-xxl-3 mb-2">
                    <h5>{TermListDisplayNames[TermLists[listName]]}</h5>
                    <TermList termsStore={terms} termList={TermLists[listName]} widthMode={WidthMode.Quarter} dragAndDropEnabled={true}
                        sortFunc={(a, b) => new Date(b.createdUtc) - new Date(a.createdUtc)} />
                </div>
            {/each}
        </div>
    </ApiDependent>
</div>

<style>
    .practice-button {
        height: 7ch;
        width: 25ch;
    }
</style>