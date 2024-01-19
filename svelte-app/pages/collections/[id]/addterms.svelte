<script>
    import { onMount, getContext } from 'svelte';
    import { writable } from 'svelte/store';

    import api from 'services/api.js';
    import { TermLists, TermListDisplayNames } from 'data/termLists.js';
    
    import ApiDependent from 'shared/misc/ApiDependent.svelte';
    import TermList from 'shared/TermList.svelte';
    import TermCard from 'shared/TermCard.svelte';

    export let setId;
    export let set;
    export let terms;
    export let labels;

    let generalSettings = {
        termList: 0
    };

    let currentNewTerm;
    clearCurrentNewTerm();
    
    onMount(async () => {

        
    });

    function addCurrentTerm() {
        let term = currentNewTerm;
        term.termList = generalSettings.termList;
        clearCurrentNewTerm();
        terms.update(x => x.pushed(term));
    }

    function clearCurrentNewTerm() {
        currentNewTerm = {
            value: '',
            definition: '',
            notes: '',
            labels: []
        };
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
    <div class="d-flex flex-column gap-3">
        <h2>Add terms - { $set.name }</h2>
        
        <fieldset style="max-width: 1500px" class="border p-2 text-start">
            <legend class="float-none w-auto px-3">General settings</legend>

            <div class="form-group d-flex gap-2 align-items-center">
                <label for="listSelect">List to insert terms into
                </label>
                <select class="mb-0 w-auto form-select" name="listSelect" bind:value={generalSettings.termList}>
                    {#each Object.keys(TermLists) as listName}
                        <option value={TermLists[listName]}>{TermListDisplayNames[TermLists[listName]]}</option>
                    {/each}
                </select>
            </div>

            <div>
                <p>Placeholder for a thing to select which labels you want added</p>
            </div>
        </fieldset>

        <br />

        <div style="max-width: 1500px" >
            <TermCard bind:term={currentNewTerm}>
                <!-- todo: make this icon fill the button -->
                <button class="btn btn-outline-secondary h-100 add-term-button" slot="right" on:click={addCurrentTerm}><i class="bi-plus-lg" /></button>
            </TermCard>
        </div>
        
        <!-- <div class="card p-3">
            <div class="row">
                <div class="col-xs-12 col-md-4 col-xxl-3 form-group">
                    <input class="form-control" placeholder="Value" bind:value={currentNewTerm.value} />
                </div>
                <div class="col-xs-12 col-md-4 col-xxl-3 form-group">
                    <input class="form-control" placeholder="Definition" bind:value={currentNewTerm.definition} />
                </div>
                <!-- Todo: big plus button on the right filling all sub-rows
            </div>
            <div class="row">
                <div class="form-group">
                    <textarea bind:value={currentNewTerm.notes} class="form-control" style="resize: none" placeholder="Notes"></textarea>
                </div>
            </div>
        </div> -->


        <div class="row mt-3">
            {#each Object.keys(TermLists) as listName}
                <div class="col-xs-1 col-lg-6 col-xl-4 col-xxl-3">
                    <h4>{TermListDisplayNames[TermLists[listName]]}</h4>
                    <hr />
                    <TermList termsWritable={terms} termSetId={setId} termList={TermLists[listName]} />
                </div>
            {/each}
        </div>

    </div>
    
</ApiDependent>