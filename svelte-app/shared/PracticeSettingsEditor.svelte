<script>
    import HelpButton from "shared/misc/HelpButton.svelte";

    import api from "services/api";
    import { onMount } from "svelte";
    import { writable } from "svelte/store";
    import { navigate } from "pages/utils";

    export let settings;
    export let collectionId;
    let collections = writable([]);
    export let allowChangingSet = true;

    onMount(async () => {
        collections.set(await api.safe.getJson('collections'));
    });

    let id = randomId();
</script>



<div class="d-flex flex-column gap-2">
    <fieldset>
        <legend>General</legend>
        {#if allowChangingSet}
            <div class="row align-items-center mb-2">
                <label class="col-auto" for={id + "list"}>Collection to use</label>
                <div class="col-auto">
                    <select class="form-select mb-0" id={id + "list"} on:change={e => navigate(`/collections/${e.target.value}/practice`)}>
                        {#each $collections
                            .toSorted((a, b) => (a.name.toLowerCase() > b.name.toLowerCase() ? 1 : (a.name.toLowerCase() < b.name.toLowerCase() ? -1 : 0)))
                            as collection}
                            <option value={collection.id} selected={collection.id == collectionId}>{collection.name}</option>
                        {/each}
                    </select>
                </div>
            </div>
        {/if}

        <div class="row align-items-center">
            <label class="col-auto" for={id + "size"}>Amount of terms</label>
            <div class="col-auto">
                <input class="mb-0 form-control" type="number" size=6 min=3 id={id + "size"} bind:value={settings.roundLength}/>
            </div>
        </div>

    </fieldset>

    <fieldset>
        <legend>Labels</legend>
        <p>todo: make a bearable label list editor and use it here</p>
    </fieldset>

    <fieldset class="d-flex flex-column">
        <legend>Review </legend>
        <div class="d-flex gap-2 align-items-center">
            <label class="form-check-label" for={id + "toggleRecent"}>Include recent review</label>
            <input class="form-check-input mb-0" type="checkbox" id={id + "toggleRecent"} bind:checked={settings.includeRecentReview} />
            <HelpButton topic="Recent review" text="If enabled, recent review will introduce some recently learned terms into practice sessions. This helps build long-term memory of the terms." /> 
        </div>
        {#if settings.includeRecentReview}
            <div class="row align-items-center">
                <label class="col-auto" for={id + "recentAmount"}>Recent review proportion</label>
                <div class="col-auto">
                    <input class="mb-0 form-control" type="number" size=6 min=0 max=0.5 step=0.05 id={id + "recentAmount"} bind:value={settings.recentReviewProportion}/>
                </div>
            </div>
        {/if}
        <div class="d-flex gap-2 align-items-center mt-2">
            <label class="form-check-label" for={id + "toggleLate"}>Include late review</label>
            <input class="form-check-input mb-0" type="checkbox" id={id + "toggleLate"} bind:checked={settings.includeLateReview} />
            <HelpButton topic="Late review" text="If enabled, late review will introduce some learned terms into practice sessions. (Planned, todo:) it will favour terms that you have historically gotten correct less often." /> 
        </div>
        {#if settings.includeLateReview}
            <div class="row align-items-center">
                <label class="col-auto" for={id + "lateAmount"}>Late review proportion</label>
                <div class="col-auto">
                    <input class="mb-0 form-control" type="number" size=6 min=0 max=0.5 step=0.05 id={id + "lateAmount"} bind:value={settings.lateReviewProportion}/>
                </div>
            </div>
        {/if}
    </fieldset>
</div>