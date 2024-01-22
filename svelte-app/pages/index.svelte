<script>
    import { onMount } from 'svelte';
    
    import api from 'services/api.js';
    import { navigate } from './utils.js';
    
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import CollectionCard from "shared/CollectionCard.svelte";

    let collections = null;
    onMount(async () => collections = await api.get('collections', 'Failed getting collections'));

</script>

<title>Home | MyWords</title>

<h2>Your collections</h2>
<hr />

<div class="d-flex gap-2">
    <ApiDependent ready={collections != null}>
        {#if collections.length > 0}
            {#each collections as set}
                <CollectionCard click={() => navigate(`/collections/${set.id}`)}>
                    <h5>{ set.name }</h5>
                    <p>{ set.description }</p>
                </CollectionCard>
            {/each}
            <CollectionCard click={() => navigate('/collections/new')}>
                <div class="text-center">
                    <h1 class="mb-0"><i class="bi-plus-lg" /></h1>
                    <p>Create a new collection</p>
                </div>
            </CollectionCard>
        {:else}
            <CollectionCard click={() => navigate('/collections/new')}>
                <h5>You have no collections</h5>
                <p>Click to create your first one</p>
            </CollectionCard>
        {/if}

    </ApiDependent>
</div>