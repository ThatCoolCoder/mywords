<script>
    import { onMount } from "svelte";
    import { writable } from "svelte/store";
	let count = writable(1);

	// the `$:` means 're-run whenever these values change'
	$: data = {c: count};
	$: z = JSON.stringify(data);

	onMount(() => {
		count = writable(4);
		console.log(data);
	});
</script>

<p>doubled, in obj: {JSON.stringify(data.c)}</p>
<p>doubled, in obj, json: {JSON.stringify(data)}</p>
<p>secondary json: {z}</p>