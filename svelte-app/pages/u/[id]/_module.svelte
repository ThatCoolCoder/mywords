<script>
    export let id;
</script>

<!-- whyyyy cant children just have hte id populated without _module getting it and passing it to them -->
<slot props={{id}}/>