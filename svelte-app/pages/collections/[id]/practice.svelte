<!-- routify:meta _noLeftPad -->
<!-- routify:meta _noBottomPad -->

<script>
    import { onMount } from "svelte";
    
    import api from "services/api";

    import ApiDependent from "shared/misc/ApiDependent.svelte";
    import PracticeSettingsEditor from "shared/PracticeSettingsEditor.svelte";

    export let collectionId;
    export let collection;
    export let terms;
    export let labels;

    let practiceStarted = false;

    let defaultSettings = null;
    let settings = null;
    onMount(async () => {
        defaultSettings = await api.get(`practice/settings/default`, "Failed fetching practice settings");
        settings = defaultSettings;
    });
</script>

<title>Practice | {$collection?.name ?? "Collection"} | MyWords</title>

<ApiDependent ready={$collection != null && $terms != null && $labels != null}>
    {#if ! practiceStarted}
        <!-- todo: save 2 levels of indentation by moving setup and execution into separate files -->
        <div class="row h-100">
            <div class="px-0 col-xs-12 col-lg-8 col-xl-8 col-xxl-6 d-flex flex-column gap-2">
                <div class="d-flex flex-column gap-2 px-4">
                    <h2>Setup practice - {$collection.name}</h2>
                    <ApiDependent ready={settings != null}>
                        <PracticeSettingsEditor {settings}/>
                    </ApiDependent>
                </div>
                <div class="mt-auto d-flex flex-row bg-light border-top p-3">
                    <button class="btn btn-lg btn-primary ms-auto mb-0">Start</button>
                </div>
            </div>
            
            <div class="vr col-1 px-0 d-none d-lg-block"></div>
        </div>
    {:else}
        haha there is nothing here
    {/if}
</ApiDependent>