<script>
    import { onMount, getContext } from 'svelte';
    
    import api from 'services/api.js';
    import { navigate } from 'pages/utils.js';
    
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import CollectionCard from "shared/CollectionCard.svelte";
    import BackButton from "shared/misc/BackButton.svelte";
    import { writable } from 'svelte/store';
    import CollectionMetadataEditor from 'shared/CollectionMetadataEditor.svelte';

    const user = getContext('user');
    const { open, close } = getContext('simple-modal');

    let collections = null;
    let hovering = {};
    onMount(async () => collections = await api.get('collections/', 'Failed getting collections'));

    async function manageCollection(e, collection) {
        e.stopPropagation();

        open(CollectionMetadataEditor, {collectionWritable: writable(collection)})
    }

</script>

<title> Collections | MyWords </title>


<BackButton text="Back to homepage" href="/" />
    
<h2>My Collections</h2>

<hr class="mt-2" />

<ApiDependent ready={collections != null}>
    {#if collections.length > 0}
        <div class="row g-3 mb-5">
            <CollectionCard click={() => navigate('/collections/new')}>
                <div class="text-center">
                    <h1 class="mb-0"><i class="bi-plus-lg" /></h1>
                    <p>Create a new collection</p>
                </div>
            </CollectionCard>
        </div>
        <div class="row g-3">
            {#each collections as collection}
                <CollectionCard click={() => navigate(`/collections/${collection.id}`)} onHoverChanged={(val) => hovering[collection.id] = val.detail}>
                    <div>
                        <h5>{ collection.name }</h5>
                        <p>{ collection.description }</p>
                    </div>
                    <div class="d-flex justify-content-end align-items-center gap-2 mt-auto">
                        <span class="text-secondary">Viewed { new Date(collection.viewedTimeUtc).toLocaleDateString() }</span>
                        <button class="btn btn-outline-secondary mb-0 hidable-button" data-visible={hovering[collection.id]} on:click={(e) => manageCollection(e, collection)}>Manage</button>
                    </div>
                </CollectionCard>
            {/each}
        </div>
    {:else}
        <CollectionCard click={() => navigate('/collections/new')}>
            <h5>You have no collections</h5>
            <p>Click to create your first one</p>
        </CollectionCard>
    {/if}
</ApiDependent>