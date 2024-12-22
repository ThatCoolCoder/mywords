<!-- routify:meta _noLeftPad -->
<!-- routify:meta _noRightPad -->
<!-- routify:meta _noTopPad -->
<!-- routify:meta _noBottomPad -->

<script>
    import { onMount } from "svelte";
    import { writable } from "svelte/store";
    
    import api from "services/api";
    import { display } from "services/errorService";
    import { enterNotifier } from "shared/misc/enterNotifier";
    import { PracticeAnswerResult } from "data/practiceAnswerResult";
    import { TermLists } from "data/termLists";
    import { isCorrect } from "services/practiceGrading";
    
    import ApiDependent from "shared/misc/ApiDependent.svelte";
    import PracticeSettingsEditor from "shared/PracticeSettingsEditor.svelte";
    import BackButton from "shared/misc/BackButton.svelte";

    export let collectionId;
    export let collection;
    export let terms;
    export let labels;

    const State = {
        Setup: 0,
        InRound: 1,
        PostRound: 2
    }

    let state = State.Setup;

    // Setup stuff

    let defaultSettings = null;
    let settings = null;
    onMount(async () => {
        defaultSettings = await api.safe.getJson(`practice/settings/default`, "Failed fetching practice settings");
        settings = defaultSettings;
    });

    async function startPractice() {
        await startRound();
        state = State.InRound;
    }

    // round stuff (yes I know the presence of this comment implies I should move the whole section into a new component)

    const RoundSubState = {
        AwaitingAnswer: 0,
        ShowingSkip: 1,
        ShowingAnswerResult: 2,
    }

    let currentRoundTerms = [];
    let currentTermIndex = 0;
    let currentTerm = null;
    let subState = RoundSubState.AwaitingAnswer;
    let currentAnswer = "";
    let currentAnswerResult = null;

    async function startRound() {
        currentRoundTerms = await api.safe.postJson(`practice/newround/${collectionId}`, settings);
        if (currentRoundTerms.length == 0) display("Error generating round", "for some reason no terms were returned");
        currentTermIndex = -1;
        nextTerm();
    }
    
    async function submit() {
        if (currentAnswer.trim().length == 0) {
            skip();
            return;
        }

        let correct = isCorrect(currentAnswer, currentTerm.definition);

        var result = await api.safe.postJson(`practice/submitanswer/${currentTerm.id}/${correct ? "correct" : "incorrect"}`);

        terms.update(t => {
            t[t.indexOf(currentTerm)] = result.term;
            return t;
        });

        currentAnswerResult = result.result;
        if (currentAnswerResult != PracticeAnswerResult.StillInSameList) {
            subState = RoundSubState.ShowingAnswerResult;
        }
        else if (correct) {
            nextTerm();
        }
        else {
            subState = RoundSubState.ShowingIncorrect;
        }
    }

    async function skip() {
        subState = RoundSubState.ShowingSkip;
    }

    function nextTerm() {
        currentTermIndex ++;
        currentAnswer = "";
        if (currentTermIndex >= currentRoundTerms.length) {
            state = State.PostRound;
        }
        else {
            currentTerm = $terms.find(t => t.id == currentRoundTerms[currentTermIndex]);
            subState = RoundSubState.AwaitingAnswer;
        }
    }

    async function saveCurrentTermAndMoveOn() {
        await api.safe.put(`terms/${currentTerm.id}`, currentTerm, "Failed updating term");
        nextTerm();
    }

    let id = randomId();
</script>

<title>Practice | {$collection?.name ?? "Collection"} | MyWords</title>

