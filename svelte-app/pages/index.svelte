<script>
    import { onMount, getContext } from 'svelte';
    
    import api from 'services/api.js';
    import { navigate } from './utils.js';
    
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import CollectionCard from "shared/CollectionCard.svelte";

    const user = getContext('user');

    let collections = null;
    onMount(async () => collections = await api.get('collections/recent?amount=3', 'Failed getting collections'));

</script>

<title>Home | MyWords</title>

<ApiDependent ready={user != undefined}>
    <h2 class="mb-4">Welcome back, {$user?.firstName}</h2>
</ApiDependent>

<!-- <h3>Recent collections</h3>
<hr /> -->

<div class="d-flex align-items-end">
    <h3 class="mb-0">Recent collections</h3>
    
    <a class="btn text-secondary p-0 ms-5 text-large-1 mb-0" href="/collections/">
        View all <i class="bi-chevron-right"></i>
    </a>
</div>

<hr class="mt-2" />

<div class="row g-3">
    <ApiDependent ready={collections != null}>
        {#if collections.length > 0}
            {#each collections as set}
                <CollectionCard click={() => navigate(`/collections/${set.id}`)}>
                    <h5>{ set.name }</h5>
                    <p>{ set.description }</p>
                </CollectionCard>
            {/each}
        {:else}
            <CollectionCard click={() => navigate('/collections/new')}>
                <h5>You have no collections</h5>
                <p>Click to create your first one</p>
            </CollectionCard>
        {/if}

    </ApiDependent>
</div>