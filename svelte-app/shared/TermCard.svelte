

<script>
    import { getContext, onMount } from 'svelte';
    import { writable } from 'svelte/store';

    import { TermListDisplayNames } from 'data/termLists.js';
    import api from 'services/api';
    
    import LabelBadge from 'shared/LabelBadge.svelte';
    import EditTermLabels from 'shared/EditTermLabels.svelte';

    const { open, close } = getContext('simple-modal');
    let availableLabels = getContext('labels');
    
    export let term = {value: '', definition: '', termList: 0, notes: '', labels: []};
    export let showTermList = true; // show what term list it's in
    export let showLabels = true;
    export let syncWithApi = false; // false by default because it's a little dangerous

    let sortedTermLabels;
    $: sortedTermLabels = term.labels.map(labelId => $availableLabels.filter(x => x.id == labelId)[0]).toSorted((a, b) => a.name.compareTo(b.name));
    
    function openEditLabelsModal() {
        let w = writable(term.labels);
        w.subscribe(newVal => term.labels = newVal);
        open(EditTermLabels, {availableLabels, labels: w});
    }

    function update() {
        if (syncWithApi) {
            api.put(`terms/${term.id}`, term);
        }
    }
</script>

<div class="card p-2 d-flex flex-row gap-2">
    <div class="flex-grow-1 d-flex flex-column gap-2">
        <div class="row">
            <div class="order-2 col-xs-12 col-md-4 col-xxl-3 form-group">
                <input class="form-control mb-0" placeholder="Value" bind:value={term.value} />
            </div>
            <div class="order-3 col-xs-12 col-md-4 col-xxl-3 form-group">
                <input class="form-control mb-0" placeholder="Definition" bind:value={term.definition} />
            </div>

            
            {#if showTermList || showLabels}
                <div class="col-xl-12 order-xl-1 order-xxl-5 col-xxl-3 d-flex gap-2 justify-content-start align-items-center mb-2">
                    {#if showTermList}
                        <span class="badge rounded-pill h-auto bg-secondary w-auto">
                            {TermListDisplayNames[term.termList]}
                        </span>
                        <div class="vr"></div>
                    {/if}
                    {#if showLabels}
                        {#each sortedTermLabels as label}
                            <LabelBadge {label} />
                        {:else}
                            <span class="text-secondary">No labels</span>
                        {/each}
                        <button class="btn btn-outline-secondary btn-sm mb-0 py-0 px-1" on:click={openEditLabelsModal}><i class="bi-plus-lg"/></button>
                    {/if}
                </div>
            {/if}
        </div>
        <div class="row">
            <div class="form-group">
                <textarea bind:value={term.notes} class="form-control mb-0" style="resize: none" placeholder="Notes"></textarea>
            </div>
        </div>
    </div>
    {#if $$slots.right}
        <div class="flex-shrink-1">
            <slot name="right" />
        </div>
    {/if}
</div>