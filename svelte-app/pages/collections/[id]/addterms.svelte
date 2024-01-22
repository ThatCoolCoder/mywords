<script>
    import { onMount, getContext } from 'svelte';
    import { writable } from 'svelte/store';

    import { TermLists, TermListDisplayNames } from 'data/termLists.js';
    
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import TermList from 'shared/TermList.svelte';
    import TermCard from 'shared/TermCard.svelte';
    import EditTermLabels from 'shared/EditTermLabels.svelte';
    import LabelBadge from 'shared/LabelBadge.svelte';
    import api from 'services/api';

    export let setId;
    export let set;
    export let terms;
    export let labels;

    const { open, close } = getContext('simple-modal');

    let generalSettings = {
        termList: 0
    };

    let currentNewTerm;
    clearCurrentNewTerm();
    $: sortedNewTermLabels = currentNewTerm.labels
        .map(labelId => $labels.filter(x => x.id == labelId)[0])
        .toSorted((a, b) => a.name.compareTo(b.name));

    function addCurrentTerm() {
        let term = currentNewTerm;
        term.termList = generalSettings.termList;
        term.movedToCurrentListUtc
        clearCurrentNewTerm();
        terms.update(x => x.pushed(term));
        api.post(`terms/`, term, 'Failed to save term');
    }

    function clearCurrentNewTerm() {
        currentNewTerm = {
            id: -1,
            termSetId: setId,
            value: '',
            definition: '',
            notes: '',
            currentStreak: 0,
            labels: []
        };
    }

    function openEditLabelsModal() {
        let w = writable(currentNewTerm.labels);
        w.subscribe(newVal => currentNewTerm.labels = newVal);
        open(EditTermLabels, {availableLabels: labels, labels: w});
    }

</script>

<script context="module">
    // I spent an hour or something trying to get this to work with css, it just wouldn't stay inside the flexbox
    // So I give up and make it with js
    setInterval(() => {
        for (let e of document.getElementsByClassName('add-term-button')) {
            e.style.width = e.clientHeight + "px";
        }
    }, 100);
</script>

<title>Add Terms | MyWords</title>

<ApiDependent ready={set != null}>
    <div class="d-flex flex-column gap-2" style="max-width: 1500px">
        <h2>Add terms - { $set.name }</h2>
        
        <fieldset class="border text-start d-flex flex-column gap-3 p-3">
            <legend class="float-none w-auto px-3">General settings</legend>

            <div class="form-group d-flex gap-2 align-items-center">
                <label for="listSelect">List to insert terms into
                    <select class="mb-0 w-auto form-select" bind:value={generalSettings.termList}>
                        {#each Object.keys(TermLists) as listName}
                            <option value={TermLists[listName]}>{TermListDisplayNames[TermLists[listName]]}</option>
                        {/each}
                    </select>
                </label>
            </div>

            <div on:click={openEditLabelsModal} class="form-group d-flex gap-2 justify-content-start align-items-center">
                <span>Labels for added terms:</span>
                {#each sortedNewTermLabels as label}
                    <LabelBadge {label} />
                {:else}
                    <span class="text-secondary">(None)</span>
                {/each}
                <button class="btn btn-outline-secondary btn-sm mb-0 py-0 px-1" aria-label="edit labels"><i class={sortedNewTermLabels.length == 0 ? "bi-plus-lg" : "bi-pencil" }/></button>
            </div>
        </fieldset>

        <br />

        <div>
            <TermCard bind:term={currentNewTerm} showTermList={false} showLabels={false}>
                <button class="btn btn-outline-secondary h-100 add-term-button" slot="right" on:click={addCurrentTerm}><i class="bi-plus-lg" /></button>
            </TermCard>
        </div>

        <hr />

        <h3>Recently added</h3>
        <TermList termsWritable={terms} termSetId={setId} showTermLists={true} />

    </div>
    
</ApiDependent>