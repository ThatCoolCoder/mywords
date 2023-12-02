<script>
    import { onMount } from 'svelte';
    
    import api from 'services/api.js';
    import { navigate } from './utils.js';
    
    import ApiDependent from 'shared/ApiDependent.svelte';
    import TermSetCard from "shared/TermSetCard.svelte";

    let termSets = null;
    onMount(async () => termSets = await api.get('termsets', 'Failed getting collections'));

</script>

<title>Home | MyWords</title>

<h2>Your collections</h2>
<hr />

<div class="row gap-2">
    <ApiDependent ready={termSets != null}>
        {#if termSets.length > 0}
            {#each termSets as set}
                <TermSetCard click={() => navigate(`/collections/${set.id}`)}>
                    <h5>{ set.name }</h5>
                    <p>{ set.description }</p>
                </TermSetCard>
            {/each}
            <TermSetCard click={() => navigate('/collections/new')}>
                <div class="text-center">
                    <h1 class="mb-0"><i class="bi-plus-lg" /></h1>
                    <p>Create a new collection</p>
                </div>
            </TermSetCard>
        {:else}
            <TermSetCard click={() => navigate('/collections/new')}>
                <h5>You have no collections</h5>
                <p>Click to create your first one</p>
            </TermSetCard>
        {/if}

    </ApiDependent>
</div>