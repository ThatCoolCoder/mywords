<script>
    import { hoverNotifier } from 'shared/misc/hoverNotifier';

    export let click;
    export let onHoverChanged = () => {};
</script>

<div class="col-xs-12 col-lg-6 col-xxl-3" use:hoverNotifier on:hover_changed={onHoverChanged}>
    <div role="button" class="card p-3 h-100" style="min-height: 9em" on:click={click}>
        <slot />
    </div>
</div>