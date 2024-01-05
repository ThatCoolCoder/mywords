<script>
	import { Router } from "@roxi/routify";
	import routes from "./.routify/routes.default.js";

	const config = {
		urlTransform: {
			remove: url => url.toLowerCase()
		}
	}
</script>

<Router {config} {routes} />