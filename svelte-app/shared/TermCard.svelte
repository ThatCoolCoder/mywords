<script context="module">
    export const WidthMode = {
        Full: 0,
        Half: 1,
        Quarter: 2,
    }
</script>

<script>
    import { getContext } from 'svelte';
    import { writable } from 'svelte/store';

    import { TermListDisplayNames } from 'data/termLists.js';
    import api from 'services/api';

    import { clickOutside } from 'shared/misc/clickOutside.js';
    import { hoverNotifier } from 'shared/misc/hoverNotifier';
    import LabelBadge from 'shared/LabelBadge.svelte';
    import EditTermLabels from 'shared/EditTermLabels.svelte';

    const { open, close } = getContext('simple-modal');
    let availableLabels = getContext('labels');
    
    export let term = {value: '', definition: '', termList: 0, notes: '', labels: []};
    export let showTermList = true; // show what term list it's in
    
    export let showLabels = true;
    export let widthMode = WidthMode.Full;
    
    export let dragAndDropEnabled = false; // enable dragging labels between lists

    export let syncWithApi = false; // false by default because it's a little dangerous
    export let editable = true;
    export let editing = false;
    export let forcedEditing = false;

    export let onDeleted = null;

    let hovering = false;

    const primaryInputSizings = {
        [WidthMode.Full] : "col-xs-12 col-md-4 col-xxl-3",
        [WidthMode.Half] : "col-12 col-xl-6",
        [WidthMode.Quarter] : "col-12 col-xxl-6"
    };

    const labelsRowSizings = {
        [WidthMode.Full] : "col-xl-12 col-xxl-6",
        [WidthMode.Half] : "col-12",
        [WidthMode.Quarter] : "col-12"
    };

    // Sorry these two are janky as they rely on the knowledge of the column breakpoints in the index.svelte
    const previewTextSizings = {
        [WidthMode.Full] : "col-xl-2 col-lg-2 col-md-3 col-sm-4 col-6",
        [WidthMode.Half] : "col-xl-2 col-lg-3 col-md-4 col-6",
        [WidthMode.Quarter] : "col-xxl-6 col-xl-6 col-lg-3 col-md-3 col-6"
    };

    const previewLabelsSizings = {
        [WidthMode.Full] : "col-xl-8 col-lg-8 col-md-6 col-xs-12",
        [WidthMode.Half] : "col-xxl-12 col-xl-6 col-lg-6 col-12",
        [WidthMode.Quarter] : "col-xxl-12 col-xl-12 col-lg-6 col-sm-6 col-12"
    };

    let sortedTermLabels;
    $: sortedTermLabels = term.labels.map(labelId => $availableLabels.filter(x => x.id == labelId)[0]).toSorted((a, b) => a.name.compareTo(b.name));

    function tryEdit() {
        if (editable) editing = true;
    }

    function closeEdit() {
        editing = false;
    }

    function onHoverChanged(newValue) {
        hovering = newValue.detail;
    }
    
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
        if (confirm(`Are you sure you want to delete the term ${term.value} (${term.definition.length == 0 ? 'no definition' : term.definition})?`)) {
            api.post(`terms/${term.id}/delete/`);
            if (onDeleted != null) onDeleted(term);
            closeEdit();
        }
    }

    function onDragStart(e) {
        e.dataTransfer.setData("text/plain", term.id);
        e.dataTransfer.dropEffect = "move";
    }
</script>

<style>
    .delete-button {
        opacity: 0%;
        transition: opacity 0.2s ease-in-out;
    }
    
    .delete-button[data-visible="true"] {
        opacity: 100%;
    }
</style>