<ApiDependent ready={$collection != null && $terms != null && $labels != null}>
    {#if state == State.Setup}
        <!-- todo: save 2 levels of indentation by moving setup and execution into separate files -->
        
        <div class="row h-100 gx-0">
            <div class="px-0 col-xs-12 col-lg-8 col-xl-8 col-xxl-6 d-flex flex-column gap-2">
                <div class="d-flex flex-column gap-2 px-4 py-4 align-items-start">
                    <BackButton text="Back to {$collection.name}" href="/collections/{collectionId}" />
                    <h2>Setup practice - {$collection.name}</h2>
                    <ApiDependent ready={settings != null}>
                        <PracticeSettingsEditor {settings} {collectionId} />
                    </ApiDependent>
                </div>
                <div class="mt-auto d-flex flex-row bg-light border-top p-3">
                    <button class="btn btn-lg btn-primary ms-auto mb-0" on:click={startPractice}>Start</button>
                </div>
            </div>
            
            <div class="vr col-1 px-0 d-none d-lg-block"></div>
        </div>
    {:else if state == State.InRound}
        <div class="h-100 w-100 d-flex flex-column px-5 bg-light m-0 justify-content-center align-items-center">
            <div class="flex-grow-1 d-flex flex-column border rounded-3 bg-white px-4 py-3" style="max-height: 25rem; width: 50rem; max-width: 100%">
                {#if subState == RoundSubState.AwaitingAnswer}
                    <span class="text-secondary">Term {currentTermIndex + 1} / {currentRoundTerms.length}</span>
                    <h3>{currentTerm.value}</h3>
                    <div class="form-floating mt-auto">
                        <input class="form-control" id={id + "answer"} bind:value={currentAnswer} use:enterNotifier on:enter_pressed={submit}/>
                        <label for={id + "answer"}>Enter definition</label>
                    </div> 
                    <div class="d-flex justify-content-end gap-2">
                        <button class="btn btn-outline-secondary" on:click={skip}>Skip</button>
                        <button class="btn btn-primary" on:click={submit}>Submit</button>
                    </div>
                {:else if subState == RoundSubState.ShowingSkip}
                    <h3>Term skipped</h3>
                    <p class="mt-4 lead ">Definition: {currentTerm.definition}</p>
                    <button class="btn btn-primary mt-auto align-self-end" on:click={nextTerm}>Continue</button>
                {:else if subState == RoundSubState.ShowingAnswerResult}
                    {#if currentAnswerResult == PracticeAnswerResult.CanReturnToLearning}
                        <h3>Oops, that's not the right answer!</h3>
                        <p class="mt-4 lead">Do you want to move this term back to learning?</p>
                        <div class="d-flex mt-auto justify-content-end gap-2">
                            <button class="btn btn-outline-secondary" on:click={nextTerm}>No</button>
                            <button class="btn btn-primary" on:click={() => {currentTerm.termList = TermLists.Learning; saveCurrentTermAndMoveOn();}}>Ok</button>
                        </div>
                    {:else if currentAnswerResult == PracticeAnswerResult.CanMoveToRecentlyLearned}
                        <h3>You've gotten this term right a few times</h3>
                        <p class="mt-4 lead">Do you want to move this term into recently learned?</p>
                        <p>You'll still be tested on it at a lesser frequency, so that you can complete the learning process</p>
                        <div class="d-flex mt-auto justify-content-end gap-2">
                            <button class="btn btn-outline-secondary" on:click={nextTerm}>Not yet</button>
                            <button class="btn btn-primary" on:click={() => {currentTerm.termList = TermLists.RecentlyLearned; saveCurrentTermAndMoveOn();}}>Ok</button>
                        </div>
                    {:else}
                        <h3>You've gotten this term right a few times</h3>
                        <p class="mt-4 lead">Do you want to move it to learned?</p>
                        <p>You'll still be tested on it periodically to ensure that you don't forget it</p>
                        <div class="d-flex mt-auto justify-content-end gap-2">
                            <button class="btn btn-outline-secondary" on:click={nextTerm}>Not yet</button>
                            <button class="btn btn-primary" on:click={() => {currentTerm.termList = TermLists.Learned; saveCurrentTermAndMoveOn();}}>Ok</button>
                        </div>
                    {/if}
                {:else}
                    <h3>Oops, that's not the right answer!</h3>
                    <p class="mt-4 lead">Term: {currentTerm.value}</p>
                    <p class="mt-2 lead">You answered: {currentAnswer}</p>
                    <p class="mt-2 lead">Correct answer: {currentTerm.definition}</p>
                    <button class="btn btn-primary mt-auto align-self-end" on:click={nextTerm}>Continue</button>
                {/if}
            </div>
        </div>
    {:else}
        haha there is nothing here either. You finished practice
        <button class="btn btn-primary" on:click={() => state = State.Setup}>New round</button>
    {/if}
</ApiDependent>