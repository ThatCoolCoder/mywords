

<script>
    import { getContext, onMount } from 'svelte';
    import { get } from 'svelte/store';

    import { TermListDisplayNames } from 'data/termLists.js';
    
    import LabelCard from 'shared/LabelCard.svelte';
    import EditTermLabels from 'shared/EditTermLabels.svelte';

    const { open, close } = getContext('simple-modal');
    let availableLabels = getContext('labels');
    
    export let term = {value: '', definition: '', termList: 0, notes: '', labels: []};
    export let showTermList = true;
    

    function openEditLabelsModal() {
        open(EditTermLabels, {availableLabels, labels: []});
    }
</script>

<div class="card p-3 d-flex flex-row gap-2">
    <div class="flex-grow-1 d-flex flex-column gap-2">
        <div class="d-flex gap-2 justify-content-start align-items-center">
            {#if showTermList}
                <span class="badge rounded-pill h-auto bg-secondary w-auto">
                    {TermListDisplayNames[term.termList]}
                </span>
                <div class="vr"></div>
            {/if}
            {#each term.labels as label}
                <LabelCard {label} />
            {:else}
                <span class="text-secondary">No labels</span>
            {/each}
            <button class="btn btn-outline-secondary btn-sm mb-0 py-0 px-1" on:click={openEditLabelsModal}><i class="bi-plus-lg"/></button>
        </div>
        <div class="row">
            <div class="col-xs-12 col-md-4 col-xxl-3 form-group">
                <input class="form-control" placeholder="Value" bind:value={term.value} />
            </div>
            <div class="col-xs-12 col-md-4 col-xxl-3 form-group">
                <input class="form-control" placeholder="Definition" bind:value={term.definition} />
            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <textarea bind:value={term.notes} class="form-control mb-0" style="resize: none" placeholder="Notes"></textarea>
            </div>
        </div>
    </div>
    <div class="flex-shrink-1">
        <slot name="right" />
    </div>
</div>