<!-- Yes, this has become hideously nested and indented. No, I don't care -->
<div use:clickOutside on:click_outside={closeEdit}
    use:hoverNotifier on:hover_changed={onHoverChanged}
    class="card p-2 d-flex flex-row gap-2"
    draggable={dragAndDropEnabled} on:dragstart={onDragStart} on:dragstart on:dragend>
    {#if editing || forcedEditing}
        <div class="flex-grow-1 d-flex flex-column gap-2">
            <div class="d-flex gap-2">
                <!-- value/definition plus labels -->
                <div class="row flex-grow-1 gy-2">
                    <div class={"form-group " + primaryInputSizings[widthMode]}>
                        <input on:change={update} bind:value={term.value} class={"form-control mb-0 " + (term.value == "" ? "" : "fw-bold")} placeholder="Term" />
                    </div>
                    <div class={"form-group " + primaryInputSizings[widthMode]}>
                        <input on:change={update} bind:value={term.definition} class="form-control mb-0" placeholder="Definition" />
                    </div>
                
                    <!-- Labels, term list and delete, which collapse to below the main inputs when it gets too small -->
                    <div class={"d-flex gap-2 justify-content-start align-items-center " + labelsRowSizings[widthMode]}>
                        {#if showTermList}
                            <span class="badge rounded-pill h-auto bg-secondary w-auto">
                                {TermListDisplayNames[term.termList]}
                            </span>
                            <div class="vr"></div>
                        {/if}
                        {#if showLabels}
                            <div class="d-flex flex-row gap-2 flex-wrap" on:click={openEditLabelsModal} on:keypress={(e) => e.key == "Enter" && openEditLabelsModal() }>
                                {#each sortedTermLabels as label}
                                    <LabelBadge {label} />
                                {:else}
                                    <span class="text-secondary">No labels</span>
                                {/each}
                                <button class="btn btn-outline-secondary btn-sm mb-0 py-0 px-1 h-100 btn-block" title="Delete term">
                                    <i class={sortedTermLabels.length == 0 ? "bi-plus-lg" : "bi-pencil"}/>
                                </button>
                            </div>
                        {/if}
                        {#if syncWithApi}
                            <button class="btn btn-outline-danger p-1 mb-auto ms-auto" on:click={askDeleteTerm}><i class="bi-trash mb-0"/></button>
                        {/if}
                    </div>
                </div>
            </div>
            <!-- Notes/description -->
            <div class="row">
                <div class="form-group">
                    <textarea on:change={update} bind:value={term.notes} class="form-control mb-0" style="resize: none" placeholder="Notes"></textarea>
                </div>
            </div>
        </div>
    {:else}
        <!-- svelte-ignore a11y-click-events-have-key-events a11y-interactive-supports-focus -->
        <div class="row flex-grow-1" role="button" on:click={tryEdit}>
            <div class="d-flex">
                <div class="d-flex gap-2 flex-wrap">
                    <div class="d-flex gap-2">
                    <!-- <div class={previewTextSizings[widthMode]}> -->
                        <span class="fw-bold">{term.value}</span>
                    <!-- </div>
                    <div class={previewTextSizings[widthMode]}> -->
                        <span class={term.definition.length == 0 ? "text-secondary" : ""}>{term.definition.length == 0 ? "(No definition)" : term.definition}</span>
                    <!-- </div> -->
                
                    </div>
                    <div class={"d-flex gap-2 justify-content-start align-items-center flex-wrap "}>
                        {#if showTermList}
                            <span class="badge rounded-pill h-auto bg-secondary w-auto">
                                {TermListDisplayNames[term.termList]}
                            </span>
                        {/if}
                        {#if showLabels}
                            {#each sortedTermLabels as label}
                                <LabelBadge {label} />
                            {/each}
                        {/if}
                    </div>
                </div>
                {#if syncWithApi}
                    <button class="btn btn-outline-danger px-1 py-0 ms-auto my-auto delete-button"
                        data-visible={hovering}
                        on:click={askDeleteTerm}><i class="bi-trash mb-0"/></button>
                {/if}
            </div>
        </div>
    {/if}

    {#if $$slots.right}
        <div class="flex-shrink-1">
            <slot name="right" />
        </div>
    {/if}
</div>