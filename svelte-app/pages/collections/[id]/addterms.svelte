<script context="module">
    // I spent an hour or something trying to get this to work with css, it just wouldn't stay inside the flexbox
    // So I give up and make it with js
    setInterval(() => {
        for (let e of document.getElementsByClassName("add-term-button")) {
            e.style.width = e.clientHeight + "px";
            e.style.maxWidth = "20vw";
        }
    }, 100);
</script>

<script>
    import { getContext } from "svelte";
    import { derived, writable } from "svelte/store";

    import { TermLists } from "data/termLists.js";
    import api from "services/api";

    import ApiDependent from "shared/misc/ApiDependent.svelte";
    import TermList from "shared/TermList.svelte";
    import TermCard from "shared/TermCard.svelte";
    import EditTermLabels from "shared/EditTermLabels.svelte";
    import LabelBadge from "shared/LabelBadge.svelte";
    import HelpButton from "shared/misc/HelpButton.svelte";
    import BackButton from "shared/misc/BackButton.svelte";

    export let collectionId;
    export let collection;
    export let terms;
    export let labels;

    let recentTerms = derived(terms, $terms => $terms.filter(t => {
        return Date.now() - new Date(t.createdUtc) < 1000 * 3600 * 24 * 2;
    }).toSorted((a, b) => new Date(b.createdUtc) - new Date(a.createdUtc))); // if it gets slow then don't parse the dates 3 times

    const termListsHelp =
        "MyWords offers multiple lists that you can organise your words into.\n" +
        "Backlog is for words which you have come across but haven't yet found the meaning of\n" +
        "Learning is for words that you have found the meaning of but have not yet memorised this\n" +
        "Auto mode will sort newly added words sorted based on whether you have added a definition or not";

    const { open, close } = getContext("simple-modal");

    const TermListModes = {
        Backlog: 0,
        Learning: 1,
        Auto: 2,
    };

    let generalSettings = {
        termListMode: TermListModes.Auto,
    };

    let currentNewTerm;
    clearCurrentNewTerm();
    $: sortedNewTermLabels = currentNewTerm.labels
        .map((labelId) => $labels.filter((x) => x.id == labelId)[0])
        .toSorted((a, b) => a.name.compareTo(b.name));

    async function addCurrentTerm() {
        let term = currentNewTerm;

        if (generalSettings.termListMode == TermListModes.Backlog)
            term.termList = TermLists.Backlog;
        else if (generalSettings.termListMode == TermListModes.Learning)
            term.termList = TermLists.Learning;
        else if (generalSettings.termListMode == TermListModes.Auto) {
            term.termList =
                term.definition.trim() == ""
                    ? TermLists.Backlog
                    : TermLists.Learning;
        }

        clearCurrentNewTerm();
        let newTermData = await api.safe.postJson(`terms/`, term, "Failed to save term");
        terms.update((x) => x.pushed(newTermData));
    }

    function clearCurrentNewTerm() {
        currentNewTerm = {
            id: -1,
            collectionId,
            value: "",
            definition: "",
            notes: "",
            currentStreak: 0,
            labels: currentNewTerm?.labels ?? [],
        };
    }

    function openEditLabelsModal() {
        let w = writable(currentNewTerm.labels);
        w.subscribe((newVal) => (currentNewTerm.labels = newVal));
        open(EditTermLabels, { availableLabels: labels, labels: w });
    }
</script>

<title>Add Terms | MyWords</title>

<ApiDependent ready={collection != null}>

    <BackButton text="Back to collection" href="/collections/{collectionId}" />

    <div class="d-flex flex-column gap-2" style="max-width: 1500px">
        <h2>Add terms - {$collection.name}</h2>

        <fieldset class="d-flex flex-column gap-3">
            <legend>General settings</legend>

            <div class="form-group d-flex gap-2 align-items-center">
                <label for="listSelect">List to insert terms into</label>
                <select id="listSelect" class="mb-0 w-auto form-select" bind:value={generalSettings.termListMode} >
                    {#each Object.keys(TermListModes) as modeName}
                        <option value={TermListModes[modeName]}>{modeName}</option>
                    {/each}
                </select>
                <HelpButton topic="Term lists" text={termListsHelp} />
            </div>

            <!-- svelte-ignore a11y-click-events-have-key-events -->
            <div on:click={openEditLabelsModal} class="form-group d-flex flex-wrap gap-2 justify-content-start align-items-center">
                <span>Labels for added terms:</span>
                {#each sortedNewTermLabels as label}
                    <LabelBadge {label} />
                {:else}
                    <span class="text-secondary">(None)</span>
                {/each}
                <button class="btn btn-outline-secondary btn-sm mb-0 py-0 px-1" aria-label="edit labels">
                    <i class={sortedNewTermLabels.length == 0 ? "bi-plus-lg" : "bi-pencil"}/>
                </button>
            </div>
        </fieldset>

        <br />

        <div>
            <TermCard bind:term={currentNewTerm} showTermList={false} showLabels={false} forcedEditing={true}>
                <button class="btn btn-outline-secondary h-100 add-term-button" slot="right" on:click={addCurrentTerm}><i class="bi-plus-lg" /></button>
            </TermCard>
        </div>

        <hr />

        <h3>Recently added</h3>
        <TermList termsStore={recentTerms} baseTermsWritable={terms} showTermLists={true} sortFunc={(a, b) => new Date(b.createdUtc) - new Date(a.createdUtc)} noTermsText="Nothing added recently" />
    </div>
</ApiDependent>
