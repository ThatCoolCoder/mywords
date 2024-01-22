<script context="module">
    export const WidthMode = {
        Full: 0,
        Half: 1,
        Quarter: 2,
    }
</script>

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
    export let onDeleted = null;
    export let widthMode = WidthMode.Full;

    let primaryInputSizings = {
        [WidthMode.Full] : "col-xs-12 col-md-4 col-xxl-3",
        [WidthMode.Half] : "col-12 col-xxl-3",
        [WidthMode.Quarter] : "col-12 col-xxl-3"
    };

    let labelsRowSizings = {
        [WidthMode.Full] : "col-xl-12 order-xl-1 order-xxl-5 col-xxl-3",
        [WidthMode.Half] : "col-12 order-1",
        [WidthMode.Quarter] : "col-12 order-1"
    };

    let sortedTermLabels;
    $: sortedTermLabels = term.labels.map(labelId => $availableLabels.filter(x => x.id == labelId)[0]).toSorted((a, b) => a.name.compareTo(b.name));
    
    function openEditLabelsModal() {
        let w = writable(term.labels);
        w.subscribe(newVal => term.labels = newVal);
        open(EditTermLabels, {availableLabels, labels: w}, {}, {
            onClosed: update
        });
    }

    function update() {
        if (syncWithApi) {
            api.put(`terms/${term.id}`, term);
        }
    }

    function askDeleteTerm() {
        // todo: make it not use confirm
        if (confirm(`Are you sure you want to delete the term ${term.value} (${term.definition})?`)) {
            api.post(`terms/${term.id}/delete/`);
            if (onDeleted != null) onDeleted(term);
        }
    }
</script>

<div class="card p-2 d-flex flex-row gap-2">
    <div class="flex-grow-1 d-flex flex-column gap-2">
        <div class="d-flex gap-2">
            <!-- value/definition plus labels -->
            <div class="row flex-grow-1">
                <div class={"order-2 form-group " + primaryInputSizings[widthMode]}>
                    <input on:change={update} bind:value={term.value} class="form-control mb-0 fw-bold" placeholder="Term" />
                </div>
                <div class={"order-3 form-group " + primaryInputSizings[widthMode]}>
                    <input on:change={update} bind:value={term.definition} class="form-control mb-0" placeholder="Definition" />
                </div>
            
                <!-- Labels and term list, which collapse to above the main inputs when it gets too small -->
                {#if showTermList || showLabels}
                    <div class={"d-flex gap-2 justify-content-start align-items-center mb-2 " + labelsRowSizings[widthMode]}>
                        {#if showTermList}
                            <span class="badge rounded-pill h-auto bg-secondary w-auto">
                                {TermListDisplayNames[term.termList]}
                            </span>
                            <div class="vr"></div>
                        {/if}
                        {#if showLabels}
                            <div class="d-flex flex-row gap-2" on:click={openEditLabelsModal} >
                                {#each sortedTermLabels as label}
                                    <LabelBadge {label} />
                                {:else}
                                    <span class="text-secondary">No labels</span>
                                {/each}
                                <button class="btn btn-outline-secondary btn-sm mb-0 py-0 px-1"><i class={sortedTermLabels.length == 0 ? "bi-plus-lg" : "bi-pencil"}/></button>
                            </div>
                        {/if}
                    </div>
                {/if}
            </div>
            <!-- Close button -->
            {#if syncWithApi}
                <button class="btn btn-outline-danger p-1 mb-auto ml-auto" on:click={askDeleteTerm}><i class="bi-trash mb-0"/></button>
            {/if}
        </div>
        <div class="row">
            <div class="form-group">
                <textarea on:change={update} bind:value={term.notes} class="form-control mb-0" style="resize: none" placeholder="Notes"></textarea>
            </div>
        </div>
    </div>
    {#if $$slots.right}
        <div class="flex-shrink-1">
            <slot name="right" />
        </div>
    {/if}
</div>