<script>
    import { onMount, getContext } from 'svelte';
    
    import api from 'services/api.js';
    import { navigate } from 'pages/utils.js';
    
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import CollectionCard from "shared/CollectionCard.svelte";
    import BackButton from "shared/misc/BackButton.svelte";

    const user = getContext('user');

    let collections = null;
    onMount(async () => collections = await api.get('collections/', 'Failed getting collections'));

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
            {#each collections as set}
                <CollectionCard click={() => navigate(`/collections/${set.id}`)}>
                    <h5>{ set.name }</h5>
                    <p>{ set.description }</p>
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