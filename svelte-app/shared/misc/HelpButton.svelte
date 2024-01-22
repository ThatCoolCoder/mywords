<script>
    import { getContext } from "svelte";

    import HelpPopup from "shared/misc/HelpPopup.svelte";

    const { open, close } = getContext('simple-modal');

    export let topic = '';
    export let text = '';

    function openHelp() {
        open(HelpPopup, { topic, text })
    }
</script>

<button on:click={openHelp} class="btn p-0 mb-0" style="font-size: 1.4em"><i class="text-secondary bi-question-circle-fill" /></button>