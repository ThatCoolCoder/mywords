<script>
    import { onMount, getContext } from 'svelte';
    import { writable } from 'svelte/store';

    import api from 'services/api.js';
    import { TermLists, TermListDisplayNames } from 'data/termLists.js';
    
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import TermList from 'shared/TermList.svelte';

    export let setId;
    export let set;
    export let terms;
    export let labels;

    let inputValues = {
        termList: 0
    }
    
    onMount(async () => {

        
    });

    // const { open, close } = getContext('simple-modal');


</script>

<title>Add Terms | MyWords</title>

<ApiDependent ready={set != null}>
    <h2>Add terms - { $set.name }</h2>
    <fieldset class="border p-2 text-start">
        <legend class="float-none w-auto px-3">General settings</legend>

        <div class="form-group d-flex gap-2 align-items-center">
            <label for="listSelect">List to insert terms into
            </label>
            <select class="mb-0 w-auto form-select" name="listSelect" bind:value={inputValues.termList}>
                {#each Object.keys(TermLists) as listName}
                    <option value={TermLists[listName]}>{TermListDisplayNames[TermLists[listName]]}</option>
                {/each}
            </select>
        </div>
    </fieldset>
    
    <hr />

    <TermList termsWritable={terms} termSetId={setId} />
    
</ApiDependent>